using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.Common.Log;
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
        private readonly RetryPolicy _retryPolicy;
        private readonly ILog _log;

        public HistoryOperationService(
            HistoryOperationPublisher historyOperationPublisher,
            RetryPolicySettings retryPolicySettings,
            ILog log)
        {
            _historyOperationPublisher = historyOperationPublisher;
            _log = log.CreateComponentScope(nameof(HistoryOperationService));
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryPolicySettings.DefaultAttempts,
                    attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    (ex, timespan) => _log.Error("Publish invoice payment to history with retry", ex));
        }

        public async Task PublishOutgoingInvoicePayment(HistoryOperationCommand command)
        {
            var historyOperation = Mapper.Map<HistoryOperation>(command);
            historyOperation.Type = HistoryOperationType.OutgoingInvoicePayment;

            await _retryPolicy.ExecuteAsync(() => _historyOperationPublisher.PublishAsync(historyOperation));
        }

        public async Task PublishIncomingInvoicePayment(HistoryOperationCommand command)
        {
            var historyOperation = Mapper.Map<HistoryOperation>(command);
            historyOperation.Type = HistoryOperationType.IncomingInvoicePayment;

            await _retryPolicy.ExecuteAsync(() => _historyOperationPublisher.PublishAsync(historyOperation));
        }
    }
}
