using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.PayInvoice.Core.Domain.PushNotification
{
    public class InvoicePaidPushNotificationCommand : PushNotificationCommand
    {
        public string PayerMerchantName { get; set; }
        public string PaidAmount { get; set; }
    }
}
