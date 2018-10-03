using System;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.PayHistory.Client;
using Lykke.Service.PayHistory.Client.AutorestClient.Models;
using Lykke.Service.PayHistory.Client.Publisher;
using Lykke.Service.PayInvoice.Core.Domain.HistoryOperation;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Core.Settings;
using Polly;
using Polly.Retry;

namespace Lykke.Service.PayInvoice.Services
{
    public class HistoryOperationService : IHistoryOperationService
    {
        private readonly HistoryOperationPublisher _historyOperationPublisher;
        private readonly IPayHistoryClient _payHistoryClient;
        private readonly RetryPolicy _retryPolicy;
        private readonly ILog _log;

        public HistoryOperationService(
            HistoryOperationPublisher historyOperationPublisher,
            IPayHistoryClient payHistoryClient,
            RetryPolicySettings retryPolicySettings,
            ILogFactory logFactory)
        {
            _historyOperationPublisher = historyOperationPublisher;
            _payHistoryClient = payHistoryClient;
            _log = logFactory.CreateLog(this);
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryPolicySettings.DefaultAttempts,
                    attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    (ex, timespan) => _log.Error(ex, "Publish invoice payment to history with retry"));
        }

        public async Task<string> PublishOutgoingInvoicePayment(HistoryOperationCommand command)
        {
            var historyOperation = Mapper.Map<HistoryOperation>(command);
            historyOperation.Type = HistoryOperationType.OutgoingInvoicePayment;

            await _retryPolicy.ExecuteAsync(() => _historyOperationPublisher.PublishAsync(historyOperation));

            return historyOperation.Id;
        }

        public async Task<string> PublishIncomingInvoicePayment(HistoryOperationCommand command)
        {
            var historyOperation = Mapper.Map<HistoryOperation>(command);
            historyOperation.Type = HistoryOperationType.IncomingInvoicePayment;

            await _retryPolicy.ExecuteAsync(() => _historyOperationPublisher.PublishAsync(historyOperation));

            return historyOperation.Id;
        }

        public async Task RemoveAsync(string id)
        {
            await _retryPolicy.ExecuteAsync(() => _payHistoryClient.SetRemovedAsync(id));
        }
    }
}
