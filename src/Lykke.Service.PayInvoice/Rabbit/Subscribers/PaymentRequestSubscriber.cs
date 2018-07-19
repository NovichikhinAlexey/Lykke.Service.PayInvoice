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
using Lykke.Common.Log;
using Lykke.Service.PayInvoice.Core.Extensions;

namespace Lykke.Service.PayInvoice.Rabbit.Subscribers
{
    public class PaymentRequestSubscriber : IStartable, IStopable
    {
        private readonly RabbitSettings _settings;
        private readonly ILogFactory _logFactory;
        private readonly IInvoiceService _invoiceService;
        private readonly ILog _log;
        private RabbitMqSubscriber<PaymentRequestDetailsMessage> _subscriber;

        public PaymentRequestSubscriber(
            IInvoiceService invoiceService,
            RabbitSettings settings,
            ILogFactory logFactory)
        {
            _invoiceService = invoiceService;
            _log = logFactory.CreateLog(this);
            _settings = settings;
            _logFactory = logFactory;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .CreateForSubscriber(_settings.ConnectionString, _settings.PaymentRequestsExchangeName, "payinvoice")
                .MakeDurable();

            settings.DeadLetterExchangeName = null;

            _subscriber = new RabbitMqSubscriber<PaymentRequestDetailsMessage>(
                    _logFactory,
                    settings,
                    new ResilientErrorHandlingStrategy(_logFactory, settings,
                        TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<PaymentRequestDetailsMessage>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
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
                await _invoiceService.UpdateAsync(message);
            }
            catch (Exception ex)
            {
                _log.Error(ex, message);
            }
        }
    }
}
