using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Log;
using Lykke.Service.PayInternal.Client;
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
        private readonly IHistoryRepository _historyRepository;
        private readonly IPayInternalClient _payInternalClient;
        private readonly ILog _log;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            IFileInfoRepository fileInfoRepository,
            IFileRepository fileRepository,
            IHistoryRepository historyRepository,
            IPayInternalClient payInternalClient,
            ILog log)
        {
            _invoiceRepository = invoiceRepository;
            _fileInfoRepository = fileInfoRepository;
            _fileRepository = fileRepository;
            _historyRepository = historyRepository;
            _payInternalClient = payInternalClient;
            _log = log.CreateComponentScope(nameof(InvoiceService));
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

        public Task<IReadOnlyList<HistoryItem>> GetHistoryAsync(string invoiceId)
        {
            return _historyRepository.GetByInvoiceIdAsync(invoiceId);
        }

        public async Task<Invoice> CreateDraftAsync(Invoice invoice)
        {
            invoice.Status = InvoiceStatus.Draft;
            invoice.CreatedDate = DateTime.UtcNow;

            Invoice createdInvoice = await _invoiceRepository.InsertAsync(invoice);

            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(CreateDraftAsync),
                invoice.ToContext().ToJson(), "Invoice draft created");

            await WriteHistory(createdInvoice, "Invoice draft created");

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

            await WriteHistory(invoice, "Invoice draft updated");
        }

        public async Task<Invoice> CreateAsync(Invoice invoice)
        {
            PaymentRequestModel paymentRequest = await CreatePaymentRequestAsync(invoice);

            invoice.Status = InvoiceStatus.Unpaid;
            invoice.CreatedDate = DateTime.UtcNow;
            invoice.PaymentRequestId = paymentRequest.Id;

            Invoice createdInvoice = await _invoiceRepository.InsertAsync(invoice);

            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(CreateAsync),
                invoice.ToContext().ToJson(), "Invoice created");

            await WriteHistory(createdInvoice, "Invoice created");

            return createdInvoice;
        }

        public async Task<Invoice> CreateFromDraftAsync(string invoiceId)
        {
            Invoice invoice = await _invoiceRepository.FindByIdAsync(invoiceId);

            if (invoice == null)
                throw new InvoiceNotFoundException(invoiceId);

            if (invoice.Status != InvoiceStatus.Draft)
                throw new InvalidOperationException("Invoice status is invalid.");

            PaymentRequestModel paymentRequest = await CreatePaymentRequestAsync(invoice);

            invoice.Status = InvoiceStatus.Unpaid;
            invoice.PaymentRequestId = paymentRequest.Id;
            
            await _invoiceRepository.UpdateAsync(invoice);

            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(CreateFromDraftAsync),
                invoice.ToContext().ToJson(), "Invoice created from draft");

            await WriteHistory(invoice, "Invoice created from draft");

            return invoice;
        }

        public async Task DeleteAsync(string invoiceId)
        {
            Invoice invoice = await _invoiceRepository.FindByIdAsync(invoiceId);

            if (invoice == null)
                return;

            if (invoice.Status == InvoiceStatus.Draft)
            {
                await _invoiceRepository.DeleteAsync(invoice.MerchantId, invoice.Id);

                IEnumerable<FileInfo> fileInfos = await _fileInfoRepository.GetAsync(invoiceId);

                foreach (FileInfo fileInfo in fileInfos)
                {
                    await _fileInfoRepository.DeleteAsync(invoiceId, fileInfo.Id);
                    await _fileRepository.DeleteAsync(fileInfo.Id);
                }

                await _log.WriteInfoAsync(nameof(InvoiceService), nameof(DeleteAsync),
                    invoice.ToContext().ToJson(), "Invoice deleted.");

                await _historyRepository.DeleteAsync(invoiceId);
            }
            else if (invoice.Status == InvoiceStatus.Unpaid)
            {
                try
                {
                    await _payInternalClient.CancelAsync(invoice.MerchantId, invoice.PaymentRequestId);
                }
                catch (Exception ex)
                {
                    const string message = "PaymentRequest is not cancelled";
                    _log.WriteError(nameof(DeleteAsync), new {message, invoice}, ex);
                    throw new InvalidOperationException(message, ex);
                }
                
                await _invoiceRepository.SetStatusAsync(invoice.MerchantId, invoice.Id, InvoiceStatus.Removed);

                await _log.WriteInfoAsync(nameof(InvoiceService), nameof(DeleteAsync),
                    invoice.ToContext().ToJson(), "Invoice removed");

                invoice.Status = InvoiceStatus.Removed;

                await WriteHistory(invoice, "Invoice removed");
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

            if (invoice.Status == status)
                return;

            await _invoiceRepository.SetStatusAsync(invoice.MerchantId, invoice.Id, status);

            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(UpdateAsync),
                invoice.Id.ToContext(nameof(invoice.Id))
                    .ToContext(nameof(status), status)
                    .ToJson(), "Status updated.");

            invoice.Status = status;

            var history = Mapper.Map<HistoryItem>(invoice);
            history.WalletAddress = message.WalletAddress;
            history.PaidAmount = message.PaidAmount;
            history.PaymentAmount = message.Order.PaymentAmount;
            history.ExchangeRate = message.Order.ExchangeRate;
            history.SourceWalletAddresses = message.Transactions
                .SelectMany(o => o.SourceWalletAddresses)
                .Distinct()
                .ToList();
            history.RefundWalletAddress = message.Refund?.Address;
            history.RefundAmount = message.Refund?.Amount ?? decimal.Zero;
            history.PaidDate = message.PaidDate;
            history.Reason = "Payment request updated";
            history.Date = DateTime.UtcNow;

            await _historyRepository.InsertAsync(history);
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

        private async Task WriteHistory(Invoice invoice, string reason)
        {
            var history = Mapper.Map<HistoryItem>(invoice);
            history.Reason = reason;
            history.Date = DateTime.UtcNow;
            
            await _historyRepository.InsertAsync(history);
        }
    }
}