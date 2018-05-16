using Lykke.Service.PayInvoice.Core.Domain;
using System.Threading.Tasks;

namespace Lykke.Service.PayInvoice.Core.Repositories
{
    public interface IMerchantSettingRepository
    {
        Task<MerchantSetting> GetByIdAsync(string merchantId);

        Task<MerchantSetting> SetAsync(MerchantSetting merchantSettings);
    }
}
