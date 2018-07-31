using System.ComponentModel.DataAnnotations;
using LykkePay.Common.Validation;

namespace Lykke.Service.PayInvoice.Models.Invoice
{
    public class CancelInvoiceDisputeRequest
    {
        [Required]
        [Guid]
        public string InvoiceId { get; set; }
        [Required]
        [Guid]
        public string EmployeeId { get; set; }
    }
}
