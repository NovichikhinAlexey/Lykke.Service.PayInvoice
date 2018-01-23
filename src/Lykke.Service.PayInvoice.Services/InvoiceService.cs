using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Log;
using Lykke.Service.PayInternal.Client;
using Lykke.Service.PayInternal.Client.Models;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Exceptions;
using Lykke.Service.PayInvoice.Core.Repositories;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Core.Utils;

namespace Lykke.Service.PayInvoice.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IInvoiceMerchantLinkRepository _invoiceMerchantLinkRepository;
        private readonly IFileInfoRepository _fileInfoRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IPayInternalClient _payInternalClient;
        private readonly ILog _log;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            IInvoiceMerchantLinkRepository invoiceMerchantLinkRepository,
            IFileInfoRepository fileInfoRepository,
            IFileRepository fileRepository,
            IPayInternalClient payInternalClient,
            ILog log)
        {
            _invoiceRepository = invoiceRepository;
            _invoiceMerchantLinkRepository = invoiceMerchantLinkRepository;
            _fileInfoRepository = fileInfoRepository;
            _fileRepository = fileRepository;
            _payInternalClient = payInternalClient;
            _log = log;
        }


        public async Task<IReadOnlyList<IInvoice>> GetAsync()
        {
            return await _invoiceRepository.GetAsync();
        }
        
        public async Task<IReadOnlyList<IInvoice>> GetAsync(string merchantId)
        {
            return await _invoiceRepository.GetAsync(merchantId);
        }

        public async Task<IInvoice> GetAsync(string merchantId, string invoiceId)
        {
            return await _invoiceRepository.GetAsync(merchantId, invoiceId);
        }

        public async Task<IInvoice> CreateDraftAsync(IInvoice invoice)
        {
            var draftInvoice = Mapper.Map<Invoice>(invoice);

            draftInvoice.Status = InvoiceStatus.Draft;
            draftInvoice.CreatedDate = DateTime.UtcNow;

            await _invoiceRepository.InsertAsync(draftInvoice);

            await _invoiceMerchantLinkRepository.InsertAsync(invoice.MerchantId, invoice.Id);
            
            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(CreateDraftAsync),
                ToContext(invoice), "Invoice draft created");

            return draftInvoice;
        }

        public async Task UpdateDraftAsync(IInvoice invoice)
        {
            IInvoice draftInvoice = await _invoiceRepository.GetAsync(invoice.MerchantId, invoice.Id);

            if (draftInvoice == null)
                throw new InvoiceNotFoundException(invoice.Id);

            if (draftInvoice.Status != InvoiceStatus.Draft)
                throw new InvalidOperationException("Invoice status is invalid.");

            Mapper.Map(invoice, draftInvoice);

            await _invoiceRepository.ReplaceAsync(draftInvoice);

            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(UpdateDraftAsync),
                ToContext(invoice), "Invoice draft updated");
        }

        public async Task<IInvoice> CreateAsync(IInvoice invoice)
        {
            var createdInvoice = Mapper.Map<Invoice>(invoice);

            createdInvoice.Status = InvoiceStatus.Unpaid;
            createdInvoice.CreatedDate = DateTime.UtcNow;

            await _invoiceRepository.InsertAsync(createdInvoice);
            await _invoiceMerchantLinkRepository.InsertAsync(invoice.MerchantId, invoice.Id);
            
            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(CreateAsync),
                ToContext(invoice), "Invoice created");

            return createdInvoice;
        }

        public async Task<IInvoice> CreateFromDraftAsync(IInvoice invoice)
        {
            IInvoice existingInvoice = await _invoiceRepository.GetAsync(invoice.MerchantId, invoice.Id);

            if(existingInvoice == null)
                throw new InvoiceNotFoundException(invoice.Id);

            if(existingInvoice.Status != InvoiceStatus.Draft)
                throw new InvalidOperationException("Invoice status is invalid.");

            var draftInvoice = Mapper.Map<Invoice>(existingInvoice);

            Mapper.Map(invoice, draftInvoice);
            
            draftInvoice.Status = InvoiceStatus.Unpaid;
            draftInvoice.CreatedDate = DateTime.UtcNow;

            await _invoiceRepository.ReplaceAsync(draftInvoice);

            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(CreateFromDraftAsync),
                ToContext(invoice), "Invoice created from draft");

            return draftInvoice;
        }

        public async Task SetStatus(string invoiceId, InvoiceStatus status)
        {
            string merchantId = await _invoiceMerchantLinkRepository.GetAsync(invoiceId);
            
            IInvoice invoice = await _invoiceRepository.GetAsync(merchantId, invoiceId);

            if (invoice == null)
                throw new InvoiceNotFoundException(invoiceId);

            await _invoiceRepository.SetStatusAsync(merchantId, invoiceId, status);

            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(SetStatus),
                invoiceId.ToContext(nameof(invoiceId))
                    .ToContext(nameof(status), status)
                    .ToJson(),
                "Status updated.");
        }

        public async Task DeleteAsync(string merchantId, string invoiceId)
        {
            IInvoice entity = await _invoiceRepository.GetAsync(merchantId, invoiceId);

            if (entity == null)
                return;

            var invoice = Mapper.Map<Invoice>(entity);

            if (invoice.Status == InvoiceStatus.Draft)
            {
                await _invoiceRepository.DeleteAsync(merchantId, invoiceId);
                await _invoiceMerchantLinkRepository.DeleteAsync(invoiceId);
                
                IEnumerable<IFileInfo> fileInfos = await _fileInfoRepository.GetAsync(invoiceId);

                foreach (IFileInfo fileInfo in fileInfos)
                {
                    await _fileInfoRepository.DeleteAsync(invoiceId, fileInfo.Id);
                    await _fileRepository.DeleteAsync(fileInfo.Id);
                }
                
                await _log.WriteInfoAsync(nameof(InvoiceService), nameof(DeleteAsync),
                    ToContext(invoice),
                    "Invoice deleted.");
            }
            else if (invoice.Status == InvoiceStatus.Unpaid)
            {
                await _invoiceRepository.SetStatusAsync(merchantId, invoiceId, InvoiceStatus.Removed);
                
                await _log.WriteInfoAsync(nameof(InvoiceService), nameof(DeleteAsync),
                    ToContext(invoice),
                    "Invoice removed");
            }
            else
            {
                await _log.WriteInfoAsync(nameof(InvoiceService), nameof(DeleteAsync),
                    ToContext(invoice),
                    "Cannot remove invoice");
            }
        }

        public async Task<IInvoiceDetails> GetDetailsAsync(string invoiceId)
        {
            string merchantId = await _invoiceMerchantLinkRepository.GetAsync(invoiceId);
            
            IInvoice invoice = await _invoiceRepository.GetAsync(merchantId, invoiceId);

            if (invoice == null)
                throw new InvoiceNotFoundException(invoiceId);
            
            if(invoice.DueDate < DateTime.UtcNow)
                throw new InvalidOperationException("Invoice expired.");

            if (invoice.Status == InvoiceStatus.Draft || invoice.Status == InvoiceStatus.Removed)
                throw new InvalidOperationException("Invoice status is invalid.");

            CreateOrderResponse response;
            
            if (string.IsNullOrEmpty(invoice.WalletAddress))
            {
                response = await _payInternalClient.CreateOrderAsync(new CreateOrderRequest
                {
                    MerchantId = invoice.MerchantId,
                    AssetPairId = invoice.AssetPairId,
                    InvoiceAssetId = invoice.AssetId,
                    ExchangeAssetId = invoice.ExchangeAssetId,
                    InvoiceAmount = invoice.Amount,
                    MarkupPercent = 0,
                    MarkupPips = 0,
                    WalletDueDate = invoice.DueDate
                });
            }
            else
            {
                response = await _payInternalClient.ReCreateOrderAsync(new ReCreateOrderRequest
                {
                    WalletAddress = invoice.WalletAddress
                });
            }

            var invoiceDetails = Mapper.Map<InvoiceDetails>(invoice);

            invoiceDetails.ExchangeAmount = response.ExchangeAmount;
            invoiceDetails.OrderId = response.OrderId;
            invoiceDetails.OrderDueDate = response.DueDate;

            return invoiceDetails;
        }
        
        private string ToContext(IInvoice invoice)
        {
            var copy = Mapper.Map<Invoice>(invoice);
            copy.ClientEmail = copy.ClientEmail.SanitizeEmail();
            return copy.ToJson();
        }
    }
}