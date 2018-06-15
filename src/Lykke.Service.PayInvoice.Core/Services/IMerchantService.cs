using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.PayInvoice.Core.Services
{
    public interface IMerchantService
    {
        Task<IReadOnlyList<string>> GetGroupMerchants(string merchantId);
    }
}
