using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using System.Net;
using VizoMenuAPIv3.Data;
using VizoMenuAPIv3.Models;

namespace VizoMenuAPIv3.Functions
{
    public class VenueHoursFunctions
    {
        private readonly VizoMenuDbContext _db;

        public VenueHoursFunctions(VizoMenuDbContext db)
        {
            _db = db;
        }

        // 📆 Get all hours for a venue
        [Function("GetVenueHours")]
        public async Task<HttpResponseData> GetVenueHoursAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "venues/{venueId}/hours")] HttpRequestData req,
            Guid venueId,
            FunctionContext context)
        {
            var hours = await _db.VenueHours
                .Where(h => h.VenueId == venueId)
                .OrderBy(h => h.DayOfWeek)
                .ToListAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(hours);
            return response;
        }

        // 🔁 Replace all hours for a venue
        [Function("SetVenueHours")]
        public async Task<HttpResponseData> SetVenueHoursAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "venues/{venueId}/hours")] HttpRequestData req,
            Guid venueId,
            FunctionContext context)
        {
            var incomingHours = await req.ReadFromJsonAsync<List<VenueHour>>();
            if (incomingHours == null || incomingHours.Count != 7)
                return req.CreateResponse(HttpStatusCode.BadRequest);

            var existing = _db.VenueHours.Where(h => h.VenueId == venueId);
            _db.VenueHours.RemoveRange(existing);

            foreach (var hour in incomingHours)
            {
                hour.Id = Guid.NewGuid();
                hour.VenueId = venueId;
            }

            await _db.VenueHours.AddRangeAsync(incomingHours);
            await _db.SaveChangesAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(incomingHours);
            return response;
        }

        // ✏️ Update a specific day's hour
        [Function("UpdateVenueHour")]
        public async Task<HttpResponseData> UpdateVenueHourAsync(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "venues/{venueId}/hours/{dayOfWeek:int}")] HttpRequestData req,
            Guid venueId, int dayOfWeek,
            FunctionContext context)
        {
            var update = await req.ReadFromJsonAsync<VenueHour>();
            if (update == null) return req.CreateResponse(HttpStatusCode.BadRequest);

            var hour = await _db.VenueHours
                .FirstOrDefaultAsync(h => h.VenueId == venueId && h.DayOfWeek == dayOfWeek);

            if (hour == null) return req.CreateResponse(HttpStatusCode.NotFound);

            hour.OpenTime = update.OpenTime;
            hour.CloseTime = update.CloseTime;
            hour.IsClosed = update.IsClosed;

            await _db.SaveChangesAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(hour);
            return response;
        }

        // ❌ Mark closed for a given day
        [Function("CloseVenueDay")]
        public async Task<HttpResponseData> CloseVenueDayAsync(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "venues/{venueId}/hours/{dayOfWeek:int}/close")] HttpRequestData req,
            Guid venueId, int dayOfWeek,
            FunctionContext context)
        {
            var hour = await _db.VenueHours
                .FirstOrDefaultAsync(h => h.VenueId == venueId && h.DayOfWeek == dayOfWeek);

            if (hour == null) return req.CreateResponse(HttpStatusCode.NotFound);

            hour.IsClosed = true;
            hour.OpenTime = null;
            hour.CloseTime = null;

            await _db.SaveChangesAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync("Day closed");
            return response;
        }

        // ✅ Reopen a day (manual override)
        [Function("ReopenVenueDay")]
        public async Task<HttpResponseData> ReopenVenueDayAsync(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "venues/{venueId}/hours/{dayOfWeek:int}/reopen")] HttpRequestData req,
            Guid venueId, int dayOfWeek,
            FunctionContext context)
        {
            var update = await req.ReadFromJsonAsync<VenueHour>();
            if (update == null || update.OpenTime == null || update.CloseTime == null)
                return req.CreateResponse(HttpStatusCode.BadRequest);

            var hour = await _db.VenueHours
                .FirstOrDefaultAsync(h => h.VenueId == venueId && h.DayOfWeek == dayOfWeek);

            if (hour == null) return req.CreateResponse(HttpStatusCode.NotFound);

            hour.IsClosed = false;
            hour.OpenTime = update.OpenTime;
            hour.CloseTime = update.CloseTime;

            await _db.SaveChangesAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync("Day reopened");
            return response;
        }
    }
}
