using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VizoMenuAPIv3.Data;
using Microsoft.EntityFrameworkCore;

namespace VizoMenuAPIv3.Functions
{
    public class SiteFunctions
    {
        private readonly VizoMenuDbContext _db;

        public SiteFunctions(VizoMenuDbContext db)
        {
            _db = db;
        }

        [Function("GetSitesByVenueId")]
        public async Task<HttpResponseData> GetSitesByVenueId(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "venues/{venueId}/sites")] HttpRequestData req,
    Guid venueId,
    FunctionContext context)
        {
            var logger = context.GetLogger("GetSitesByVenueId");

            var sites = await _db.Sites
                .Where(s => s.VenueId == venueId)
                .OrderBy(s => s.SiteName)
                .ToListAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(sites);
            return response;
        }
    }
}
