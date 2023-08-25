using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VisoMenuAPI.data;
using System.Web.Http;

namespace VisoMenuAPI
{
    public static class ARF
    {
        [FunctionName("ARF")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "arf")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("ARF message recieved.");
            arfData dta = new arfData();
            arfData.arfMessage myclass = await System.Text.Json.JsonSerializer.DeserializeAsync<arfData.arfMessage>(req.Body);

            try
            {
                await dta.Insert_Arf_Message(myclass, log);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Message not saved");
                return new InternalServerErrorResult();
            }

            return new OkResult();
        }
    }
}
