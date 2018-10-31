using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.PayInvoice.Core.Domain.InvoiceUpdate;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Settings.ServiceSettings.Rabbit;

namespace Lykke.Service.PayInvoice.Rabbit.Publishers
{
    public class InvoiceUpdatePublisher : IInvoiceUpdatePublisher, IStartable, IStopable
    {
        private readonly RabbitSettings _rabbitSettings;
        private readonly ILogFactory _logFactory;
        private readonly ILog _log;

        private RabbitMqPublisher<Contract.Invoice.InvoiceUpdateMessage> _publisher;

        public InvoiceUpdatePublisher(
            RabbitSettings rabbitSettings,
            ILogFactory logFactory)
        {
            _rabbitSettings = rabbitSettings;
            _logFactory = logFactory;
            _log = logFactory.CreateLog(this);
        }

        public void Start()
        {
            var rabbitMqSubscriptionSettings =
                RabbitMqSubscriptionSettings
                    .ForPublisher(
                        _rabbitSettings.ConnectionString,
                        _rabbitSettings.InvoiceUpdateExchangeName)
                    .MakeDurable();

            _publisher = new RabbitMqPublisher<Contract.Invoice.InvoiceUpdateMessage>(_logFactory, rabbitMqSubscriptionSettings)
                .SetSerializer(new JsonMessageSerializer<Contract.Invoice.InvoiceUpdateMessage>())
                .SetPublishStrategy(new DefaultFanoutPublishStrategy(rabbitMqSubscriptionSettings))
                .PublishSynchronously()
                .Start();
        }

        public void Dispose()
        {
            _publisher?.Dispose();
        }

        public void Stop()
        {
            _publisher?.Stop();
        }

        public async Task PublishAsync(InvoiceUpdateMessage message)
        {
            _log.Info("Publishing invoice update message", message);
      
            await _publisher.ProduceAsync(Mapper.Map<Contract.Invoice.InvoiceUpdateMessage>(message));
        }
    }
}
