using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using System.Net;
using VizoMenuAPIv3.Data;
using VizoMenuAPIv3.Models;

namespace VizoMenuAPIv3.Functions
{
    public class VenueFunctions
    {
        private readonly VizoMenuDbContext _db;

        public VenueFunctions(VizoMenuDbContext db)
        {
            _db = db;
        }

        // 🔍 View all venues for a given organization
        [Function("GetVenuesByOrganization")]
        public async Task<HttpResponseData> GetVenuesByOrganizationAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "organization/{orgId}/venues")] HttpRequestData req,
            Guid orgId,
            FunctionContext context)
        {
            var venues = await _db.Venues
                .Where(v => v.OrganizationId == orgId && v.DisabledUTC == null)
                .ToListAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(venues);
            return response;
        }

        // ➕ Add new venue to an organization
        [Function("AddVenue")]
        public async Task<HttpResponseData> AddVenueAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "organization/{orgId}/venues")] HttpRequestData req,
            Guid orgId,
            FunctionContext context)
        {
            var newVenue = await req.ReadFromJsonAsync<Venue>();
            if (newVenue == null || string.IsNullOrWhiteSpace(newVenue.VenueName))
                return req.CreateResponse(HttpStatusCode.BadRequest);

            newVenue.Id = Guid.NewGuid();
            newVenue.OrganizationId = orgId;
            newVenue.EnteredUTC = DateTime.UtcNow;

            _db.Venues.Add(newVenue);
            await _db.SaveChangesAsync();

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(newVenue);
            return response;
        }

        // 📋 Copy an existing venue (new ID, new name)
        [Function("CopyVenue")]
        public async Task<HttpResponseData> CopyVenueAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "venues/{venueId}/copy")] HttpRequestData req,
            Guid venueId,
            FunctionContext context)
        {
            var copyRequest = await req.ReadFromJsonAsync<Venue>();
            if (copyRequest == null || string.IsNullOrWhiteSpace(copyRequest.VenueName))
                return req.CreateResponse(HttpStatusCode.BadRequest);

            var existingVenue = await _db.Venues
                .Include(v => v.Hours)
                .FirstOrDefaultAsync(v => v.Id == venueId);

            if (existingVenue == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var newVenue = new Venue
            {
                Id = Guid.NewGuid(),
                OrganizationId = existingVenue.OrganizationId,
                VenueName = copyRequest.VenueName,
                Location = existingVenue.Location,
                Is24Hours = existingVenue.Is24Hours,
                EnteredUTC = DateTime.UtcNow
            };

            // Clone hours
            foreach (var hour in existingVenue.Hours)
            {
                newVenue.Hours.Add(new VenueHour
                {
                    Id = Guid.NewGuid(),
                    DayOfWeek = hour.DayOfWeek,
                    OpenTime = hour.OpenTime,
                    CloseTime = hour.CloseTime,
                    IsClosed = hour.IsClosed
                });
            }

            _db.Venues.Add(newVenue);
            await _db.SaveChangesAsync();

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(newVenue);
            return response;
        }

        // ✏️ Edit an existing venue
        [Function("UpdateVenue")]
        public async Task<HttpResponseData> UpdateVenueAsync(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "venues/{venueId}")] HttpRequestData req,
            Guid venueId,
            FunctionContext context)
        {
            var updateRequest = await req.ReadFromJsonAsync<Venue>();
            if (updateRequest == null)
                return req.CreateResponse(HttpStatusCode.BadRequest);

            var venue = await _db.Venues.FirstOrDefaultAsync(v => v.Id == venueId);
            if (venue == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            venue.VenueName = updateRequest.VenueName;
            venue.Location = updateRequest.Location;
            venue.Is24Hours = updateRequest.Is24Hours;

            await _db.SaveChangesAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(venue);
            return response;
        }

        [Function("GetVenueById")]
        public async Task<HttpResponseData> GetVenueById(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "venue/{venueId}")] HttpRequestData req,
    Guid venueId)
        {
            var venue = await _db.Venues.FirstOrDefaultAsync(v => v.Id == venueId);

            if (venue == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(venue);
            return response;
        }

    }
}
