using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Log;
using Lykke.Service.PayInternal.Client;
using Lykke.Service.PayInternal.Client.Exceptions;
using Lykke.Service.PayInternal.Client.Models.PaymentRequest;
using Lykke.Service.PayInternal.Contract.PaymentRequest;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Domain.PaymentRequest;
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
        private readonly IPaymentRequestHistoryRepository _paymentRequestHistoryRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IInvoiceDisputeRepository _invoiceDisputeRepository;
        private readonly IPayInternalClient _payInternalClient;
        private readonly ILog _log;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            IFileInfoRepository fileInfoRepository,
            IFileRepository fileRepository,
            IHistoryRepository historyRepository,
            IPaymentRequestHistoryRepository paymentRequestHistoryRepository,
            IEmployeeRepository employeeRepository,
            IInvoiceDisputeRepository invoiceDisputeRepository,
            IPayInternalClient payInternalClient,
            ILog log)
        {
            _invoiceRepository = invoiceRepository;
            _fileInfoRepository = fileInfoRepository;
            _fileRepository = fileRepository;
            _historyRepository = historyRepository;
            _paymentRequestHistoryRepository = paymentRequestHistoryRepository;
            _employeeRepository = employeeRepository;
            _invoiceDisputeRepository = invoiceDisputeRepository;
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

        public async Task<IReadOnlyList<Invoice>> GetByFilterAsync(string merchantId)
        {
            return await _invoiceRepository.GetAsync(merchantId);
        }

        public async Task<IReadOnlyList<Invoice>> GetByFilterAsync(InvoiceFilter invoiceFilter)
        {
            var filtered = await _invoiceRepository.GetByFilterAsync(invoiceFilter);

            if (invoiceFilter.GreaterThan.HasValue)
            {
                filtered = filtered.Where(x => x.Amount >= invoiceFilter.GreaterThan.Value).ToList();
            }

            if (invoiceFilter.LessThan.HasValue)
            {
                filtered = filtered.Where(x => x.Amount <= invoiceFilter.LessThan.Value).ToList();
            }

            return filtered;
        }

        public Task<IReadOnlyList<HistoryItem>> GetHistoryAsync(string invoiceId)
        {
            return _historyRepository.GetByInvoiceIdAsync(invoiceId);
        }

        public async Task<IReadOnlyList<PaymentRequestHistoryItem>> GetPaymentRequestsOfInvoiceAsync(string invoiceId)
        {
            var result = new List<PaymentRequestHistoryItem>();

            var history = await _paymentRequestHistoryRepository.GetByInvoiceIdAsync(invoiceId);
            result.AddRange(history.OrderBy(x => x.CreatedAt));

            var invoice = await _invoiceRepository.FindByIdAsync(invoiceId);

            if (invoice == null)
                throw new InvoiceNotFoundException(invoiceId);

            PaymentRequestModel paymentRequest = await _payInternalClient.GetPaymentRequestAsync(invoice.MerchantId, invoice.PaymentRequestId);

            // Add current payment request
            var currentPaymentRequest = new PaymentRequestHistoryItem
            {
                InvoiceId = invoice.Id,
                PaymentRequestId = invoice.PaymentRequestId,
                PaymentAssetId = invoice.PaymentAssetId,
                CreatedAt = paymentRequest.Timestamp ?? DateTime.UtcNow
            };
            result.Add(currentPaymentRequest);

            return result;
        }

        public async Task<Invoice> CreateDraftAsync(Invoice invoice)
        {
            invoice.Status = InvoiceStatus.Draft;
            invoice.CreatedDate = DateTime.UtcNow;

            Invoice createdInvoice = await _invoiceRepository.InsertAsync(invoice);

            _log.WriteInfo(nameof(CreateDraftAsync), invoice.Sanitize(), "Invoice draft created");

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

            _log.WriteInfo(nameof(UpdateDraftAsync), invoice.Sanitize(), "Invoice draft updated");

            await WriteHistory(invoice, "Invoice draft updated");
        }

        public async Task<Invoice> CreateAsync(Invoice invoice)
        {
            PaymentRequestModel paymentRequest = await CreatePaymentRequestAsync(invoice);

            invoice.Status = InvoiceStatus.Unpaid;
            invoice.CreatedDate = DateTime.UtcNow;
            invoice.PaymentRequestId = paymentRequest.Id;

            Invoice createdInvoice = await _invoiceRepository.InsertAsync(invoice);

            _log.WriteInfo(nameof(CreateAsync), invoice.Sanitize(), "Invoice created");

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

            _log.WriteInfo(nameof(CreateFromDraftAsync), invoice.Sanitize(), "Invoice created from draft");

            await WriteHistory(invoice, "Invoice created from draft");

            return invoice;
        }

        public async Task<Invoice> ChangePaymentRequestAsync(string invoiceId, string paymentAssetId)
        {
            Invoice invoice = await _invoiceRepository.FindByIdAsync(invoiceId);

            if (invoice == null)
                throw new InvoiceNotFoundException(invoiceId);

            if (invoice.Status != InvoiceStatus.Unpaid)
                throw new InvalidOperationException("Invoice status is invalid.");

            if (invoice.PaymentAssetId == paymentAssetId)
                return invoice;

            var previousPaymentAssetId = invoice.PaymentAssetId;
            invoice.PaymentAssetId = paymentAssetId;

            PaymentRequestModel paymentRequest = await CreatePaymentRequestAsync(invoice);

            var previousPaymentRequestId = invoice.PaymentRequestId;
            invoice.PaymentRequestId = paymentRequest.Id;

            await _invoiceRepository.UpdateAsync(invoice);

            await WritePaymentRequestHistory(invoice.MerchantId, invoice.Id, previousPaymentRequestId, previousPaymentAssetId);

            await CancelPaymentRequestAsync(invoice.MerchantId, previousPaymentRequestId);

            _log.WriteInfo(nameof(ChangePaymentRequestAsync), 
                new { invoice = invoice.Sanitize(), previousPaymentRequestId, newPaymentRequest = paymentRequest }, 
                "Payment request changed");

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

                _log.WriteInfo(nameof(DeleteAsync), invoice.Sanitize(), "Invoice deleted.");

                await _historyRepository.DeleteAsync(invoiceId);
            }
            else if (invoice.Status == InvoiceStatus.Unpaid)
            {
                await CancelPaymentRequestAsync(invoice.MerchantId, invoice.PaymentRequestId);

                await _invoiceRepository.SetStatusAsync(invoice.MerchantId, invoice.Id, InvoiceStatus.Removed);

                _log.WriteInfo(nameof(DeleteAsync), invoice.Sanitize(), "Invoice removed");

                invoice.Status = InvoiceStatus.Removed;

                await WriteHistory(invoice, "Invoice removed");
            }
            else
            {
                _log.WriteInfo(nameof(DeleteAsync), invoice.Sanitize(), "Cannot remove invoice");
            }
        }

        public async Task UpdateAsync(PaymentRequestDetailsMessage message)
        {
            var paymentRequestId = message.Id;
            Invoice invoice = await _invoiceRepository.FindByPaymentRequestIdAsync(paymentRequestId);

            if (invoice == null)
                throw new InvoiceNotFoundException();

            if (message.Status == PayInternal.Contract.PaymentRequest.PaymentRequestStatus.Cancelled)
            {
                var paymentRequestHistoryItem = _paymentRequestHistoryRepository.GetAsync(invoice.Id, paymentRequestId);

                if (paymentRequestHistoryItem != null)
                    return;
            }

            InvoiceStatus status = StatusConverter.Convert(message.Status, message.ProcessingError);

            if (invoice.Status == status)
                return;

            await _invoiceRepository.SetStatusAsync(invoice.MerchantId, invoice.Id, status);

            if (new List<InvoiceStatus>() {
                    InvoiceStatus.Paid,
                    InvoiceStatus.Overpaid,
                    InvoiceStatus.Underpaid,
                    InvoiceStatus.LatePaid }
                .Contains(status))
            {
                await _invoiceRepository.SetPaidAmountAsync(invoice.MerchantId, invoice.Id, message.PaidAmount);
            }

            _log.WriteInfo(nameof(UpdateAsync), new { invoice.Id,  status }, "Status updated.");
            
            invoice.Status = status;

            var history = Mapper.Map<HistoryItem>(invoice);
            history.WalletAddress = message.WalletAddress;
            history.PaidAmount = message.PaidAmount;
            history.PaymentAmount = message.Order?.PaymentAmount ?? decimal.Zero;
            history.ExchangeRate = message.Order?.ExchangeRate ?? decimal.Zero;
            history.SourceWalletAddresses = message.Transactions?
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

        private async Task CancelPaymentRequestAsync(string merchantId, string paymentRequestId)
        {
            try
            {
                var paymentRequest = await _payInternalClient.GetPaymentRequestAsync(merchantId, paymentRequestId);
                if (paymentRequest.Status != PayInternal.Client.Models.PaymentRequest.PaymentRequestStatus.Cancelled)
                {
                    await _payInternalClient.CancelAsync(merchantId, paymentRequestId);
                }
            }
            catch (Exception ex)
            {
                const string message = "PaymentRequest is not cancelled";
                _log.WriteError(nameof(CancelPaymentRequestAsync), new { message, merchantId, paymentRequestId }, ex);
                throw new InvalidOperationException(message, ex);
            }
        }

        private async Task<PaymentRequestModel> CreatePaymentRequestAsync(Invoice invoice)
        {
            try
            {
                return await _payInternalClient.CreatePaymentRequestAsync(
                    new CreatePaymentRequestModel
                    {
                        MerchantId = invoice.MerchantId,
                        Amount = invoice.Amount,
                        DueDate = invoice.DueDate,
                        PaymentAssetId = invoice.PaymentAssetId,
                        SettlementAssetId = invoice.SettlementAssetId
                    });
            }
            catch (DefaultErrorResponseException ex)
            {
                throw new InvalidOperationException($"{ex.StatusCode}: {ex.Error.ErrorMessage}", ex);
            }
        }

        private async Task WriteHistory(Invoice invoice, string reason)
        {
            var history = Mapper.Map<HistoryItem>(invoice);
            history.Reason = reason;
            history.Date = DateTime.UtcNow;
            
            await _historyRepository.InsertAsync(history);
        }

        private async Task WritePaymentRequestHistory(string merchantId, string invoiceId, string paymentRequestId, string paymentAssetId)
        {
            PaymentRequestModel paymentRequest = await _payInternalClient.GetPaymentRequestAsync(merchantId, paymentRequestId);

            var history = new PaymentRequestHistoryItem
            {
                InvoiceId = invoiceId,
                PaymentRequestId = paymentRequestId,
                PaymentAssetId = paymentAssetId,
                CreatedAt = paymentRequest.Timestamp ?? DateTime.UtcNow
            };

            await _paymentRequestHistoryRepository.InsertAsync(history);
        }

        public async Task MarkDisputeAsync(string invoiceId, string reason, string employeeId)
        {
            await ValidateDisputeActions(invoiceId, employeeId, isMarkAction: true);

            await _invoiceRepository.MarkDisputeAsync(invoiceId);

            await _invoiceDisputeRepository.InsertAsync(new InvoiceDispute
            {
                InvoiceId = invoiceId,
                Reason = reason,
                EmployeeId = employeeId,
                CreatedAt = DateTime.UtcNow
            });
        }

        public async Task CancelDisputeAsync(string invoiceId, string employeeId)
        {
            await ValidateDisputeActions(invoiceId, employeeId, isMarkAction: false);

            await _invoiceRepository.CancelDisputeAsync(invoiceId);
        }

        public async Task<InvoiceDispute> GetInvoiceDisputeAsync(string invoiceId)
        {
            var invoiceDispute = await _invoiceDisputeRepository.GetByInvoiceIdAsync(invoiceId);

            if (invoiceDispute == null)
                throw new InvoiceDisputeNotFoundException(invoiceId);

            return invoiceDispute;
        }

        private async Task ValidateDisputeActions(string invoiceId, string employeeId, bool isMarkAction)
        {
            Invoice invoice = await _invoiceRepository.FindByIdAsync(invoiceId);

            if (invoice == null)
                throw new InvoiceNotFoundException(invoiceId);

            if (isMarkAction && invoice.Status != InvoiceStatus.Unpaid)
                throw new InvalidOperationException("Invoice status is invalid.");

            Employee employee = await _employeeRepository.GetByIdAsync(employeeId);

            if (employee == null)
                throw new EmployeeNotFoundException(employeeId);

            if (invoice.ClientName != employee.MerchantId)
                throw new InvalidOperationException("Only counterparty can mark or cancel an invoice as Dispute");
        }
    }
}
