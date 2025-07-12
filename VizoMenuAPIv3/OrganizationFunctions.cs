using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;


public class OrganizationFunctions
{
    public OrganizationFunctions()
    {
        // If you want to inject services like DbContext, you'll do it here later
    }

    [Function("GetOrganizations")]
    public async Task<HttpResponseData> GetOrganizationsAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "organizations")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("GetOrganizations");
        logger.LogInformation("Processing GetOrganizations request.");

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new { message = "It works!" });

        return response;
    }
}
