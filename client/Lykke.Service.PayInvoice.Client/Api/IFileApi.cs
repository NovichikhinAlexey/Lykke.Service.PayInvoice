using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Client.Models.File;
using Refit;

namespace Lykke.Service.PayInvoice.Client.Api
{
    public interface IFileApi
    {
        [Get("/api/invoices/{invoiceId}/files")]
        Task<IEnumerable<FileInfoModel>> GetAsync(string invoiceId);

        [Get("/api/invoices/{invoiceId}/files/{fileId}")]
        Task<HttpResponseMessage> GetAsync(string invoiceId, string fileId);

        [Multipart]
        [Post("/api/invoices/{invoiceId}/files")]
        Task UploadAsync(string invoiceId, [AliasAs("file")] StreamPart stream);
    }
}
