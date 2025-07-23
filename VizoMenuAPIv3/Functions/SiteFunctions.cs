using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VizoMenuAPIv3.Data;
using VizoMenuAPIv3.Models;

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
        [Function("AddSite")]
        public async Task<HttpResponseData> AddSite(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = "venues/{venueId}/sites")] HttpRequestData req,
    Guid venueId,
    FunctionContext context)
        {
            var logger = context.GetLogger("AddSite");
            var newSite = await req.ReadFromJsonAsync<Site>();

            if (newSite == null || string.IsNullOrWhiteSpace(newSite.SiteName))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            newSite.Id = Guid.NewGuid();
            newSite.VenueId = venueId;
            newSite.EnteredUTC = DateTime.UtcNow;
            
            _db.Sites.Add(newSite);
            await _db.SaveChangesAsync();

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(newSite);
            return response;
        }

        [Function("UpdateSite")]
        public async Task<HttpResponseData> UpdateSite(
    [HttpTrigger(AuthorizationLevel.Function, "put", Route = "sites/{siteId}")] HttpRequestData req,
    Guid siteId,
    FunctionContext context)
        {
            var logger = context.GetLogger("UpdateSite");
            var updateData = await req.ReadFromJsonAsync<Site>();

            var site = await _db.Sites.FindAsync(siteId);
            if (site == null) return req.CreateResponse(HttpStatusCode.NotFound);

            site.SiteName = updateData.SiteName;
            site.Description = updateData.Description;
            await _db.SaveChangesAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(site);
            return response;
        }

        [Function("ToggleSiteActive")]
        public async Task<HttpResponseData> ToggleSiteActive(
    [HttpTrigger(AuthorizationLevel.Function, "patch", Route = "sites/{siteId}/toggle")] HttpRequestData req,
    Guid siteId,
    FunctionContext context)
        {
            var logger = context.GetLogger("ToggleSiteActive");
            var body = await req.ReadFromJsonAsync<ToggleRequest>();
            if (body == null)
                return req.CreateResponse(HttpStatusCode.BadRequest);

            var site = await _db.Sites.FindAsync(siteId);
            if (site == null) return req.CreateResponse(HttpStatusCode.NotFound);

            site.IsActive = !site.IsActive;
            site.ActiveChangedBy = body.ChangedBy;
            site.ActiveChangedUTC = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(site);
            return response;
        }

        public class ToggleRequest
        {
            public string ChangedBy{ get; set; }
        }

        [Function("GetSiteById")]
        public async Task<HttpResponseData> GetSiteById(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "sites/{siteId}")] HttpRequestData req,
    Guid siteId,
    FunctionContext context)
        {
            var logger = context.GetLogger("GetSiteById");

            var site = await _db.Sites
                .Include(s => s.Venue) // optional if you need venue info
                .FirstOrDefaultAsync(s => s.Id == siteId);

            if (site == null)
            {
                var notFound = req.CreateResponse(HttpStatusCode.NotFound);
                await notFound.WriteStringAsync("Site not found.");
                return notFound;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(site);
            return response;
        }

    }
}
