using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.PayInvoice.Models.Invoice
{
    public class UpdateDraftInvoiceModel : NewDraftInvoiceModel
    {
        [Required]
        public string InvoiceId { get; set; }
    }
}
