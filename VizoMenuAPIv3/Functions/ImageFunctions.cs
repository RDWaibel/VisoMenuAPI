using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VizoMenuAPIv3.Data;
using VizoMenuAPIv3.Services;

namespace VizoMenuAPIv3.Functions
{
    public class ImageFunctions
    {
        //same-o page-o™
        private readonly ImageImportService _imageImportService;
        private readonly VizoMenuDbContext _db;
        private readonly AzureBlobService _blobService;

        public ImageFunctions(ImageImportService imageImportService, VizoMenuDbContext db, AzureBlobService blobService)
        {
            _imageImportService = imageImportService;
            _db = db;
            _blobService = blobService;
        }

        [Function("GetImages")]
        public async Task<HttpResponseData> GetImages(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "images")] HttpRequestData req,
    FunctionContext context)
        {
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            string? container = query["container"];
            string? path = query["path"];

            var images = await _db.Images
                .Where(i =>
                    (container == null || i.BlobContainer == container) &&
                    (path == null || i.ImagePath == path))
                .OrderBy(i => i.FileName)
                .ToListAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            var result = images.Select(i => new
            {
                i.ImageID,
                i.Description,
                i.BlobContainer,
                i.ImagePath,
                i.FileName,
                ImageUrl = _blobService.GetImageUrl(i.BlobContainer, $"{i.ImagePath}/{i.FileName}")
            });

            await response.WriteAsJsonAsync(result);
            return response;
        }


        #region CSV imports
        [Function("ImportImages")]
        public async Task<HttpResponseData> ImportImagesAsync(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = "images/import")] HttpRequestData req,
    FunctionContext context)
        {
            var logger = context.GetLogger("ImportImages");

            try
            {
                var contentType = req.Headers.GetValues("Content-Type").FirstOrDefault();
                var boundary = HeaderUtilities.RemoveQuotes(MediaTypeHeaderValue.Parse(contentType).Boundary).Value;

                var reader = new MultipartReader(boundary, req.Body);
                MultipartSection? section;

                while ((section = await reader.ReadNextSectionAsync()) != null)
                {
                    var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);
                    if (hasContentDispositionHeader && contentDisposition?.DispositionType == "form-data" && contentDisposition.FileName.HasValue)
                    {
                        using var stream = new MemoryStream();
                        await section.Body.CopyToAsync(stream);
                        stream.Position = 0;

                        var importedImages = await _imageImportService.ImportImagesFromCsvAsync(stream);

                        var response = req.CreateResponse(HttpStatusCode.OK);
                        await response.WriteAsJsonAsync(importedImages);
                        return response;
                    }
                }

                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync("No file section found.");
                return bad;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CSV image import failed.");
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteStringAsync("Import failed: " + ex.Message);
                return error;
            }
        }

        #endregion
    }
}
