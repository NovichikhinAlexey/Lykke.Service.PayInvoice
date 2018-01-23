using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Models.File;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.PayInvoice.Controllers
{
    [Route("api")]
    public class FilesController : Controller
    {
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// Returns collection of file info for invoice.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns>The collection of file info.</returns>
        /// <response code="200">The collection of file info.</response>
        [HttpGet]
        [Route("invoices/{invoiceId}/files")]
        [SwaggerOperation("FileGetAll")]
        [ProducesResponseType(typeof(IEnumerable<FileInfoModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllAsync(string invoiceId)
        {
            IEnumerable<IFileInfo> fileInfos = await _fileService.GetInfoAsync(invoiceId);

            return Ok(Mapper.Map<List<FileInfoModel>>(fileInfos));
        }

        /// <summary>
        /// Returns file.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="fileId">The file id.</param>
        /// <returns>The file stream.</returns>
        /// <response code="200">The file stream.</response>
        /// <response code="404">File info not found.</response>
        [HttpGet]
        [Route("invoices/{invoiceId}/files/{fileId}")]
        [SwaggerOperation("FileGetContent")]
        [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetContentAsync(string invoiceId, string fileId)
        {
            IFileInfo fileInfo = await _fileService.GetInfoAsync(invoiceId, fileId);

            if (fileInfo == null)
                return NotFound();

            byte[] content = await _fileService.GetFileAsync(fileId);

            return File(new MemoryStream(content), fileInfo.Type, fileInfo.Name);
        }

        /// <summary>
        /// Saves file.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="file">The file.</param>
        /// <response code="204">File successfuly uploaded.</response>
        /// <response code="400">Invalid file.</response>
        [HttpPost]
        [Route("invoices/{invoiceId}/files")]
        [SwaggerOperation("FileUpload")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UploadAsync(string invoiceId, IFormFile file)
        {
            if(file == null || file.Length == 0)
                return BadRequest("Invalid file");

            var fileInfo = new Core.Domain.FileInfo
            {
                InvoiceId = invoiceId,
                Type = file.ContentType,
                Name = file.FileName,
                Size = (int) file.Length
            };

            byte[] content;

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                content = ms.ToArray();
            }

            await _fileService.SaveAsync(fileInfo, content);

            return NoContent();
        }
    }
}
