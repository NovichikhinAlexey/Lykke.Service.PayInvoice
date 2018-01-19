using System.Collections.Generic;

namespace Lykke.Service.PayInvoice.Client.Models
{
    public class ErrorResponse
    {
        public string ErrorMessage { get; set; }

        public Dictionary<string, List<string>> ModelErrors { get; set; }
    }
}
