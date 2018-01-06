using System.Threading.Tasks;
using AzureStorage;
using Lykke.Pay.Service.Invoces.Core.Repositories;

namespace Lykke.Pay.Service.Invoces.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly IBlobStorage _storage;

        private const string ContainerName = "invoicefiles";

        public FileRepository(IBlobStorage storage)
        {
            _storage = storage;
        }

        public async Task<byte[]> GetAsync(string id)
        {
            using (var stream = await _storage.GetAsync(ContainerName, id))
            {
                byte[] buffer = new byte[stream.Length];
                await stream.ReadAsync(buffer, 0, (int)stream.Length);
                return buffer;
            }
        }

        public async Task<string> InsertAsync(byte[] file, string id)
        {
            await _storage.SaveBlobAsync(ContainerName, id, file);
            return _storage.GetBlobUrl(ContainerName, id);
        }

        public async Task DeleteAsync(string id)
        {
            await _storage.DelBlobAsync(ContainerName, id);
        }
    }
}
