using System.ComponentModel.DataAnnotations;

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
        public string MerchantId { get; set; }
        [Required]
        public bool IsBlocked { get; set; }
    }
}