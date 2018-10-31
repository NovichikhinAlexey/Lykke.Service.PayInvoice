using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.PayInvoice.Core.Extensions;
using Lykke.Service.PayInvoice.Core.Services;

namespace Lykke.Service.PayInvoice.Services
{
    // NOTE: Sometimes, startup process which is expressed explicitly is not just better, 
    // but the only way. If this is your case, use this class to manage startup.
    // For example, sometimes some state should be restored before any periodical handler will be started, 
    // or any incoming message will be processed and so on.
    // Do not forget to remove As<IStartable>() and AutoActivate() from DI registartions of services, 
    // which you want to startup explicitly.

    public class StartupManager : IStartupManager
    {
        private readonly ILog _log;
        private readonly IInvoiceUpdatePublisher _invoiceUpdatePublisher;

        public StartupManager(
            IInvoiceUpdatePublisher invoiceUpdatePublisher,
            ILogFactory logFactory)
        {
            _log = logFactory.CreateLog(this);
            _invoiceUpdatePublisher = invoiceUpdatePublisher;
        }

        public void Start()
        {
            StartComponent("Invoice update publisher", _invoiceUpdatePublisher);
        }

        private void StartComponent(string componentDisplayName, object component)
        {
            _log.Info($"Starting {componentDisplayName} ...");

            if (component is IStartable startableComponent)
            {
                startableComponent.Start();

                _log.Info($"{componentDisplayName} successfully started.");
            }
            else
            {
                _log.WarningWithDetails("Component has not been started", details: component.ToJson());
            }
        }
    }
}
