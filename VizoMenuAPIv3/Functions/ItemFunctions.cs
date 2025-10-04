using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VizoMenuAPIv3.Data;
using VizoMenuAPIv3.Models;
using VizoMenuAPIv3.Services;

namespace VizoMenuAPIv3.Functions
{
    public class ItemFunctions
    {

        private readonly ItemImportService _itemImportService;
        private readonly VizoMenuDbContext _db;

        public ItemFunctions(ItemImportService itemImportService, VizoMenuDbContext db)
        {
            _itemImportService = itemImportService;
            _db = db;
        }

        [Function("GetItems")]
        public async Task<HttpResponseData> GetItems(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "items")] HttpRequestData req,
    FunctionContext context)
        {
            var items = await _db.Items
                .OrderBy(i => i.BaseName)
                .Select(i => new
                {
                    i.Id,
                    i.BaseName,
                    i.Category,
                    i.ImageID
                })
                .ToListAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(items);
            return response;
        }


        #region Import Endpoints
        [Function("ImportItems")]
        public async Task<HttpResponseData> ImportItemsAsync(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = "items/import")] HttpRequestData req,
    FunctionContext context)
        {
            var logger = context.GetLogger("ImportItems");

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

                        var importedItems = await _itemImportService.ImportItemsFromCsvAsync(stream);
                        if(importedItems.Count == 0)
                        {
                            var responseNC = req.CreateResponse(HttpStatusCode.NoContent);
                            return responseNC;
                        }
                        else { 
                            var response = req.CreateResponse(HttpStatusCode.OK);
                            await response.WriteAsJsonAsync(importedItems);
                            return response;
                        }
                        
                    }
                }

                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync("No file section found.");
                return bad;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CSV import failed.");
                var error = req.CreateResponse(HttpStatusCode.InternalServerError);
                await error.WriteStringAsync("Import failed: " + ex.Message);
                return error;
            }
        }



        #endregion
    }
}
