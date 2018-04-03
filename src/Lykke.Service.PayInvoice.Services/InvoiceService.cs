using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Log;
using Lykke.Service.PayInternal.Client;
using Lykke.Service.PayInternal.Client.Models.Merchant;
using Lykke.Service.PayInternal.Client.Models.PaymentRequest;
using Lykke.Service.PayInternal.Contract.PaymentRequest;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Exceptions;
using Lykke.Service.PayInvoice.Core.Repositories;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Core.Utils;
using Lykke.Service.PayInvoice.Services.Extensions;

namespace Lykke.Service.PayInvoice.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IFileInfoRepository _fileInfoRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IPayInternalClient _payInternalClient;
        private readonly ILog _log;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            IFileInfoRepository fileInfoRepository,
            IFileRepository fileRepository,
            IPayInternalClient payInternalClient,
            ILog log)
        {
            _invoiceRepository = invoiceRepository;
            _fileInfoRepository = fileInfoRepository;
            _fileRepository = fileRepository;
            _payInternalClient = payInternalClient;
            _log = log;
        }

        public async Task<IReadOnlyList<Invoice>> GetAsync(string merchantId)
        {
            return await _invoiceRepository.GetAsync(merchantId);
        }

        public async Task<Invoice> GetAsync(string merchantId, string invoiceId)
        {
            return await _invoiceRepository.GetAsync(merchantId, invoiceId);
        }

        public async Task<Invoice> GetByIdAsync(string invoiceId)
        {
            return await _invoiceRepository.FindByIdAsync(invoiceId);
        }
        
        public async Task<Invoice> CreateDraftAsync(Invoice invoice)
        {
            invoice.Status = InvoiceStatus.Draft;
            invoice.CreatedDate = DateTime.UtcNow;

            Invoice createdInvoice = await _invoiceRepository.InsertAsync(invoice);

            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(CreateDraftAsync),
                invoice.ToContext().ToJson(), "Invoice draft created");

            return createdInvoice;
        }

        public async Task UpdateDraftAsync(Invoice invoice)
        {
            Invoice sourceInvoice = await _invoiceRepository.GetAsync(invoice.MerchantId, invoice.Id);

            if (sourceInvoice == null)
                throw new InvoiceNotFoundException(invoice.Id);

            if (sourceInvoice.Status != InvoiceStatus.Draft)
                throw new InvalidOperationException("Invoice status is invalid");

            invoice.Status = sourceInvoice.Status;
            invoice.CreatedDate = sourceInvoice.CreatedDate;
            
            await _invoiceRepository.UpdateAsync(invoice);

            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(UpdateDraftAsync),
                invoice.ToContext().ToJson(), "Invoice draft updated");
        }

        public async Task<Invoice> CreateAsync(Invoice invoice)
        {
            PaymentRequestModel paymentRequest = await CreatePaymentRequestAsync(invoice);

            invoice.Status = InvoiceStatus.Unpaid;
            invoice.CreatedDate = DateTime.UtcNow;
            invoice.PaymentRequestId = paymentRequest.Id;
            invoice.WalletAddress = paymentRequest.WalletAddress;

            Invoice createdInvoice = await _invoiceRepository.InsertAsync(invoice);

            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(CreateAsync),
                invoice.ToContext().ToJson(), "Invoice created");

            return createdInvoice;
        }

        public async Task<Invoice> CreateFromDraftAsync(Invoice invoice)
        {
            Invoice sourceInvoice = await _invoiceRepository.GetAsync(invoice.MerchantId, invoice.Id);

            if (sourceInvoice == null)
                throw new InvoiceNotFoundException(invoice.Id);

            if (sourceInvoice.Status != InvoiceStatus.Draft)
                throw new InvalidOperationException("Invoice status is invalid.");

            PaymentRequestModel paymentRequest = await CreatePaymentRequestAsync(invoice);

            invoice.Status = InvoiceStatus.Unpaid;
            invoice.CreatedDate = DateTime.UtcNow;
            invoice.PaymentRequestId = paymentRequest.Id;
            invoice.WalletAddress = paymentRequest.WalletAddress;
            invoice.CreatedDate = sourceInvoice.CreatedDate;

            await _invoiceRepository.UpdateAsync(invoice);

            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(CreateFromDraftAsync),
                invoice.ToContext().ToJson(), "Invoice created from draft");

            return invoice;
        }

        public async Task DeleteAsync(string merchantId, string invoiceId)
        {
            Invoice invoice = await _invoiceRepository.GetAsync(merchantId, invoiceId);

            if (invoice == null)
                return;

            if (invoice.Status == InvoiceStatus.Draft)
            {
                await _invoiceRepository.DeleteAsync(merchantId, invoiceId);

                IEnumerable<FileInfo> fileInfos = await _fileInfoRepository.GetAsync(invoiceId);

                foreach (FileInfo fileInfo in fileInfos)
                {
                    await _fileInfoRepository.DeleteAsync(invoiceId, fileInfo.Id);
                    await _fileRepository.DeleteAsync(fileInfo.Id);
                }

                await _log.WriteInfoAsync(nameof(InvoiceService), nameof(DeleteAsync),
                    invoice.ToContext().ToJson(), "Invoice deleted.");
            }
            else if (invoice.Status == InvoiceStatus.Unpaid)
            {
                await _invoiceRepository.SetStatusAsync(merchantId, invoiceId, InvoiceStatus.Removed);

                await _log.WriteInfoAsync(nameof(InvoiceService), nameof(DeleteAsync),
                    invoice.ToContext().ToJson(), "Invoice removed");
            }
            else
            {
                await _log.WriteInfoAsync(nameof(InvoiceService), nameof(DeleteAsync),
                    invoice.ToContext().ToJson(), "Cannot remove invoice");
            }
        }

        public async Task UpdateAsync(PaymentRequestDetailsMessage message)
        {
            Invoice invoice = await _invoiceRepository.FindByPaymentRequestIdAsync(message.Id);

            if (invoice == null)
                throw new InvoiceNotFoundException();

            InvoiceStatus status = StatusConverter.Convert(message.Status, message.ProcessingError);

            if (invoice.Status != status)
            {
                await _invoiceRepository.SetStatusAsync(invoice.MerchantId, invoice.Id, status);

                await _log.WriteInfoAsync(nameof(InvoiceService), nameof(UpdateAsync),
                    invoice.Id.ToContext(nameof(invoice.Id))
                        .ToContext(nameof(status), status)
                        .ToJson(),
                    "Status updated.");
            }
        }

        public async Task<InvoiceDetails> CheckoutAsync(string invoiceId)
        {
            Invoice invoice = await _invoiceRepository.FindByIdAsync(invoiceId);

            if (invoice == null)
                throw new InvoiceNotFoundException(invoiceId);

            if (invoice.DueDate < DateTime.UtcNow)
                throw new InvalidOperationException("Invoice expired.");

            if (invoice.Status == InvoiceStatus.Draft || invoice.Status == InvoiceStatus.Removed)
                throw new InvalidOperationException("Invoice status is invalid.");

            PaymentRequestModel paymentRequest =
                await _payInternalClient.GetPaymentRequestAsync(invoice.MerchantId, invoice.PaymentRequestId);

            if (paymentRequest == null)
                throw new InvalidOperationException("Payment request does not exist.");

            PaymentRequestDetailsModel paymentRequestDetails =
                await _payInternalClient.ChechoutAsync(invoice.MerchantId, invoice.PaymentRequestId);

            MerchantModel merchant =
                await _payInternalClient.GetMerchantByIdAsync(invoice.MerchantId);

            InvoiceStatus invoiceStatus = StatusConverter.Convert(paymentRequestDetails.Status, paymentRequestDetails.ProcessingError);

            if (invoice.Status != invoiceStatus)
            {
                await _invoiceRepository.SetStatusAsync(invoice.MerchantId, invoiceId, invoiceStatus);

                invoice = await _invoiceRepository.GetAsync(invoice.MerchantId, invoice.Id);
            }

            var invoiceDetails = Mapper.Map<InvoiceDetails>(invoice);

            invoiceDetails.WalletAddress = paymentRequest.WalletAddress;
            invoiceDetails.PaymentAmount = paymentRequestDetails.Order.PaymentAmount;
            invoiceDetails.OrderDueDate = paymentRequestDetails.Order.DueDate;
            invoiceDetails.OrderCreatedDate = paymentRequestDetails.Order.CreatedDate;
            invoiceDetails.DeltaSpread = merchant.DeltaSpread;
            invoiceDetails.MarkupPercent = paymentRequestDetails.MarkupPercent;
            invoiceDetails.ExchangeRate = paymentRequestDetails.Order.ExchangeRate;
            invoiceDetails.PaidAmount = (decimal)paymentRequestDetails.PaidAmount;
            invoiceDetails.PaidDate = paymentRequestDetails.PaidDate;

            return invoiceDetails;
        }

        private async Task<PaymentRequestModel> CreatePaymentRequestAsync(Invoice invoice)
        {
            return await _payInternalClient.CreatePaymentRequestAsync(
                new CreatePaymentRequestModel
                {
                    MerchantId = invoice.MerchantId,
                    Amount = invoice.Amount,
                    DueDate = invoice.DueDate,
                    MarkupPercent = 0,
                    MarkupPips = 0,
                    PaymentAssetId = invoice.PaymentAssetId,
                    SettlementAssetId = invoice.SettlementAssetId
                });
        }
    }
}