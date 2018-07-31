using System;
using System.ComponentModel.DataAnnotations;
using LykkePay.Common.Validation;

namespace Lykke.Service.PayInvoice.Models.Invoice
{
    public class UpdateInvoiceModel : CreateInvoiceModel
    {
        [Required]
        [Guid]
        public string Id { get; set; }
    }
}
