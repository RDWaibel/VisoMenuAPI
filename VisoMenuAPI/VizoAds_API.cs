using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using VisoMenuAPI.data;

namespace VisoMenuAPI
{
    public static class VizoAds_API
    {
        [FunctionName("LocationAds")]
        public static async Task<IActionResult> getLocationAds(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ads/{locid}")] HttpRequest req,
           string locid, ILogger log)
        {
            //Ads assigned to a location
            log.LogInformation("Getting the ads for a location");
            sql_Procedures dta = new sql_Procedures();
            Guid LocID = Guid.Parse(locid);

            if (LocID.ToString().Length > 0)
            {
                List<LocationAds> locMenu = await dta.rtn_LocationAds(LocID.ToString(), log);

                string jsonDta = JsonConvert.SerializeObject(locMenu);
                string responseMessage = string.IsNullOrEmpty(jsonDta)
                    ? "This HTTP triggered function executed successfully, however, no data was found."
                    : $"{jsonDta}";
                return new OkObjectResult(responseMessage);
            }
            else
            {
                log.LogError("No location provided.");
                return new BadRequestErrorMessageResult("unable to process request");
            }
        }

        [FunctionName("ViewedAd")]
        public static async Task<IActionResult> viewedAd(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "ViewedAd/{locid}/{adID}")] HttpRequest req,
           string locid, int adID,  ILogger log
            )
        {
            log.LogInformation("Update when an ad image is viewed");
            Guid LocGUID = Guid.Parse(locid);
            sql_Procedures dta = new sql_Procedures();
            if (LocGUID.ToString().Length > 0 && adID > 0)
            {
                await dta.UpdateAdViewed(LocGUID.ToString(), adID, log);
                return new OkResult();            
            }
            else
            {
                log.LogError("Missing required data");
                return new BadRequestErrorMessageResult("Missing required data, unable to process request");
            }
        }
    }
}
