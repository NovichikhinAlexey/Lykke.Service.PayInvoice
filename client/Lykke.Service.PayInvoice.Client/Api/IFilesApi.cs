using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Lykke.Service.PayInvoice.Client.Models.File;
using Refit;

namespace Lykke.Service.PayInvoice.Client.Api
{
    internal interface IFilesApi
    {
        [Get("/api/invoices/{invoiceId}/files")]
        Task<IEnumerable<FileInfoModel>> GetAllAsync(string invoiceId);

        [Get("/api/invoices/{invoiceId}/files/{fileId}")]
        Task<HttpResponseMessage> GetAsync(string invoiceId, string fileId);

        [Multipart]
        [Post("/api/invoices/{invoiceId}/files")]
        Task UploadAsync(string invoiceId, [AliasAs("file")] StreamPart stream);

        [Delete("/api/invoices/{invoiceId}/files/{fileId}")]
        Task<HttpResponseMessage> DeleteAsync(string invoiceId, string fileId);
    }
}
