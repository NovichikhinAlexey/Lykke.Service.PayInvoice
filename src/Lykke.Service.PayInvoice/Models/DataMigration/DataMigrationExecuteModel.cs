using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.PayInvoice.Models.DataMigration
{
    public class DataMigrationExecuteModel
    {
        [Required]
        public string MigrationName { get; set; }
    }
}
