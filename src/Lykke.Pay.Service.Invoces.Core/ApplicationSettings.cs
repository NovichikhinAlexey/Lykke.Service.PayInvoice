using JetBrains.Annotations;

namespace Lykke.Pay.Service.Invoces.Core
{
    [UsedImplicitly]
    public class ApplicationSettings
    {
        public InvoicesSettings InvoicesService { get; set; }

        public SlackNotificationsSettings SlackNotifications { get; set; }

        [UsedImplicitly]
        public class InvoicesSettings
        {
            public string DbConnectionString { get; set; }
            public LogsSettings Logs { get; set; }
        }


        [UsedImplicitly]
        public class LogsSettings
        {
            public string DbConnectionString { get; set; }
        }

        [UsedImplicitly]
        public class SlackNotificationsSettings
        {
            public AzureQueueSettings AzureQueue { get; set; }

            public int ThrottlingLimitSeconds { get; set; }
        }

        [UsedImplicitly]
        public class AzureQueueSettings
        {
            public string ConnectionString { get; set; }

            public string QueueName { get; set; }
        }

        
    }

    
}