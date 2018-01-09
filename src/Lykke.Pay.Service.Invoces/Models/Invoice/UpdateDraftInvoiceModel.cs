using System.ComponentModel.DataAnnotations;

namespace Lykke.Pay.Service.Invoces.Models.Invoice
{
    public class UpdateDraftInvoiceModel : NewDraftInvoiceModel
    {
        [Required]
        public string InvoiceId { get; set; }
    }
}
