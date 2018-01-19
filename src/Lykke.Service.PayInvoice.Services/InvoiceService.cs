using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Pay.Common;
using Lykke.Service.PayInvoice.Core.Clients;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Exceptions;
using Lykke.Service.PayInvoice.Core.Repositories;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Core.Utils;
using Lykke.Service.PayInvoice.Services.ext;

namespace Lykke.Service.PayInvoice.Services
{
    [UsedImplicitly]
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IFileInfoRepository _fileInfoRepository;
        private readonly IFileRepository _fileRepository;
        private readonly ILykkePayServiceClient _lykkePayServiceClient;
        private readonly ICallbackUrlFormatter _callbackUrlFormatter;
        private readonly ILog _log;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            IFileInfoRepository fileInfoRepository,
            IFileRepository fileRepository,
            ILykkePayServiceClient lykkePayServiceClient,
            ICallbackUrlFormatter callbackUrlFormatter,
            ILog log)
        {
            _invoiceRepository = invoiceRepository;
            _fileInfoRepository = fileInfoRepository;
            _fileRepository = fileRepository;
            _lykkePayServiceClient = lykkePayServiceClient;
            _callbackUrlFormatter = callbackUrlFormatter;
            _log = log;
        }

        public async Task<IInvoice> GetAsync(string merchantId, string invoiceId)
        {
            return await _invoiceRepository.GetAsync(merchantId, invoiceId);
        }

        public async Task<IEnumerable<IInvoice>> GetAllAsync()
        {
            return await _invoiceRepository.GetAsync();
        }

        public async Task<IEnumerable<IInvoice>> GetByMerchantIdAsync(string merchantId)
        {
            return await _invoiceRepository.GetByMerchantIdAsync(merchantId);
        }

        public async Task<IInvoice> GetByAddressAsync(string address)
        {
            return await _invoiceRepository.GetByAddressAsync(address);
        }

        public async Task<IInvoice> CreateDraftAsync(IInvoice invoice)
        {
            var draftInvoice = Mapper.Map<Invoice>(invoice);

            draftInvoice.Status = InvoiceStatus.Draft.ToString();
            draftInvoice.InvoiceId = Guid.NewGuid().ToString("D");
            draftInvoice.StartDate = DateTime.Now.RepoDateStr();

            await _invoiceRepository.InsertAsync(draftInvoice);

            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(CreateDraftAsync),
                invoice.ToContext().ToJson(), "Invoice draft created");

            return draftInvoice;
        }

        public async Task UpdateDraftAsync(IInvoice invoice)
        {
            IInvoice existingInvoice = await _invoiceRepository.GetAsync(invoice.MerchantId, invoice.InvoiceId);

            if (existingInvoice == null)
                throw new InvoiceNotFoundException(invoice.InvoiceId);

            if (existingInvoice.Status != InvoiceStatus.Draft.ToString())
                throw new InvalidOperationException("Invoice status is invalid.");

            var draftInvoice = Mapper.Map<Invoice>(invoice);

            draftInvoice.Status = InvoiceStatus.Draft.ToString();
            draftInvoice.StartDate = DateTime.Now.RepoDateStr();

            await _invoiceRepository.UpdateAsync(draftInvoice);

            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(UpdateDraftAsync),
                invoice.ToContext().ToJson(), "Invoce draft updated");
        }

        public async Task<IInvoice> GenerateAsync(IInvoice invoice)
        {
            var generatedInvoice = Mapper.Map<Invoice>(invoice);

            generatedInvoice.InvoiceId = Guid.NewGuid().ToString("D");

            string walletAddress = await CreateOrder(generatedInvoice);

            generatedInvoice.Status = InvoiceStatus.Unpaid.ToString();
            generatedInvoice.StartDate = DateTime.Now.RepoDateStr();
            generatedInvoice.WalletAddress = walletAddress;

            await _invoiceRepository.InsertAsync(generatedInvoice);

            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(GenerateAsync),
                invoice.ToContext().ToJson(), "Invoice generated");

            return generatedInvoice;
        }

        public async Task<IInvoice> GenerateFromDraftAsync(IInvoice invoice)
        {
            IInvoice existingInvoice = await _invoiceRepository.GetAsync(invoice.MerchantId, invoice.InvoiceId);

            if(existingInvoice == null)
                throw new InvoiceNotFoundException(invoice.InvoiceId);

            if(existingInvoice.Status != InvoiceStatus.Draft.ToString())
                throw new InvalidOperationException("Invoice status is invalid.");

            var generatedInvoice = Mapper.Map<Invoice>(invoice);

            string walletAddress = await CreateOrder(invoice);

            generatedInvoice.Status = InvoiceStatus.Unpaid.ToString();
            generatedInvoice.StartDate = DateTime.Now.RepoDateStr();
            generatedInvoice.WalletAddress = walletAddress;

            await _invoiceRepository.InsertAsync(generatedInvoice);

            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(GenerateFromDraftAsync),
                generatedInvoice.ToContext().ToJson(), "Invoice generated from draft");

            return generatedInvoice;
        }

        public async Task UpdateStatus(string invoiceId, InvoiceStatus status)
        {
            IInvoice entity = await _invoiceRepository.GetAsync(invoiceId);

            if (entity == null)
                throw new InvoiceNotFoundException(invoiceId);

            var invoice = Mapper.Map<Invoice>(entity);

            invoice.Status = status.ToString();

            await _invoiceRepository.UpdateAsync(invoice);

            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(UpdateStatus), invoiceId,
                $"Status updated to {status}");
        }

        public async Task DeleteAsync(string merchantId, string invoiceId)
        {
            IInvoice entity = await _invoiceRepository.GetAsync(merchantId, invoiceId);

            if (entity == null)
                return;

            var invoice = Mapper.Map<Invoice>(entity);

            var invoiceStatus = invoice.Status.ParsePayEnum<InvoiceStatus>();

            if (invoiceStatus == InvoiceStatus.Draft)
            {
                await _invoiceRepository.DeleteAsync(merchantId, invoiceId);

                IEnumerable<IFileInfo> fileInfos = await _fileInfoRepository.GetAsync(invoiceId);

                foreach (IFileInfo fileInfo in fileInfos)
                {
                    await _fileInfoRepository.DeleteAsync(invoiceId, fileInfo.FileId);
                    await _fileRepository.DeleteAsync(fileInfo.FileId);
                }

                await _log.WriteInfoAsync(nameof(InvoiceService), nameof(DeleteAsync),
                    invoice.ToContext().ToJson(),
                    $"Delete invoce {invoice.InvoiceId}, for merchant {invoice.MerchantId} with status {invoice.Status}");
            }
            else if (invoiceStatus == InvoiceStatus.Unpaid)
            {

                invoice.Status = InvoiceStatus.Removed.ToString();
                await _invoiceRepository.UpdateAsync(invoice);
                await _log.WriteInfoAsync(nameof(InvoiceService), nameof(DeleteAsync),
                    invoice.ToContext().ToJson(),
                    $"Delete invoce {invoice.InvoiceId}, for merchant {invoice.MerchantId} with status {InvoiceStatus.Unpaid}");
            }
            else
            {
                await _log.WriteInfoAsync(nameof(InvoiceService), nameof(DeleteAsync), invoice.ToContext().ToJson(),
                    $"CANNOT delete invoce {invoice.InvoiceId}, for merchant {invoice.MerchantId} in current status");
            }
        }

        public async Task<Tuple<IInvoice, IOrder>> GetOrderDetails(string invoiceId)
        {
            IInvoice invoice = await _invoiceRepository.GetAsync(invoiceId);

            if(invoice.DueDate.GetRepoDateTime() < DateTime.Now)
                throw new InvalidOperationException("Invoice expired.");

            if (invoice.Status == InvoiceStatus.Draft.ToString() || invoice.Status == InvoiceStatus.Removed.ToString())
                throw new InvalidOperationException("Invoice status is invalid.");

            Core.Clients.OrderResponse response =
                await _lykkePayServiceClient.ReCreateOrderAsync(invoice.WalletAddress, invoice.MerchantId);

            await _log.WriteInfoAsync(nameof(InvoiceService), nameof(GetOrderDetails),
                invoiceId, "Order re-created");

            return new Tuple<IInvoice, IOrder>(invoice, new Order
            {
                OrderId = response.OrderId,
                Currency = response.Currency,
                Amount = response.Amount,
                ExchangeRate = response.ExchangeRate,
                TotalAmount = response.TotalAmount,
                // Seconds from 01.01.1970
                TransactionWaitingTime = response.TransactionWaitingTime
            });
        }

        private async Task<string> CreateOrder(IInvoice invoice)
        {
            try
            {
                var orderRequest = new OrderRequest
                {
                    Currency = invoice.Currency,
                    Amount = invoice.Amount.ToString(CultureInfo.InvariantCulture),
                    ExchangeCurrency = "BTC",
                    OrderId = invoice.InvoiceNumber,
                    Markup = new Markup //TODO Needs to set Merchant Markup here
                    {
                        Percent = 0,
                        Pips = 0
                    },
                    SuccessUrl = _callbackUrlFormatter.GetSuccessUrl(invoice.InvoiceId),
                    ErrorUrl = _callbackUrlFormatter.GetErrorUrl(invoice.InvoiceId),
                    ProgressUrl = _callbackUrlFormatter.GetProgressUrl(invoice.InvoiceId)
                };

                Core.Clients.OrderResponse response =
                    await _lykkePayServiceClient.CreateOrderAsync(orderRequest, invoice.MerchantId);

                await _log.WriteInfoAsync(nameof(InvoiceService), nameof(CreateOrder),
                    invoice.InvoiceId, "Order created.");

                return response.Address;
            }
            catch (Exception exception)
            {
                await _log.WriteErrorAsync(nameof(InvoiceService), nameof(CreateOrder),
                    invoice.InvoiceId, exception);
                throw;
            }
        }
    }
}