using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Client.Models.File;
using Refit;

namespace Lykke.Service.PayInvoice.Client.Api
{
    public interface IFileApi
    {
        [Get("/api/file/invoice/{invoiceId}/{fileId}")]
        Task<FileInfoModel> GetAsync(string invoiceId, string fileId);

        [Get("/api/file/invoice/{invoiceId}")]
        Task<IEnumerable<FileInfoModel>> GetAllAsync(string invoiceId);

        [Get("/api/file/invoice/{invoiceId}/{fileId}/content")]
        Task<HttpResponseMessage> GetContentAsync(string invoiceId, string fileId);

        [Multipart]
        [Post("/api/file/invoice/{invoiceId}")]
        Task UploadAsync(string invoiceId, [AliasAs("file")] StreamPart stream);
    }
}
