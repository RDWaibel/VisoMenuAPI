using CsvHelper;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VizoMenuAPIv3.Data;
using VizoMenuAPIv3.Models;
using VizoMenuAPIv3.Models.Import;

namespace VizoMenuAPIv3.Functions
{
    public class MenuFunctions
    {
        private readonly VizoMenuDbContext _db;

        public MenuFunctions(VizoMenuDbContext db)
        {
            _db = db;
        }


        [Function("ImportMenusFromCsv")]
        public async Task<HttpResponseData> ImportMenusFromCsvAsync(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = "menus/import")] HttpRequestData req,
    FunctionContext context)
        {
            var logger = context.GetLogger("ImportMenusFromCsv");

            try
            {
                using var reader = new StreamReader(req.Body);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var records = csv.GetRecords<MenuCsvModel>().ToList();
                var menus = new List<Menu>();

                foreach (var record in records)
                {
                    menus.Add(new Menu
                    {
                        Id = Guid.Parse(record.Id),
                        SiteId = Guid.Parse(record.SiteId),
                        MenuName = record.MenuName,
                        SortOrder = record.SortOrder,
                        DisplayText = record.DisplayText,
                        AdditionalText1 = record.AdditionalText1,
                        AdditionalText2 = record.AdditionalText2,
                        IsActive = record.IsActive,
                        EnteredBy = record.EnteredBy,
                        EnteredUTC = record.EnteredUTC,
                        LastChangedUTC = record.LastChangedUTC,
                        LastChangedBy = record.LastChangedBy,
                        ButtonImageId = string.IsNullOrWhiteSpace(record.ButtonImageId) ? null : Guid.Parse(record.ButtonImageId)
                    });
                }

                _db.Menus.AddRange(menus);
                await _db.SaveChangesAsync();

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync($"Imported {menus.Count} menus successfully.");
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error importing menus from CSV.");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Failed to import menus: {ex.Message}");
                return errorResponse;
            }
        }

    }
}
