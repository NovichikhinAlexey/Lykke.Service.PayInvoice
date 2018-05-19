using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.PayInvoice.Core.Domain.DataMigrations
{
    public class DataMigrationResult
    {
        public bool HasExecuted { get; set; }
        public string Info { get; set; }
    }
}
