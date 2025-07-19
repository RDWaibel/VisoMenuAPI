using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;
using VizoMenuAPIv3.Data;
using VizoMenuAPIv3.Models;
using VizoMenuAPIv3.Services;


public class OrganizationFunctions
{
    private readonly VizoMenuDbContext _db;
    private readonly JwtService _jwt;

    public OrganizationFunctions(VizoMenuDbContext db, JwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }


    [Function("GetOrganizations")]
    public async Task<HttpResponseData> GetOrganizationsAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "organizations")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("GetOrganizations");
        logger.LogInformation("Processing GetOrganizations request.");

        var organizations = await _db.Organizations
        .AsNoTracking()
        .ToListAsync();

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(organizations);

        return response;
    }

    [Function("CreateOrganization")]
    public async Task<HttpResponseData> CreateOrganizationAsync(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = "organizations")] HttpRequestData req,
    FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("CreateOrganization");
        logger.LogInformation("Received POST to create a new organization.");

        // Extract JWT token from Authorization header
        var authHeader = req.Headers.GetValues("Authorization").FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            return req.CreateResponse(HttpStatusCode.Unauthorized);

        var token = authHeader.Substring("Bearer ".Length);
        var principal = _jwt.ValidateToken(token);
        if (principal == null || !principal.IsInRole("SuperAdmin"))
            return req.CreateResponse(HttpStatusCode.Forbidden);

        var dto = await req.ReadFromJsonAsync<OrganizationDto>();
        if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Invalid request. Name is required.");
            return badResponse;
        }

        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Address = dto.Address,
            City = dto.City,
            State = dto.State,
            ZipCode = dto.ZipCode,
            ContactPhone = dto.ContactPhone,
            ContactEmail = dto.ContactEmail,
            EnteredUTC = DateTime.UtcNow,
            EnteredBy = principal.Identity?.Name ?? "unknown"
        };

        _db.Organizations.Add(organization);
        await _db.SaveChangesAsync();

        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(organization);

        return response;
    }

    [Function("UpdateOrganization")]
    public async Task<HttpResponseData> UpdateOrganizationAsync(
    [HttpTrigger(AuthorizationLevel.Function, "put", Route = "organizations/{id}")] HttpRequestData req,
    FunctionContext context,
    string id)
    {
        var logger = context.GetLogger("UpdateOrganization");
        var request = await req.ReadFromJsonAsync<OrganizationDto>();
        if (request == null || string.IsNullOrEmpty(id))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Invalid request body or missing ID.");
            return bad;
        }

        var guid = Guid.Parse(id);
        var org = await _db.Organizations.FindAsync(guid);
        if (org == null)
            return req.CreateResponse(HttpStatusCode.NotFound);

        // Update fields
        org.Name = request.Name;
        org.Address = request.Address;
        org.City = request.City;
        org.State = request.State;
        org.ZipCode = request.ZipCode;
        org.ContactPhone = request.ContactPhone;
        org.ContactEmail = request.ContactEmail;
        //org.EnteredBy = request.EnteredBy;
        //org.EnteredUTC = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return req.CreateResponse(HttpStatusCode.NoContent); // 204 success
    }


}
