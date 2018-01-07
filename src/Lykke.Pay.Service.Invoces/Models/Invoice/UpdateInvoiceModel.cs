using System.ComponentModel.DataAnnotations;

namespace Lykke.Pay.Service.Invoces.Models.Invoice
{
    public class UpdateInvoiceModel : NewInvoiceModel
    {
        [Required]
        public string InvoiceId { get; set; }
    }
}
