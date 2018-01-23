using System;
using Autofac;
using Common;
using Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using System.Threading.Tasks;
using Lykke.Service.PayInternal.Contract;
using Lykke.Service.PayInvoice.Settings.ServiceSettings.Rabbit;

namespace Lykke.Service.PayInvoice.Rabbit.Subscribers
{
    public class TransactionUpdatesSubscriber : IStartable, IStopable
    {
        private readonly RabbitSettings _settings;
        private readonly ILog _log;
        private RabbitMqSubscriber<TransactionUpdateMessage> _subscriber;

        public TransactionUpdatesSubscriber(
            RabbitSettings settings,
            ILog log)
        {
            _log = log;
            _settings = settings;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .CreateForSubscriber(_settings.ConnectionString, _settings.TransactionUpdatesExchangeName, "payinvoice")
                .MakeDurable();

            settings.DeadLetterExchangeName = null;

            _subscriber = new RabbitMqSubscriber<TransactionUpdateMessage>(settings,
                    new ResilientErrorHandlingStrategy(_log, settings,
                        TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<TransactionUpdateMessage>())
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

        private async Task ProcessMessageAsync(TransactionUpdateMessage message)
        {
            try
            {
                // TODO: Set invoice status

                await _log.WriteInfoAsync(nameof(TransactionUpdatesSubscriber), nameof(ProcessMessageAsync),
                    message.ToJson(), "Transaction updated message processed");
            }
            catch (Exception exception)
            {
                await _log.WriteErrorAsync(nameof(TransactionUpdatesSubscriber), nameof(ProcessMessageAsync),
                    message.ToJson(), exception);
                throw;
            }
        }
    }
}