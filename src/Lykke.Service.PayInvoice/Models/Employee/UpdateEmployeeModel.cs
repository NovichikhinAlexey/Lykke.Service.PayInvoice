using System.ComponentModel.DataAnnotations;
using LykkePay.Common.Validation;

namespace Lykke.Service.PayInvoice.Models.Employee
{
    public class UpdateEmployeeModel : CreateEmployeeModel
    {
        [Required]
        [Guid]
        public string Id { get; set; }
    }
}
