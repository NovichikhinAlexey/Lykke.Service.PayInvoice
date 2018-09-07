using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.PayInvoice.Settings.Clients
{
    public class EmailPartnerRouterServiceClientSettings
    {
        [HttpCheck("api/isalive")]
        public string ServiceUrl { get; set; }
    }
}
