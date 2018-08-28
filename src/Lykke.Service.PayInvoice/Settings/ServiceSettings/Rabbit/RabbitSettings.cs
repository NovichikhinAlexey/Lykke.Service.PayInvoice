using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.PayInvoice.Settings.ServiceSettings.Rabbit
{
    public class RabbitSettings
    {
        [AmqpCheck]
        public string ConnectionString { get; set; }

        [AmqpCheck]
        public string SagasConnectionString { get; set; }
        
        public string PaymentRequestsExchangeName { get; set; }
    }
}
