using System.ComponentModel.DataAnnotations;
using Lykke.Service.PayInvoice.Validation;

namespace Lykke.Service.PayInvoice.Models.Employee
{
    public class UpdateEmployeeModel : CreateEmployeeModel
    {
        [Required]
        [RowKey]
        public string Id { get; set; }
    }
}
