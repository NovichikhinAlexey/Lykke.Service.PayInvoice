using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Lykke.Pay.Service.Invoces.Client.Models.File;
using Refit;

namespace Lykke.Pay.Service.Invoces.Client.Api
{
    public interface IFileApi
    {
        [Get("/api/file/{fileId}/invoice/{invoiceId}")]
        Task<FileInfoModel> GetInfoAsync(string invoiceId, string fileId);

        [Get("/api/file/invoice/{invoiceId}")]
        Task<IEnumerable<FileInfoModel>> GetInfoByInvoiceAsync(string invoiceId);

        [Get("/api/file/{fileId}/invoice/{invoiceId}/content")]
        Task<HttpResponseMessage> GetFileAsync(string invoiceId, string fileId);

        [Multipart]
        [Post("/api/file/invoice/{invoiceId}")]
        Task UploadFileAsync(string invoiceId, [AliasAs("file")] StreamPart stream);
    }
}
