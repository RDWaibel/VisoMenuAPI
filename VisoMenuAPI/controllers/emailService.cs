using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace VisoMenuAPI.controllers
{
    public static class emailService
    {
        [Disable]
        [FunctionName("emailService")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "send", "post", Route = "contactus")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Contact us request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
        public class ContactRequest
        {
            public string FromName { get; set; }
            [Required]
            public string fromEmail { get; set; }
            public string message { get; set; }

        }
    }
}
