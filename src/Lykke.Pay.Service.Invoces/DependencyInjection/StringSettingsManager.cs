using System.Threading.Tasks;
using Lykke.SettingsReader;

namespace Lykke.Pay.Service.Invoces.DependencyInjection
{
    public class StringSettingsManager : IReloadingManager<string>
    {
        private readonly string _settings;
        public StringSettingsManager(string settings)
        {
            _settings = settings;
        }
        public async Task<string> Reload()
        {
            return _settings;
        }

        public bool HasLoaded => true;
        public string CurrentValue => _settings;
    }
}