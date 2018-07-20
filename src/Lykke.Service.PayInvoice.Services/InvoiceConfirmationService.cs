using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.PayCallback.Client.InvoiceConfirmation;
using Lykke.Service.PayInvoice.Core.Domain.InvoiceConfirmation;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Core.Settings;
using Polly;
using Polly.Retry;

namespace Lykke.Service.PayInvoice.Services
{
    public class InvoiceConfirmationService : IInvoiceConfirmationService
    {
        private readonly InvoiceConfirmationPublisher _invoiceConfirmationPublisher;
        private readonly RetryPolicy _retryPolicy;
        private readonly ILog _log;

        public InvoiceConfirmationService(
            InvoiceConfirmationPublisher invoiceConfirmationPublisher,
            RetryPolicySettings retryPolicySettings,
            ILogFactory logFactory)
        {
            _invoiceConfirmationPublisher = invoiceConfirmationPublisher;
            _log = logFactory.CreateLog(this);
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryPolicySettings.DefaultAttempts,
                    attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    (ex, timespan) => _log.Error(ex, "Publish confirmations to callback with retry"));
        }

        public async Task PublishDisputeCancelled(DisputeCancelledConfirmationCommand command)
        {
            await _retryPolicy.ExecuteAsync(() => _invoiceConfirmationPublisher.PublishAsync(new InvoiceConfirmation
            {
                UserEmail = command.EmployeeEmail,
                InvoiceList = new InvoiceOperation[]
                {
                    new InvoiceOperation {
                        InvoiceNumber = command.InvoiceNumber,
                        Dispute = new DisputeOperation
                        {
                            Status = DisputeStatus.Cancelled,
                            DateTime = command.DateTime
                        }
                    }
                }
            }));
        }

        public async Task PublishDisputeRaised(DisputeRaisedConfirmationCommand command)
        {
            await _retryPolicy.ExecuteAsync(() => _invoiceConfirmationPublisher.PublishAsync(new InvoiceConfirmation
            {
                UserEmail = command.EmployeeEmail,
                InvoiceList = new InvoiceOperation[]
                {
                    new InvoiceOperation {
                        InvoiceNumber = command.InvoiceNumber,
                        Dispute = new DisputeOperation
                        {
                            Status = DisputeStatus.Raised,
                            Reason = command.Reason,
                            DateTime = command.DateTime
                        }
                    }
                }
            }));
        }

        public async Task PublishInvoicePayment(InvoiceConfirmationCommand command)
        {
            await _retryPolicy.ExecuteAsync(() => _invoiceConfirmationPublisher.PublishAsync(new InvoiceConfirmation
            {
                UserEmail = command.EmployeeEmail,
                InvoiceList = new InvoiceOperation[]
                {
                    new InvoiceOperation {
                        InvoiceNumber = command.InvoiceNumber,
                        AmountPaid = command.AmountPaid,
                        AmountLeftPaid = command.AmountLeftPaid
                    }
                },
                BlockchainHash = command.TxHash,
                SettledInBlockchainDateTime = command.TxFirstSeen
            }));
        }
    }
}
