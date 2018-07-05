using System.ComponentModel.DataAnnotations;
using Lykke.Service.PayInvoice.Validation;

namespace Lykke.Service.PayInvoice.Models.Employee
{
    public class CreateEmployeeModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }

        [Required]
        [RowKey]
        public string MerchantId { get; set; }

        public bool IsBlocked { get; set; }

        public bool IsInternalSupervisor { get; set; }
    }
}
