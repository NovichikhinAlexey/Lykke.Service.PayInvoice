using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.PayInvoice.Core.Domain.DataMigrations;
using Lykke.Service.PayInvoice.Core.Repositories;
using Common;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInternal.Client;
using Lykke.Service.PayInternal.Client.Models.PaymentRequest;

namespace Lykke.Service.PayInvoice.Services
{
    public class DataMigrationOneDotSeven : IDataMigrationOneDotSeven
    {
        private readonly IPayInternalClient _payInternalClient;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IDataMigrationRepository _dataMigrationRepository;
        private readonly ILog _log;

        public DataMigrationOneDotSeven(
            IPayInternalClient payInternalClient,
            IInvoiceRepository invoiceRepository,
            IDataMigrationRepository dataMigrationRepository,
            ILog log)
        {
            _payInternalClient = payInternalClient;
            _invoiceRepository = invoiceRepository;
            _dataMigrationRepository = dataMigrationRepository;
            _log = log.CreateComponentScope(nameof(DataMigrationOneDotSeven));
        }

        public string Name => "1.7";

        public async Task<bool> ExecuteAsync()
        {
            LogInfo("Started");

            var sw = new Stopwatch();
            sw.Start();

            // Get necessary data from DB
            var invoices = await _invoiceRepository.GetAllPaidAsync();

            LogInfo(new { invoices.Count }, "Get all invoices");

            var merchantIds = invoices.Select(x => x.MerchantId).Distinct().ToList();

            LogInfo(new { merchantIds.Count, merchantIds }, "Get merchant ids");

            // Get necessary data from PayInternal
            var paymentRequests = new List<PaymentRequestModel>();
            foreach (var merchantId in merchantIds)
            {
                var prs = await _payInternalClient.GetPaymentRequestsAsync(merchantId);
                paymentRequests.AddRange(prs);
                LogInfo(new { prs.Count, merchantId }, $"Get payment requests of {merchantId}");
            }

            var paymentRequestsDictionary = paymentRequests.ToDictionary(x => x.Id, x => x);

            LogInfo(new { paymentRequests.Count }, "Get all payment requests");

            var invoicesToUpdate = new List<Invoice>();
            // copy PaidAmount
            foreach (var invoice in invoices)
            {
                if (paymentRequestsDictionary.TryGetValue(invoice.PaymentRequestId, out PaymentRequestModel pr))
                {
                    invoice.PaidAmount = (decimal)pr.PaidAmount;
                    invoicesToUpdate.Add(invoice);
                }
            }

            LogInfo(new { invoicesToUpdate.Count }, "Invoices with copied PaidAmount");

            // Save in DB
            int count = 0;
            foreach (var invoice in invoicesToUpdate)
            {
                await _invoiceRepository.SetPaidAmountAsync(invoice.MerchantId, invoice.Id, invoice.PaidAmount);
                count++;
                if (count % 100 == 0)
                {
                    LogInfo(new { count }, $"{count} entities updated");
                }
            }

            LogInfo(new { count }, $"All {count} entities updated");

            await _dataMigrationRepository.AddAsync(Name);

            sw.Stop();
            LogInfo(new { sw.ElapsedMilliseconds }, "Finished");

            return true;
        }

        private void LogInfo(string info)
        {
            LogInfo(null, info);
        }

        private void LogInfo(object context, string info)
        {
            _log.WriteInfo(nameof(ExecuteAsync), context, info);
        }
    }
}