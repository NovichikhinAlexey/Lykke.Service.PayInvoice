using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.PayInvoice.Models.Invoice
{
    public class CancelInvoiceDisputeRequest
    {
        [Required]
        public string InvoiceId { get; set; }
        [Required]
        public string EmployeeId { get; set; }
    }
}
