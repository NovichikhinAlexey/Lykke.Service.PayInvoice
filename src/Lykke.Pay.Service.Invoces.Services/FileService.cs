using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Pay.Service.Invoces.Core.Domain;
using Lykke.Pay.Service.Invoces.Core.Repositories;
using Lykke.Pay.Service.Invoces.Core.Services;

namespace Lykke.Pay.Service.Invoces.Services
{
    public class FileService : IFileService
    {
        private readonly IFileInfoRepository _fileInfoRepository;
        private readonly IFileRepository _fileRepository;

        public FileService(
            IFileInfoRepository fileInfoRepository,
            IFileRepository fileRepository)
        {
            _fileInfoRepository = fileInfoRepository;
            _fileRepository = fileRepository;
        }

        public async Task<IEnumerable<IFileInfo>> GetInfoAsync(string invoiceId)
        {
            return await _fileInfoRepository.GetAsync(invoiceId);
        }

        public async Task<IFileInfo> GetInfoAsync(string invoiceId, string fileId)
        {
            return await _fileInfoRepository.GetAsync(invoiceId, fileId);
        }

        public async Task<byte[]> GetFileAsync(string fileId)
        {
            return await _fileRepository.GetAsync(fileId);
        }

        public async Task SaveAsync(IFileInfo fileInfo, byte[] content)
        {
            string fileId = await _fileInfoRepository.InsertAsync(fileInfo);
            await _fileRepository.InsertAsync(content, fileId);
        }
    }
}
