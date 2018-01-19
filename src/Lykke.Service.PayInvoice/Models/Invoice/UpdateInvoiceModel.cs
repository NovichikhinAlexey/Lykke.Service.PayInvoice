using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.PayInvoice.Models.Invoice
{
    public class UpdateInvoiceModel : NewInvoiceModel
    {
        [Required]
        public string InvoiceId { get; set; }
    }
}
