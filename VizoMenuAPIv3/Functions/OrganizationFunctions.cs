using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;
using VizoMenuAPIv3.Data;
using VizoMenuAPIv3.Models;


public class OrganizationFunctions
{
    private readonly VizoMenuDbContext _db;

    public OrganizationFunctions(VizoMenuDbContext db)
    {
        _db = db;
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

        var dto = await req.ReadFromJsonAsync<OrganizationCreateDto>();

        if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Invalid request. Name is required.");
            return badResponse;
        }

        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = dto.Name
        };

        _db.Organizations.Add(organization);
        await _db.SaveChangesAsync();

        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(organization);

        return response;
    }

}
