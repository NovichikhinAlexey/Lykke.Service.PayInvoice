using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.PayInvoice.Settings.MonitoringService
{
    public class MonitoringServiceClientSettings
    {
        [HttpCheck("api/isalive")]
        public string MonitoringServiceUrl { get; set; }
    }
}
