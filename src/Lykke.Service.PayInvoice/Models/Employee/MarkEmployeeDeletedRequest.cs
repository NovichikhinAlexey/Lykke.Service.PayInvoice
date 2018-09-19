using System.ComponentModel.DataAnnotations;
using LykkePay.Common.Validation;

namespace Lykke.Service.PayInvoice.Models.Employee
{
    public class MarkEmployeeDeletedRequest
    {
        [Required]
        [RowKey]
        public string MerchantId { get; set; }

        [Required]
        [Guid]
        public string EmployeeId { get; set; }
    }
}
