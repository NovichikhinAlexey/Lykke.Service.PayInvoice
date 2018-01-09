using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Pay.Service.Invoces.Core.Domain;
using Lykke.Pay.Service.Invoces.Core.Services;
using Lykke.Pay.Service.Invoces.Models.File;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Pay.Service.Invoces.Controllers
{
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// Returns file info.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="fileId">The file id.</param>
        /// <returns>The file info.</returns>
        /// <response code="200">The file info.</response>
        /// <response code="404">File info not found.</response>
        [HttpGet]
        [Route("invoice/{invoiceId}/{fileId}")]
        [SwaggerOperation("FileGet")]
        [ProducesResponseType(typeof(FileInfoModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAsync(string invoiceId, string fileId)
        {
            IFileInfo fileInfo = await _fileService.GetInfoAsync(invoiceId, fileId);

            if (fileInfo == null)
                return NotFound();

            return Ok(Mapper.Map<FileInfoModel>(fileInfo));
        }

        /// <summary>
        /// Returns collection of file info for invoice.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns>The collection of file info.</returns>
        /// <response code="200">The collection of file info.</response>
        [HttpGet]
        [Route("invoice/{invoiceId}")]
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
        [Route("invoice/{invoiceId}/{fileId}/content")]
        [SwaggerOperation("FileGetContent")]
        [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetContentAsync(string invoiceId, string fileId)
        {
            IFileInfo fileInfo = await _fileService.GetInfoAsync(invoiceId, fileId);

            if (fileInfo == null)
                return NotFound();

            byte[] content = await _fileService.GetFileAsync(fileId);

            return File(new MemoryStream(content), fileInfo.FileMetaType, fileInfo.FileName);
        }

        /// <summary>
        /// Saves file.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="file">The file.</param>
        /// <response code="204">File successfuly uploaded.</response>
        /// <response code="400">Invalid file.</response>
        [HttpPost]
        [Route("invoice/{invoiceId}")]
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
                FileMetaType = file.ContentType,
                FileName = file.FileName,
                FileSize = (int) file.Length
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
