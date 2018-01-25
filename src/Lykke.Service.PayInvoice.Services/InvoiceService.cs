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

            IInvoice createdInvoice = await _invoiceRepository.InsertAsync(draftInvoice);

            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(CreateDraftAsync),
                invoice.ToContext().ToJson(), "Invoice draft created");

            return createdInvoice;
        }

        public async Task UpdateDraftAsync(IInvoice invoice)
        {
            IInvoice existingInvoice = await _invoiceRepository.GetAsync(invoice.MerchantId, invoice.Id);

            if (existingInvoice == null)
                throw new InvoiceNotFoundException(invoice.Id);

            if (existingInvoice.Status != InvoiceStatus.Draft)
                throw new InvalidOperationException("Invoice status is invalid.");

            Invoice draftInvoice = Mapper.Map<Invoice>(existingInvoice);
            Mapper.Map(invoice, draftInvoice);

            draftInvoice.Status = InvoiceStatus.Draft;
            draftInvoice.CreatedDate = existingInvoice.CreatedDate;

            await _invoiceRepository.ReplaceAsync(draftInvoice);

            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(UpdateDraftAsync),
                invoice.ToContext().ToJson(), "Invoice draft updated");
        }

        public async Task<IInvoice> CreateAsync(IInvoice invoice)
        {
            var newInvoice = Mapper.Map<Invoice>(invoice);

            newInvoice.Status = InvoiceStatus.Unpaid;
            newInvoice.CreatedDate = DateTime.UtcNow;

            IInvoice createdInvoice = await _invoiceRepository.InsertAsync(newInvoice);
            
            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(CreateAsync),
                invoice.ToContext().ToJson(), "Invoice created");

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
                invoice.ToContext().ToJson(), "Invoice created from draft");

            return draftInvoice;
        }

        public async Task SetStatus(string invoiceId, InvoiceStatus status)
        {
            IInvoice invoice = await _invoiceRepository.FindAsync(invoiceId);

            if (invoice == null)
                throw new InvoiceNotFoundException(invoiceId);

            await _invoiceRepository.SetStatusAsync(invoice.MerchantId, invoiceId, status);

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
                
                IEnumerable<IFileInfo> fileInfos = await _fileInfoRepository.GetAsync(invoiceId);

                foreach (IFileInfo fileInfo in fileInfos)
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

        public async Task<IInvoiceDetails> GetDetailsAsync(string invoiceId)
        {
            IInvoice invoice = await _invoiceRepository.FindAsync(invoiceId);

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
                    InvoiceAssetId = invoice.AssetId,
                    ExchangeAssetId = invoice.ExchangeAssetId,
                    InvoiceAmount = invoice.Amount,
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
    }
}