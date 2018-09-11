using System;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.PayInternal.Contract.PaymentRequest;
using Lykke.Service.PayInvoice.Core.Domain.Notifications;
using Lykke.Service.PayInvoice.Core.Extensions;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Settings.ServiceSettings.Rabbit;

namespace Lykke.Service.PayInvoice.Rabbit.Subscribers
{
    public class NotificationsPaymentRequestSubscriber : IStartable, IStopable
    {
        private readonly RabbitSettings _settings;
        private readonly ILogFactory _logFactory;
        private readonly IInvoiceNotificationsService _notificationsService;
        private readonly ILog _log;

        private RabbitMqSubscriber<PaymentRequestDetailsMessage> _subscriber;

        public NotificationsPaymentRequestSubscriber(
            RabbitSettings settings, 
            ILogFactory logFactory, 
            IInvoiceNotificationsService notificationsService)
        {
            _settings = settings;
            _logFactory = logFactory;
            _notificationsService = notificationsService;
            _log = logFactory.CreateLog(this);
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .CreateForSubscriber(_settings.ConnectionString, _settings.PaymentRequestsExchangeName, "payinvoice-notifications")
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
                await _notificationsService.NotifyStatusUpdateAsync(
                    Mapper.Map<InvoiceStatusUpdateNotification>(message));
            }
            catch (Exception ex)
            {
                _log.ErrorWithDetails(ex, message);
            }
        }
    }
}
