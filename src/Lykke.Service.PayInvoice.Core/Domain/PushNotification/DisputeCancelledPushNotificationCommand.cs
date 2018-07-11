using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.PayInvoice.Core.Domain.PushNotification
{
    public class DisputeCancelledPushNotificationCommand
    {
        public string NotifiedMerchantId { get; set; }
        public string EmployeeFullName { get; set; }
    }
}
