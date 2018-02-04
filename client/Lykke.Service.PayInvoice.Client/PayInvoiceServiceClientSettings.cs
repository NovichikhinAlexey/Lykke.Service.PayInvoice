
namespace Lykke.Service.PayInvoice.Client
{
    /// <summary>
    /// Settings for <see cref="IPayInvoiceClient"/>.
    /// </summary>
    public class PayInvoiceServiceClientSettings
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PayInvoiceServiceClientSettings"/>.
        /// </summary>
        public PayInvoiceServiceClientSettings()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="PayInvoiceServiceClientSettings"/> with service url.
        /// </summary>
        public PayInvoiceServiceClientSettings(string serviceUrl)
        {
            ServiceUrl = serviceUrl;
        }

        /// <summary>
        /// The service url.
        /// </summary>
        public string ServiceUrl { get; set; }
    }
}
