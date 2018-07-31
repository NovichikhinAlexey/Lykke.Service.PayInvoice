using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.PayInternal.Client;
using Lykke.Service.PayInternal.Client.Exceptions;
using Lykke.Service.PayInternal.Client.Models.Order;
using Lykke.Service.PayInternal.Client.Models.PaymentRequest;
using Lykke.Service.PayInternal.Contract.PaymentRequest;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Domain.HistoryOperation;
using Lykke.Service.PayInvoice.Core.Domain.InvoiceConfirmation;
using Lykke.Service.PayInvoice.Core.Domain.InvoicePayerHistory;
using Lykke.Service.PayInvoice.Core.Domain.PaymentRequest;
using Lykke.Service.PayInvoice.Core.Domain.PushNotification;
using Lykke.Service.PayInvoice.Core.Exceptions;
using Lykke.Service.PayInvoice.Core.Extensions;
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
        private readonly IMerchantService _merchantService;
        private readonly IMerchantSettingService _merchantSettingService;
        private readonly IHistoryOperationService _historyOperationService;
        private readonly IInvoiceConfirmationService _invoiceConfirmationService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IDistributedLocksService _paymentLocksService;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IInvoiceDisputeRepository _invoiceDisputeRepository;
        private readonly IInvoicePayerHistoryRepository _invoicePayerHistoryRepository;
        private readonly IPayInternalClient _payInternalClient;
        private readonly ILog _log;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            IFileInfoRepository fileInfoRepository,
            IFileRepository fileRepository,
            IHistoryRepository historyRepository,
            IPaymentRequestHistoryRepository paymentRequestHistoryRepository,
            IMerchantService merchantService,
            IMerchantSettingService merchantSettingService,
            IHistoryOperationService historyOperationService,
            IInvoiceConfirmationService invoiceConfirmationService,
            IPushNotificationService pushNotificationService,
            IDistributedLocksService paymentLocksService,
            IEmployeeRepository employeeRepository,
            IInvoiceDisputeRepository invoiceDisputeRepository,
            IInvoicePayerHistoryRepository invoicePayerHistoryRepository,
            IPayInternalClient payInternalClient,
            ILogFactory logFactory)
        {
            _invoiceRepository = invoiceRepository;
            _fileInfoRepository = fileInfoRepository;
            _fileRepository = fileRepository;
            _historyRepository = historyRepository;
            _paymentRequestHistoryRepository = paymentRequestHistoryRepository;
            _merchantService = merchantService;
            _merchantSettingService = merchantSettingService;
            _historyOperationService = historyOperationService;
            _invoiceConfirmationService = invoiceConfirmationService;
            _pushNotificationService = pushNotificationService;
            _paymentLocksService = paymentLocksService;
            _employeeRepository = employeeRepository;
            _invoiceDisputeRepository = invoiceDisputeRepository;
            _invoicePayerHistoryRepository = invoicePayerHistoryRepository;
            _payInternalClient = payInternalClient;
            _log = logFactory.CreateLog(this);
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
            var invoice = await _invoiceRepository.FindByIdAsync(invoiceId);

            if (invoice == null)
                throw new InvoiceNotFoundException(invoiceId);

            return invoice;
        }

        public async Task<IReadOnlyList<Invoice>> GetByIdsAsync(string merchantId, IEnumerable<string> invoiceIds)
        {
            return await _invoiceRepository.GetByIdsAsync(merchantId, invoiceIds);
        }

        public async Task<IReadOnlyList<Invoice>> GetByIdsAsync(IEnumerable<string> invoiceIds)
        {
            var result = new List<Invoice>();

            foreach (var invoiceId in invoiceIds)
            {
                var invoice = await _invoiceRepository.FindByIdAsync(invoiceId);

                if (invoice == null)
                    throw new InvoiceNotFoundException(invoiceId, $"Invoice {invoiceId} not found");

                result.Add(invoice);
            }

            return result;
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

            FillLeftAmountToPayForUnderpaid(filtered);

            return filtered;
        }

        private void FillLeftAmountToPayForUnderpaid(IReadOnlyList<Invoice> invoices)
        {
            foreach (var invoice in invoices)
            {
                if (invoice.Status == InvoiceStatus.Underpaid && invoice.TotalPaidAmountInSettlementAsset.HasValue)
                {
                    // TODO: for future - if there will be multiple paid payment requests with at least one SettlementAssetId != PaymentAssetId  
                    // then rounding is required to accuracy of SettlementAssetId for calculated total
                    invoice.LeftAmountToPayInSettlementAsset = invoice.Amount - invoice.TotalPaidAmountInSettlementAsset.Value;
                }
            }
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

            _log.InfoWithDetails("Invoice draft created", invoice.Sanitize());

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

            _log.InfoWithDetails("Invoice draft updated", invoice.Sanitize());

            await WriteHistory(invoice, "Invoice draft updated");
        }

        public async Task<Invoice> CreateAsync(Invoice invoice)
        {
            PaymentRequestModel paymentRequest = await CreatePaymentRequestAsync(invoice);

            invoice.Status = InvoiceStatus.Unpaid;
            invoice.CreatedDate = DateTime.UtcNow;
            invoice.PaymentRequestId = paymentRequest.Id;

            Invoice createdInvoice = await _invoiceRepository.InsertAsync(invoice);

            _log.InfoWithDetails("Invoice created", invoice.Sanitize());

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

            _log.InfoWithDetails("Invoice created from draft", invoice.Sanitize());

            await WriteHistory(invoice, "Invoice created from draft");

            return invoice;
        }

        public Task<Invoice> ChangePaymentRequestAsync(string invoiceId, string paymentAssetId)
        {
            return ChangePaymentRequestAsync(invoiceId, paymentAssetId, null);
        }

        private Task<Invoice> ChangePaymentRequestAsync(string invoiceId, string paymentAssetId, decimal? amount)
        {
            return ChangePaymentRequestAsync(invoiceId, paymentAssetId, amount, false);
        }

        private async Task<Invoice> ChangePaymentRequestAsync(string invoiceId, string paymentAssetId, decimal? amount, bool allowUnderpaid)
        {
            Invoice invoice = await _invoiceRepository.FindByIdAsync(invoiceId);

            if (invoice == null)
                throw new InvoiceNotFoundException(invoiceId);

            bool isPayingUnderpaid = false;
            if (allowUnderpaid && invoice.Status == InvoiceStatus.Underpaid)
            {
                isPayingUnderpaid = true;
            }
            else if (invoice.Status != InvoiceStatus.Unpaid)
            {
                throw new InvalidOperationException("Invoice status is invalid.");
            }

            if (invoice.PaymentAssetId == paymentAssetId
                && !isPayingUnderpaid)
                return invoice;
            
            var previousPaymentAssetId = invoice.PaymentAssetId;
            invoice.PaymentAssetId = paymentAssetId;

            PaymentRequestModel paymentRequest = await CreatePaymentRequestAsync(invoice, amount.HasValue && amount > 0 ? amount : null);

            var previousPaymentRequestId = invoice.PaymentRequestId;
            invoice.PaymentRequestId = paymentRequest.Id;
            // for underpaid mark also that there are multiple paid payment requests
            if (isPayingUnderpaid)
            {
                invoice.HasMultiplePaymentRequests = true;
            }

            await _invoiceRepository.UpdateAsync(invoice);
            
            await WritePaymentRequestHistory(invoice.MerchantId, invoice.Id, previousPaymentRequestId, previousPaymentAssetId);

            await CancelPaymentRequestAsync(invoice.MerchantId, previousPaymentRequestId);

            _log.InfoWithDetails("Payment request changed", 
                new { invoice = invoice.Sanitize(), previousPaymentRequestId, newPaymentRequest = paymentRequest });

            return invoice;
        }

        public async Task DeleteAsync(string invoiceId)
        {
            Invoice invoice = await _invoiceRepository.FindByIdAsync(invoiceId);

            if (invoice == null)
                throw new InvoiceNotFoundException(invoiceId);

            if (invoice.Status == InvoiceStatus.Draft)
            {
                await _invoiceRepository.DeleteAsync(invoice.MerchantId, invoice.Id);

                IEnumerable<FileInfo> fileInfos = await _fileInfoRepository.GetAsync(invoiceId);

                foreach (FileInfo fileInfo in fileInfos)
                {
                    await _fileInfoRepository.DeleteAsync(invoiceId, fileInfo.Id);
                    await _fileRepository.DeleteAsync(fileInfo.Id);
                }

                _log.InfoWithDetails("Invoice deleted.", invoice.Sanitize());

                await _historyRepository.DeleteAsync(invoiceId);
            }
            else if (invoice.Status == InvoiceStatus.Unpaid)
            {
                await CancelPaymentRequestAsync(invoice.MerchantId, invoice.PaymentRequestId);

                await _invoiceRepository.SetStatusAsync(invoice.MerchantId, invoice.Id, InvoiceStatus.Removed);

                _log.InfoWithDetails("Invoice removed", invoice.Sanitize());

                invoice.Status = InvoiceStatus.Removed;

                await WriteHistory(invoice, "Invoice removed");
            }
            else
            {
                _log.WarningWithDetails("Cannot remove invoice", invoice.Sanitize());
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

            bool isRevertingStatus = false;
            InvoiceStatus previousStatus = invoice.Status;
            InvoiceStatus status = StatusConverter.Convert(message.Status, message.ProcessingError);

            decimal? totalPaidAmountInSettlementAsset = 0;
            decimal currentPaidAmountInSettlementAsset = message.SettlementAssetId == message.PaymentAssetId
                ? message.PaidAmount
                : message.Order != null ? message.PaidAmount * message.Order.ExchangeRate : 0;

            if (status.IsPaidStatus())
            {
                if (invoice.HasMultiplePaymentRequests || status == InvoiceStatus.Underpaid)
                {
                    // TODO: for future - if there will be multiple paid payment requests with at least one SettlementAssetId != PaymentAssetId  
                    // then rounding is required to accuracy of SettlementAssetId for comparing calculated total with field Amount
                    totalPaidAmountInSettlementAsset = await GetTotalPaidAmountInSettlementAsset(invoice, ignoreCurrentPaymentRequest: true);
                    totalPaidAmountInSettlementAsset += currentPaidAmountInSettlementAsset;

                    InvoiceStatus invoiceStatus;

                    if (totalPaidAmountInSettlementAsset < invoice.Amount)
                    {
                        invoiceStatus = InvoiceStatus.Underpaid;
                    }
                    else if (totalPaidAmountInSettlementAsset > invoice.Amount)
                    {
                        invoiceStatus = InvoiceStatus.Overpaid;
                    }
                    else
                    {
                        invoiceStatus = InvoiceStatus.Paid;
                    }

                    _log.InfoWithDetails("Calculate status when HasMultiplePaymentRequests or Underpaid", new
                    {
                        invoiceId = invoice.Id,
                        totalPaidAmountInSettlementAsset,
                        messageConvertedStatus = status.ToString(),
                        calculatedInvoiceStatus = invoiceStatus.ToString(),
                        message
                    });

                    status = invoiceStatus;
                }

                await _invoiceRepository.SetPaidAmountAsync(invoice.MerchantId, invoice.Id, message.PaidAmount, totalPaidAmountInSettlementAsset);
            }

            if (invoice.Status == status && invoice.Status != InvoiceStatus.Underpaid)
                return;

            // check whether we need to revert to Underpaid status
            if (previousStatus == InvoiceStatus.InProgress 
                && status == InvoiceStatus.Unpaid)
            {
                isRevertingStatus = true;

                if (invoice.HasMultiplePaymentRequests)
                {
                    status = InvoiceStatus.Underpaid;
                }
            }

            await _invoiceRepository.SetStatusAsync(invoice.MerchantId, invoice.Id, status);

            // if we are updating status from "InProgress" to any other - we have to release the lock
            if (previousStatus == InvoiceStatus.InProgress)
                await _paymentLocksService.ReleaseLockAsync(invoice.Id, invoice.MerchantId);

            _log.InfoWithDetails("Status updated.", new
            {
                invoiceId = invoice.Id,
                status = status.ToString(),
                message
            });

            // if some error occured during payment then status reverted and we need to remove history operation
            if (isRevertingStatus)
            {
                var invoicePayerHistory = await _invoicePayerHistoryRepository.GetAsync(invoice.Id, paymentRequestId);

                if (invoicePayerHistory != null)
                {
                    await RemoveHistoryOperations(message, invoicePayerHistory);
                }

                return;
            }

            invoice.Status = status;

            var history = Mapper.Map<HistoryItem>(invoice);
            history.SettlementAssetId = message.SettlementAssetId;
            history.PaymentAssetId = message.PaymentAssetId;
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

            // send additional info only for Paid Statuses and if invoice was paid by another merchant inside our system
            var invoicePayerHistoryItem = await _invoicePayerHistoryRepository.GetAsync(invoice.Id, paymentRequestId);
            if (!status.IsPaidStatus() || invoicePayerHistoryItem == null)
                return;

            var payerEmployee = await _employeeRepository.GetByIdAsync(invoicePayerHistoryItem.EmployeeId);

            if (payerEmployee == null)
            {
                _log.ErrorWithDetails("PayerEmployee should not be empty", new { message, invoiceId = invoice.Id, invoicePayerHistoryItem.EmployeeId });
                return;
            }

            var transaction = message.Transactions?.OrderByDescending(x => x.FirstSeen).FirstOrDefault();

            if (transaction == null)
            {
                _log.ErrorWithDetails("Transaction should not be empty", new { message, invoiceId = invoice.Id });
                return;
            }

            // send info to history service
            var historyOperationCommand = new HistoryOperationCommand
            {
                InvoiceId = invoice.Id,
                InvoiceStatus = invoice.Status.ToString(),
                MerchantId = payerEmployee.MerchantId,
                OppositeMerchantId = invoice.MerchantId,
                EmployeeEmail = payerEmployee.Email,
                Amount = message.PaidAmount,
                AssetId = message.PaymentAssetId,
                TxHash = transaction.Id,
                CreatedOn = transaction.FirstSeen
            };
            await _historyOperationService.PublishOutgoingInvoicePayment(historyOperationCommand);

            historyOperationCommand.MerchantId = invoice.MerchantId;
            historyOperationCommand.OppositeMerchantId = payerEmployee.MerchantId;
            await _historyOperationService.PublishIncomingInvoicePayment(historyOperationCommand);

            historyOperationCommand.EmployeeEmail = historyOperationCommand.EmployeeEmail.SanitizeEmail();
            _log.InfoWithDetails("Information sent to history service", new { message, historyOperationCommand });

            // send info to callback service
            var invoiceConfirmationCommand = new InvoiceConfirmationCommand
            {
                EmployeeEmail = payerEmployee.Email,
                InvoiceNumber = invoice.Number,
                TxHash = transaction.Id,
                TxFirstSeen = transaction.FirstSeen
            };

            if (status == InvoiceStatus.Underpaid && totalPaidAmountInSettlementAsset.HasValue)
            {
                decimal leftAmountToPayInSettlementAsset = invoice.Amount - totalPaidAmountInSettlementAsset.Value;
                invoiceConfirmationCommand.AmountLeftPaid = await GetCalculatedPaymentAmountAsync(invoice.SettlementAssetId, invoice.SettlementAssetId, leftAmountToPayInSettlementAsset, payerEmployee.MerchantId);
                invoiceConfirmationCommand.AmountPaid = currentPaidAmountInSettlementAsset;
            }

            await _invoiceConfirmationService.PublishInvoicePayment(invoiceConfirmationCommand);

            invoiceConfirmationCommand.EmployeeEmail = invoiceConfirmationCommand.EmployeeEmail.SanitizeEmail();
            _log.InfoWithDetails("Information sent to callback service", new { message, invoiceConfirmationCommand });

            // send push notifications
            await _pushNotificationService.PublishInvoicePayment(new InvoicePaidPushNotificationCommand
            {
                NotifiedMerchantId = invoice.MerchantId,
                PaidAmount = message.PaidAmount.ToString(),
                PayerMerchantName = await _merchantService.GetMerchantNameAsync(payerEmployee.MerchantId)
            });

            // removing at the end as this is the longer operation comparing to putting messages to rabbit
            await RemoveHistoryOperations(message, invoicePayerHistoryItem);
        }

        private async Task RemoveHistoryOperations(PaymentRequestDetailsMessage message, InvoicePayerHistoryItem invoicePayerHistory)
        {
            try
            {
                await Task.WhenAll(_historyOperationService.RemoveAsync(invoicePayerHistory.OutgoingHistoryOperationId),
                    _historyOperationService.RemoveAsync(invoicePayerHistory.IncomingHistoryOperationId));
            }
            catch (Exception ex)
            {
                _log.ErrorWithDetails(ex, new { message, invoicePayerHistory });
            }
        }

        public async Task<IReadOnlyList<Invoice>> ValidateForPayingInvoicesAsync(string merchantId, IEnumerable<string> invoicesIds, string assetForPay)
        {
            try
            {
                var merchant = await _payInternalClient.GetMerchantByIdAsync(merchantId);
            }
            catch (DefaultErrorResponseException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                throw new MerchantNotFoundException(merchantId);
            }

            if (!string.IsNullOrEmpty(assetForPay))
            {
                try
                {
                    // check in assets merchant settings
                    var assetMerchantSettingsResponse = await _payInternalClient.GetAssetMerchantSettingsAsync(merchantId);
                    if (assetMerchantSettingsResponse == null
                        || !assetMerchantSettingsResponse.PaymentAssets.Split(";").Contains(assetForPay))
                    {
                        // check whether it is base asset
                        string baseAssetId = await _merchantSettingService.GetBaseAssetAsync(merchantId);

                        if (string.IsNullOrEmpty(baseAssetId)
                            || baseAssetId != assetForPay)
                            throw new AssetNotAvailableForMerchantException();
                    }
                }
                catch (DefaultErrorResponseException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new AssetNotAvailableForMerchantException();
                }
            }

            IReadOnlyList<string> groupMerchants = await _merchantService.GetGroupMerchantsAsync(merchantId);

            var invoices = await GetByIdsAsync(invoicesIds);

            // Get group merchants and checks that the invoices belong to them
            foreach (var invoice in invoices)
            {
                if (!groupMerchants.Contains(invoice.MerchantId))
                    throw new InvoiceNotInsideGroupException(invoice.Id);
                if (invoice.ClientName != merchantId)
                    throw new MerchantNotInvoiceClientException(invoice.Id);
            }

            return invoices;
        }

        public async Task PayInvoicesAsync(Employee employee, IEnumerable<Invoice> invoices, string assetForPay, decimal amount)
        {
            var invoicesWithWrongStatus = invoices.Where(x => x.Status != InvoiceStatus.Unpaid && x.Status != InvoiceStatus.Underpaid);
            if (invoicesWithWrongStatus.Any())
                throw new InvalidOperationException("One of the invoices has been already in progress");

            decimal sumInAssetForPay = await GetSumInAssetForPayAsync(employee.MerchantId, invoices, assetForPay);

            if (amount > sumInAssetForPay)
                throw new InvalidOperationException("The amount is more than required to pay");

            _log.InfoWithDetails("PayInvoices started", new { employee.Id, invoicesIds = invoices.Select(x => x.Id), amount, sumInAssetForPay });

            // Pay firstly older invoices (sorted by date)
            invoices = invoices.ToList().OrderBy(x => x.CreatedDate);

            // Pay first invoices by full amount and the last one partially when amount is less than needed
            decimal leftAmountInAssetForPay = amount;
            int invoicesCount = invoices.Count();
            int processedCount = 0;
            int paidCount = 0;
            int occuredErrorsCount = 0;
            
            foreach (var invoice in invoices)
            {
                processedCount++;
                decimal paymentAmountInAssetForPay = 0;
                string paymentRequestIdForPaying = string.Empty;

                if (DateTime.UtcNow > invoice.DueDate)
                {
                    occuredErrorsCount++;
                    _log.ErrorWithDetails("Invoice has passed DueDate", new { invoice = invoice.Sanitize() });
                    continue;
                }

                // redis locking mechanism
                bool locked = await _paymentLocksService.TryAcquireLockAsync(invoice.Id, invoice.MerchantId, invoice.DueDate);
                if (!locked)
                {
                    occuredErrorsCount++;
                    _log.ErrorWithDetails("Invoice is in progress and locked for paying", new { invoiceId = invoice.Id });
                    continue;
                }

                try
                {
                    switch (invoice.Status)
                    {
                        case InvoiceStatus.Unpaid:
                            {
                                var paymentInfo = await GetPaymentInfoForUnpaid(invoice, assetForPay, employee.MerchantId, isPaying: true);

                                paymentAmountInAssetForPay = GetPaymentAmount(leftAmountInAssetForPay, paymentInfo.AmountToPayInAssetForPay);
                                paymentRequestIdForPaying = paymentInfo.PaymentRequestIdForPaying;

                                await CheckoutOrderAsync(invoice.MerchantId, paymentInfo.PaymentRequestIdForPaying);

                                try
                                {
                                    await PayPaymentRequestAsync(invoice.MerchantId, employee.MerchantId, paymentRequestIdForPaying, paymentAmountInAssetForPay);
                                }
                                catch (InvalidOperationException)
                                {
                                    occuredErrorsCount++;
                                    await _paymentLocksService.ReleaseLockAsync(invoice.Id, invoice.MerchantId);
                                    continue;
                                }

                                await _invoiceRepository.SetStatusAsync(invoice.MerchantId, invoice.Id, InvoiceStatus.InProgress);

                                _log.InfoWithDetails("Paid Unpaid invoice", new
                                {
                                    InvoiceId = invoice.Id,
                                    invoice.MerchantId,
                                    PayerMerchantId = employee.MerchantId,
                                    paymentRequestIdForPaying,
                                    leftAmountInAssetForPay,
                                    paymentInfo.AmountToPayInAssetForPay
                                });
                            }
                            break;
                        case InvoiceStatus.Underpaid:
                            {
                                var paymentInfo = await GetPaymentInfoForUnderpaid(invoice, assetForPay, employee.MerchantId, isPaying: true);

                                paymentAmountInAssetForPay = GetPaymentAmount(leftAmountInAssetForPay, paymentInfo.LeftAmountToPayInAssetForPay);
                                paymentRequestIdForPaying = paymentInfo.PaymentRequestIdForPaying;

                                try
                                {
                                    await PayPaymentRequestAsync(invoice.MerchantId, employee.MerchantId, paymentRequestIdForPaying, paymentAmountInAssetForPay);
                                }
                                catch (InvalidOperationException)
                                {
                                    occuredErrorsCount++;
                                    await _paymentLocksService.ReleaseLockAsync(invoice.Id, invoice.MerchantId);
                                    continue;
                                }

                                await _invoiceRepository.SetStatusAsync(invoice.MerchantId, invoice.Id, InvoiceStatus.InProgress);

                                _log.InfoWithDetails("Paid Underpaid invoice", new
                                {
                                    InvoiceId = invoice.Id,
                                    invoice.MerchantId,
                                    PayerMerchantId = employee.MerchantId,
                                    paymentRequestIdForPaying,
                                    leftAmountInAssetForPay,
                                    paymentInfo.LeftAmountToPayInAssetForPay
                                });
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    occuredErrorsCount++;
                    await _paymentLocksService.ReleaseLockAsync(invoice.Id, invoice.MerchantId);
                    _log.Error(ex, "Error occured during paying", invoice.Sanitize());
                    continue;
                }

                // send info to history service
                var historyOperationCommand = new HistoryOperationCommand
                {
                    InvoiceId = invoice.Id,
                    InvoiceStatus = InvoiceStatus.InProgress.ToString(),
                    MerchantId = employee.MerchantId,
                    OppositeMerchantId = invoice.MerchantId,
                    EmployeeEmail = employee.Email,
                    Amount = paymentAmountInAssetForPay,
                    AssetId = assetForPay,
                    CreatedOn = DateTime.UtcNow
                };
                string outgoingHistoryOperationId = await _historyOperationService.PublishOutgoingInvoicePayment(historyOperationCommand);

                historyOperationCommand.MerchantId = invoice.MerchantId;
                historyOperationCommand.OppositeMerchantId = employee.MerchantId;
                string incomingHistoryOperationId = await _historyOperationService.PublishIncomingInvoicePayment(historyOperationCommand);

                await WriteInvoicePayerHistory(invoice.Id, employee.Id, paymentRequestIdForPaying, outgoingHistoryOperationId, incomingHistoryOperationId);

                leftAmountInAssetForPay -= paymentAmountInAssetForPay;
                paidCount++;

                if (leftAmountInAssetForPay <= 0 && processedCount < invoicesCount)
                {
                    throw new InvalidOperationException("The amount wasn't enough to pay all invoices");
                }
            }

            if (occuredErrorsCount > 0)
            {
                var message = occuredErrorsCount > 1
                    ? "Some of the invoices haven't been paid, please try again."
                    : "The invoice hasn't been paid, please try again.";
                throw new InvalidOperationException(message);
            }

            _log.InfoWithDetails("PayInvoices finished", new
            {
                employee.MerchantId,
                invoicesIds = invoices.Select(x => x.Id),
                leftAmountInAssetForPay,
                paidCount,
                count = invoices.Count()
            });

            decimal GetPaymentAmount(decimal leftAmount, decimal amountToPay)
            {
                return (leftAmount - amountToPay) < 0 ? leftAmount : amountToPay;
            }
        }

        private async Task WriteInvoicePayerHistory(string invoiceId, string employeeId, string paymentRequestIdForPaying, string outgoingHistoryOperationId, string incomingHistoryOperationId)
        {
            await _invoicePayerHistoryRepository.InsertAsync(new InvoicePayerHistoryItem
            {
                InvoiceId = invoiceId,
                PaymentRequestId = paymentRequestIdForPaying,
                EmployeeId = employeeId,
                OutgoingHistoryOperationId = outgoingHistoryOperationId,
                IncomingHistoryOperationId = incomingHistoryOperationId,
                CreatedAt = DateTime.UtcNow
            });
        }

        public async Task<decimal> GetSumInAssetForPayAsync(string merchantId, IEnumerable<Invoice> invoices, string assetForPay)
        {
            decimal sum = 0;

            foreach (var invoice in invoices)
            {
                decimal paymentAmountInAssetForPay = 0;

                switch (invoice.Status)
                {
                    case InvoiceStatus.Unpaid:
                        {
                            var paymentInfo = await GetPaymentInfoForUnpaid(invoice, assetForPay, merchantId, isPaying: false);
                            paymentAmountInAssetForPay = paymentInfo.AmountToPayInAssetForPay;
                        }
                        break;
                    case InvoiceStatus.Underpaid:
                        {
                            var paymentInfo = await GetPaymentInfoForUnderpaid(invoice, assetForPay, merchantId, isPaying: false);
                            paymentAmountInAssetForPay = paymentInfo.LeftAmountToPayInAssetForPay;
                        }
                        break;
                }

                sum += paymentAmountInAssetForPay;
            }

            return sum;
        }

        private async Task<(decimal AmountToPayInAssetForPay, string PaymentRequestIdForPaying)> 
            GetPaymentInfoForUnpaid(Invoice invoice, string assetForPay, string payerMerchantId, bool isPaying)
        {
            decimal amountToPayInAssetForPay;
            string paymentRequestIdForPaying = invoice.PaymentRequestId;

            if (isPaying)
            {
                if (assetForPay == invoice.PaymentAssetId)
                {
                    amountToPayInAssetForPay = await CheckoutAndGetPaymentAmount(invoice.MerchantId, invoice.PaymentRequestId);
                }
                else
                {
                    // When invoice in EUR and baseAsset is USD or vice versa

                    Invoice updatedInvoice = await ChangePaymentRequestAsync(invoice.Id, assetForPay, invoice.Amount);
                    paymentRequestIdForPaying = updatedInvoice.PaymentRequestId;
                    amountToPayInAssetForPay = await CheckoutAndGetPaymentAmount(invoice.MerchantId, paymentRequestIdForPaying);
                }
            }
            else
            {
                if (assetForPay == invoice.SettlementAssetId)
                {
                    amountToPayInAssetForPay = invoice.Amount;
                }
                else
                {
                    amountToPayInAssetForPay = await GetCalculatedPaymentAmountAsync(invoice.SettlementAssetId, assetForPay, invoice.Amount, payerMerchantId);
                }
            }

            return (amountToPayInAssetForPay, paymentRequestIdForPaying);
        }

        private async Task<(decimal LeftAmountToPayInSettlementAsset, decimal LeftAmountToPayInAssetForPay, string PaymentRequestIdForPaying)> 
            GetPaymentInfoForUnderpaid(Invoice invoice, string assetForPay, string payerMerchantId, bool isPaying)
        {
            decimal leftAmountToPayInAssetForPay;
            string paymentRequestIdForPaying = invoice.PaymentRequestId;
            decimal leftAmountToPayInSettlementAsset = await GetLeftAmountToPayInSettlementAsset(invoice);

            if (isPaying)
            {
                paymentRequestIdForPaying = await GetOrUpdatePaymentRequestIdForPaying(assetForPay, invoice, leftAmountToPayInSettlementAsset);
                leftAmountToPayInAssetForPay = await CheckoutAndGetPaymentAmount(invoice.MerchantId, paymentRequestIdForPaying);
            }
            else
            {
                leftAmountToPayInAssetForPay = await GetCalculatedPaymentAmountAsync(invoice.SettlementAssetId, assetForPay, leftAmountToPayInSettlementAsset, payerMerchantId);
            }

            return (leftAmountToPayInSettlementAsset, leftAmountToPayInAssetForPay, paymentRequestIdForPaying);
        }

        private async Task<decimal> GetLeftAmountToPayInSettlementAsset(Invoice invoice)
        {
            // Calculation of leftAmountToPayInSettlementAsset:
            // get payment requests from history
            // get invoice history - get PaidAmount, ExchangeRate
            // add to paidInSettlementAsset
            // get current payment request
            // get payment request from PayInternal to check whether paid or not
            // in case there is PaidDate then add to paidInSettlementAsset
            // then substract paidInSettlementAsset from invoice Amount
            var paidInSettlementAsset = await GetTotalPaidAmountInSettlementAsset(invoice, ignoreCurrentPaymentRequest: false);

            return invoice.Amount - paidInSettlementAsset;
        }

        private async Task<decimal> GetTotalPaidAmountInSettlementAsset(Invoice invoice, bool ignoreCurrentPaymentRequest)
        {
            decimal paidInSettlementAsset = 0;

            var invoiceHistoryLazy = new Lazy<IReadOnlyList<HistoryItem>>(
                () => _historyRepository.GetByInvoiceIdAsync(invoice.Id).GetAwaiter().GetResult());

            //if has other payment requests - need to summarize on them
            if (invoice.HasMultiplePaymentRequests)
            {
                var paymentRequestHistory = await _paymentRequestHistoryRepository.GetByInvoiceIdAsync(invoice.Id);
                var paidPaymentRequestHistory = paymentRequestHistory.Where(x => x.IsPaid);

                if (paidPaymentRequestHistory.Any())
                {
                    foreach (var prh in paidPaymentRequestHistory)
                    {
                        var invoiceHistoryItem = invoiceHistoryLazy.Value.FirstOrDefault(x => x.PaymentRequestId == prh.PaymentRequestId && x.PaidAmount > 0);

                        if (invoiceHistoryItem == null)
                            throw new InvalidOperationException($"Invoice history record with paid payment request {prh.PaymentRequestId} not found");

                        if (invoice.SettlementAssetId == invoiceHistoryItem.PaymentAssetId)
                        {
                            paidInSettlementAsset += invoiceHistoryItem.PaidAmount;
                        }
                        else
                        {
                            paidInSettlementAsset += invoiceHistoryItem.PaidAmount * invoiceHistoryItem.ExchangeRate;
                        }
                    }
                }
            }

            if (ignoreCurrentPaymentRequest)
                return paidInSettlementAsset;

            PaymentRequestModel currentPaymentRequest = await _payInternalClient.GetPaymentRequestAsync(invoice.MerchantId, invoice.PaymentRequestId);
            if (currentPaymentRequest.PaidAmount > 0)
            {
                if (invoice.SettlementAssetId == invoice.PaymentAssetId)
                {
                    paidInSettlementAsset += (decimal)currentPaymentRequest.PaidAmount;
                }
                else
                {
                    var invoiceHistoryItem = invoiceHistoryLazy.Value.FirstOrDefault(x => x.PaymentRequestId == invoice.PaymentRequestId && x.PaidAmount > 0);

                    if (invoiceHistoryItem == null)
                        throw new InvalidOperationException($"Invoice history record with paid payment request {invoice.PaymentRequestId} not found");

                    paidInSettlementAsset += invoiceHistoryItem.PaidAmount * invoiceHistoryItem.ExchangeRate;
                }
            }

            return paidInSettlementAsset;
        }

        private async Task<string> GetOrUpdatePaymentRequestIdForPaying(string assetForPay, Invoice invoice, decimal leftAmountToPayInSettlementAsset)
        {
            string paymentRequestIdForPaying = invoice.PaymentRequestId;

            if (await IsNeedToCreateNewPaymentRequest(assetForPay, invoice.PaymentAssetId, invoice.MerchantId, invoice.PaymentRequestId))
            {
                Invoice updatedInvoice = await ChangePaymentRequestAsync(invoice.Id, assetForPay, leftAmountToPayInSettlementAsset, allowUnderpaid: true);
                paymentRequestIdForPaying = updatedInvoice.PaymentRequestId;
            }

            return paymentRequestIdForPaying;
        }

        private async Task<bool> IsNeedToCreateNewPaymentRequest(string assetForPay, string paymentAssetId, string merchantIdOfInvoice, string paymentRequestId)
        {
            bool isNeedToCreateNewPaymentRequest = false;

            if (assetForPay != paymentAssetId)
            {
                isNeedToCreateNewPaymentRequest = true;
            }
            else
            {
                // If PaymentRequest status is New then just pay
                // otherwise need to change payment request
                PaymentRequestModel currentPaymentRequest = await _payInternalClient.GetPaymentRequestAsync(merchantIdOfInvoice, paymentRequestId);
                if (currentPaymentRequest.Status != PayInternal.Client.Models.PaymentRequest.PaymentRequestStatus.New)
                {
                    isNeedToCreateNewPaymentRequest = true;
                }
            }

            return isNeedToCreateNewPaymentRequest;
        }

        private async Task PayPaymentRequestAsync(string merchantIdOfInvoice, string payerMerchantId, string paymentRequestId, decimal amount)
        {
            try
            {
                await _payInternalClient.PayAsync(new PaymentRequest
                {
                    MerchantId = merchantIdOfInvoice,
                    PayerMerchantId = payerMerchantId,
                    PaymentRequestId = paymentRequestId,
                    Amount = amount
                });
            }
            catch (DefaultErrorResponseException ex)
            {
                _log.Error(ex, ex.Error.ErrorMessage, new { merchantIdOfInvoice, payerMerchantId, paymentRequestId, amount });

                throw new InvalidOperationException($"{ex.StatusCode}: {ex.Error.ErrorMessage}", ex);
            }
        }

        private async Task<decimal> CheckoutAndGetPaymentAmount(string merchantIdOfInvoice, string paymentRequestId)
        {
            OrderModel order = await CheckoutOrderAsync(merchantIdOfInvoice, paymentRequestId);
            return order.PaymentAmount;
        }

        private async Task<decimal> GetCalculatedPaymentAmountAsync(string settlementAssetId, string paymentAssetId, decimal amount, string payerMerchantId)
        {
            try
            {
                CalculatedAmountResponse response = await _payInternalClient.GetCalculatedAmountInfoAsync(new GetCalculatedAmountInfoRequest
                {
                    SettlementAssetId = settlementAssetId,
                    PaymentAssetId = paymentAssetId,
                    Amount = amount,
                    MerchantId = payerMerchantId
                });
                return response.PaymentAmount;
            }
            catch (DefaultErrorResponseException ex)
            {
                _log.Error(ex, ex.Error.ErrorMessage, new { settlementAssetId, paymentAssetId, amount, payerMerchantId });

                throw new InvalidOperationException($"{ex.StatusCode}: {ex.Error.ErrorMessage}", ex);
            }

        }

        private async Task<OrderModel> CheckoutOrderAsync(string merchantIdOfInvoice, string paymentRequestId)
        {
            try
            {
                //var sw = Stopwatch.StartNew();
                OrderModel order = await _payInternalClient.ChechoutOrderAsync(new ChechoutRequestModel
                {
                    MerchantId = merchantIdOfInvoice,
                    PaymentRequestId = paymentRequestId
                });
                //_log.WriteInfo(nameof(PayPaymentRequestAsync), new { paymentRequestId }, $"ChechoutOrderAsync elapsed {sw.ElapsedMilliseconds}");
                return order;
            }
            catch (DefaultErrorResponseException ex)
            {
                _log.Error(ex, ex.Error.ErrorMessage, new { merchantIdOfInvoice, paymentRequestId });

                throw new InvalidOperationException($"{ex.StatusCode}: {ex.Error.ErrorMessage}", ex);
            }
        }

        private async Task CancelPaymentRequestAsync(string merchantId, string paymentRequestId)
        {
            try
            {
                var paymentRequest = await _payInternalClient.GetPaymentRequestAsync(merchantId, paymentRequestId);
                if (paymentRequest.Status == PayInternal.Client.Models.PaymentRequest.PaymentRequestStatus.New)
                {
                    await _payInternalClient.CancelAsync(merchantId, paymentRequestId);
                }
            }
            catch (Exception ex)
            {
                const string message = "PaymentRequest is not cancelled";
                _log.Error(ex, message, new { merchantId, paymentRequestId });
                throw new InvalidOperationException(message, ex);
            }
        }

        private async Task<PaymentRequestModel> CreatePaymentRequestAsync(Invoice invoice, decimal? amount = null)
        {
            try
            {
                return await _payInternalClient.CreatePaymentRequestAsync(
                    new CreatePaymentRequestModel
                    {
                        MerchantId = invoice.MerchantId,
                        Amount = amount ?? invoice.Amount,
                        DueDate = invoice.DueDate,
                        PaymentAssetId = invoice.PaymentAssetId,
                        SettlementAssetId = invoice.SettlementAssetId
                    });
            }
            catch (DefaultErrorResponseException ex)
            {
                _log.Error(ex, ex.Error.ErrorMessage, new { invoice = invoice.Sanitize(), amount });

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
                IsPaid = paymentRequest.PaidAmount > 0,
                CreatedAt = paymentRequest.Timestamp ?? DateTime.UtcNow
            };

            await _paymentRequestHistoryRepository.InsertAsync(history);
        }

        public async Task MarkDisputeAsync(string invoiceId, string reason, string employeeId)
        {
            var validationResult = await ValidateDisputeActions(invoiceId, employeeId, isMarkAction: true);

            await _invoiceRepository.MarkDisputeAsync(invoiceId);

            var now = DateTime.UtcNow;
            await _invoiceDisputeRepository.InsertAsync(new InvoiceDispute
            {
                InvoiceId = invoiceId,
                Reason = reason,
                EmployeeId = employeeId,
                CreatedAt = now
            });

            // send info to callback service
            await _invoiceConfirmationService.PublishDisputeRaised(new DisputeRaisedConfirmationCommand
            {
                InvoiceNumber = validationResult.Invoice.Number,
                EmployeeEmail = validationResult.Employee.Email,
                Reason = reason,
                DateTime = now
            });

            // send push notifications
            await _pushNotificationService.PublishDisputeRaised(new DisputeRaisedPushNotificationCommand
            {
                NotifiedMerchantId = validationResult.Invoice.MerchantId
            });
        }

        public async Task CancelDisputeAsync(string invoiceId, string employeeId)
        {
            var validationResult = await ValidateDisputeActions(invoiceId, employeeId, isMarkAction: false);

            await _invoiceRepository.CancelDisputeAsync(invoiceId);

            // send info to callback service
            await _invoiceConfirmationService.PublishDisputeCancelled(new DisputeCancelledConfirmationCommand
            {
                InvoiceNumber = validationResult.Invoice.Number,
                EmployeeEmail = validationResult.Employee.Email,
                DateTime = DateTime.UtcNow
            });

            // send push notifications
            await _pushNotificationService.PublishDisputeCancelled(new DisputeCancelledPushNotificationCommand
            {
                NotifiedMerchantId = validationResult.Invoice.MerchantId,
                EmployeeFullName = validationResult.Employee.FullName()
            });
        }

        public async Task<InvoiceDispute> GetInvoiceDisputeAsync(string invoiceId)
        {
            var invoiceDispute = await _invoiceDisputeRepository.GetByInvoiceIdAsync(invoiceId);

            if (invoiceDispute == null)
                throw new InvoiceDisputeNotFoundException(invoiceId);

            return invoiceDispute;
        }

        private async Task<(Invoice Invoice, Employee Employee)> ValidateDisputeActions(string invoiceId, string employeeId, bool isMarkAction)
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

            return (invoice, employee);
        }
    }
}
