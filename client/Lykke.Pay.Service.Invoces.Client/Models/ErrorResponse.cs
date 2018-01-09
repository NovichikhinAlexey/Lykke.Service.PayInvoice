using System.Collections.Generic;

namespace Lykke.Pay.Service.Invoces.Client.Models
{
    public class ErrorResponse
    {
        public string ErrorMessage { get; set; }

        public Dictionary<string, List<string>> ModelErrors { get; set; }
    }
}
