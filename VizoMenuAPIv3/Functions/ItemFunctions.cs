using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VizoMenuAPIv3.Services;
using VizoMenuAPIv3.Models;

namespace VizoMenuAPIv3.Functions
{
    public class ItemFunctions
    {

        private readonly ItemImportService _itemImportService;

        public ItemFunctions(ItemImportService itemImportService)
        {
            _itemImportService = itemImportService;
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
                using var stream = new MemoryStream();
                await req.Body.CopyToAsync(stream);
                stream.Position = 0;

                var importedItems = await _itemImportService.ImportItemsFromCsvAsync(stream);

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(importedItems);
                return response;
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
