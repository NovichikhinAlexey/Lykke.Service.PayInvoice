using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.Log;
using Lykke.Service.PayInvoice.Core.Domain;
using Lykke.Service.PayInvoice.Core.Extensions;
using Lykke.Service.PayInvoice.Core.Services;
using Lykke.Service.PayInvoice.Models.File;
using LykkePay.Common.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.PayInvoice.Controllers
{
    [Route("api")]
    public class FilesController : Controller
    {
        private readonly IFileService _fileService;
        private readonly ILog _log;

        public FilesController(
            IFileService fileService,
            ILogFactory logFactory)
        {
            _fileService = fileService;
            _log = logFactory.CreateLog(this);
        }

        /// <summary>
        /// Returns a collection of invoice files.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns>The collection of file info.</returns>
        /// <response code="200">The collection of file info.</response>
        /// <response code="400">Invalid model.</response>
        [HttpGet]
        [Route("invoices/{invoiceId}/files")]
        [SwaggerOperation("FileGetAll")]
        [ProducesResponseType(typeof(IEnumerable<FileInfoModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ValidateModel]
        public async Task<IActionResult> GetAllAsync([Required][Guid] string invoiceId)
        {
            IEnumerable<FileInfo> fileInfos = await _fileService.GetInfoAsync(invoiceId);

            return Ok(Mapper.Map<List<FileInfoModel>>(fileInfos));
        }

        /// <summary>
        /// Returns file content.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="fileId">The file id.</param>
        /// <returns>The file stream.</returns>
        /// <response code="200">The file stream.</response>
        /// <response code="404">File info not found.</response>
        /// <response code="400">Invalid model.</response>
        [HttpGet]
        [Route("invoices/{invoiceId}/files/{fileId}")]
        [SwaggerOperation("FileGetContent")]
        [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ValidateModel]
        public async Task<IActionResult> GetContentAsync([Required][Guid] string invoiceId, [Required][Guid] string fileId)
        {
            FileInfo fileInfo = await _fileService.GetInfoAsync(invoiceId, fileId);

            if (fileInfo == null)
            {
                string message = "File info not found.";

                _log.WarningWithDetails(message, new { invoiceId, fileId });

                return NotFound(message);
            }

            byte[] content = await _fileService.GetFileAsync(fileId);

            return File(new System.IO.MemoryStream(content), fileInfo.Type, fileInfo.Name);
        }

        /// <summary>
        /// Saves file.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="file">The file.</param>
        /// <response code="204">File successfuly uploaded.</response>
        /// <response code="400">Invalid model.</response>
        [HttpPost]
        [Route("invoices/{invoiceId}/files")]
        [SwaggerOperation("FileUpload")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ValidateModel]
        public async Task<IActionResult> UploadAsync([Required][Guid] string invoiceId, [Required] IFormFile file)
        {
            if(file.Length == 0)
                return BadRequest(ErrorResponse.Create("Invalid file"));

            var fileInfo = new FileInfo
            {
                InvoiceId = invoiceId,
                Type = file.ContentType,
                Name = file.FileName,
                Size = (int) file.Length
            };

            byte[] content;

            using (var ms = new System.IO.MemoryStream())
            {
                file.CopyTo(ms);
                content = ms.ToArray();
            }

            await _fileService.SaveAsync(fileInfo, content);

            return NoContent();
        }

        /// <summary>
        /// Deletes file.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="fileId">The file id.</param>
        /// <response code="200">File successfully deleted.</response>
        /// <response code="400">Invalid model or error occured.</response>
        [HttpDelete]
        [Route("invoices/{invoiceId}/files/{fileId}")]
        [SwaggerOperation("FileDelete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ValidateModel]
        public async Task<IActionResult> DeleteAsync([Required][Guid] string invoiceId, [Required][Guid] string fileId)
        {
            try
            {
                await _fileService.DeleteAsync(invoiceId, fileId);

                return NoContent();
            }
            catch (Exception ex)
            {
                _log.ErrorWithDetails(ex, new { invoiceId, fileId });

                return BadRequest(ex.Message);
            }
        }
    }
}
