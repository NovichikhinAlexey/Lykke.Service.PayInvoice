using System.ComponentModel.DataAnnotations;
using LykkePay.Common.Validation;

namespace Lykke.Service.PayInvoice.Models.Invoice
{
    public class MarkInvoiceDisputeRequest
    {
        [Required]
        [Guid]
        public string InvoiceId { get; set; }
        [Required]
        public string Reason { get; set; }
        [Required]
        [Guid]
        public string EmployeeId { get; set; }
    }
}
