using System;
using Autofac;
using Common;
using Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using System.Threading.Tasks;
using Lykke.Service.PayInternal.Contract.PaymentRequest;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Settings.ServiceSettings.Rabbit;

namespace Lykke.Service.PayInvoice.Rabbit.Subscribers
{
    public class PaymentRequestSubscriber : IStartable, IStopable
    {
        private readonly RabbitSettings _settings;
        private readonly IInvoiceService _invoiceService;
        private readonly ILog _log;
        private RabbitMqSubscriber<PaymentRequestDetailsMessage> _subscriber;

        public PaymentRequestSubscriber(
            IInvoiceService invoiceService,
            RabbitSettings settings,
            ILog log)
        {
            _invoiceService = invoiceService;
            _log = log;
            _settings = settings;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .CreateForSubscriber(_settings.ConnectionString, _settings.PaymentRequestsExchangeName, "payinvoice")
                .MakeDurable();

            settings.DeadLetterExchangeName = null;

            _subscriber = new RabbitMqSubscriber<PaymentRequestDetailsMessage>(settings,
                    new ResilientErrorHandlingStrategy(_log, settings,
                        TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<PaymentRequestDetailsMessage>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .SetLogger(_log)
                .Start();
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }

        public void Stop()
        {
            _subscriber.Stop();
        }

        private async Task ProcessMessageAsync(PaymentRequestDetailsMessage message)
        {
            try
            {
                await _invoiceService.SetStatusAsync(message.Id, message.Status.ToString(), message.Error);

                await _log.WriteInfoAsync(nameof(PaymentRequestSubscriber), nameof(ProcessMessageAsync),
                    message.ToJson(), "Transaction updated message processed");
            }
            catch (Exception exception)
            {
                await _log.WriteErrorAsync(nameof(PaymentRequestSubscriber), nameof(ProcessMessageAsync),
                    message.ToJson(), exception);
                throw;
            }
        }
    }
}