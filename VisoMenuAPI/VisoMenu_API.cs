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
using System.Collections.Generic;
using System.Net.Sockets;
using System.Web.Http;

namespace VisoMenuAPI
{
    public static class VisoMenu_API
    {
        //[Disable]
        [FunctionName("PullClientInfoOnly")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "{clientID}")] HttpRequest req,
            int clientID, ILogger log)
        {
            sql_Procedures dta = new sql_Procedures();
            log.LogInformation($"C# HTTP trigger function processed a request.{clientID}");

            clientData theClient = await dta.GetClientData(clientID);

            string jsonDta = JsonConvert.SerializeObject(theClient);

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(jsonDta)
                ? "This HTTP triggered function executed successfully, however, no data was found."
                : $"{jsonDta}";

            return new OkObjectResult(responseMessage);
        }

        //[Disable]
        [FunctionName("ReturnLocationFullMenu")]
        public static async Task<IActionResult> GetMenuData(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetLocationMenuData/{locid}")] HttpRequest req,
            string locid, ILogger log)
        {
            //FULL menu for location
            sql_Procedures dta = new sql_Procedures();
            Guid LocID;
            try
            {
                LocID = Guid.Parse(locid);
            }
            catch 
            {
                return new BadRequestErrorMessageResult($"Unable to process request {locid}");
        
            }
            try
            {
                List<vw_LocationsMenu> locMenu = await dta.GetLocationMenu(locid);

                string jsonDta = JsonConvert.SerializeObject(locMenu);
                string responseMessage = string.IsNullOrEmpty(jsonDta)
                    ? "This HTTP triggered function executed successfully, however, no data was found."
                    : $"{jsonDta}";
                return new OkObjectResult(responseMessage);
            }
            catch
            {
                return new BadRequestErrorMessageResult($"Unable to process request {locid}");
            }


        }


        [FunctionName("ReturnLocationMenus")]
        public static async Task<IActionResult> GetLocationMenu(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Location/{locid}")] HttpRequest req,
            string locid, ILogger log)
        {
            //Top level menues only
            log.LogInformation("Running getLocationMenu");
            sql_Procedures dta = new sql_Procedures();
            Guid LocID = Guid.Parse(locid);

            if (LocID != null)
            {
                List<locationMenuSubMenu> locMenu = await dta.rtn_LocationMenus(LocID, log);

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

        [FunctionName("ReturnSubMenu")]
        public static async Task<IActionResult> GetSubMenu(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Menu/{inMenuID}")] HttpRequest req,
            int inMenuID, ILogger log)
        {
            log.LogInformation("Running getSubMenu");
            sql_Procedures dta = new sql_Procedures();
            int menuID = inMenuID;

            if (menuID > 0)
            {
                List<SubMenus> locMenu = await dta.rtn_SubMenus(menuID);

                string jsonDta = JsonConvert.SerializeObject(locMenu);
                string responseMessage = string.IsNullOrEmpty(jsonDta)
                    ? "This HTTP triggered function executed successfully, however, no data was found."
                    : $"{jsonDta}";
                return new OkObjectResult(responseMessage);
            }
            else
            {
                log.LogError("No menuid provided.");
                return new BadRequestErrorMessageResult("unable to process request");
            }
        }


        [FunctionName("GetItems")]
        public static async Task<IActionResult> GetItems(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "subMenu/{inSubMenuID}")] HttpRequest req,
            int inSubMenuID, ILogger log)
        {
            log.LogInformation("Running GetItems API");
            sql_Procedures dta = new sql_Procedures();
            int menuID = inSubMenuID;

            if (menuID > 0)
            {
                List<MenuItems> locMenu = await dta.rtn_MenuItems(menuID, log);

                string jsonDta = JsonConvert.SerializeObject(locMenu);
                string responseMessage = string.IsNullOrEmpty(jsonDta)
                    ? "This HTTP triggered function executed successfully, however, no data was found."
                    : $"{jsonDta}";
                return new OkObjectResult(responseMessage);
            }
            else
            {
                log.LogError("No submenuid provided.");
                return new BadRequestErrorMessageResult("unable to process request");
            }
        }

        [FunctionName("GetSingleItem")]
        public static async Task<IActionResult> GetSingleItem(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "item/{inItemID}")] HttpRequest req,
            int inItemID, ILogger log)
        {
            log.LogInformation("getting a single item from database");
            sql_Procedures dta = new sql_Procedures();
            MenuItems theItem = new MenuItems();
            if (inItemID > 0)
            {
                theItem = await dta.rtn_MenuItem(inItemID, log);
                string jsonDta = JsonConvert.SerializeObject(theItem);
                string responseMessage = string.IsNullOrEmpty(jsonDta)
                    ? "This HTTP triggered function executed successfully, however, no data was found."
                    : $"{jsonDta}";
                return new OkObjectResult(responseMessage);
            }
            else
            {
                log.LogError("No item id provided.");
                return new BadRequestErrorMessageResult("unable to process request");
            }
        }
        /// <summary>
        /// return recommendations
        /// </summary>
        /// <param name="req"></param>
        /// <param name="locid"></param>
        /// <param name="inItemID"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("Recommendations")]
        public static async Task<IActionResult> GetRecommendations(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "recommend/{locid}/{inItemID}")]
            HttpRequest req, string locid, int inItemID, ILogger log)
        {
            log.LogInformation("Getting the recommendations");
            Guid testGuid;
            sql_Procedures dta = new sql_Procedures();
            List<MenuItems> theItems = new List<MenuItems>();
            if (inItemID > 0 && Guid.TryParse(locid, out testGuid) )
            {
                theItems = await dta.rtn_Recommendations(locid, inItemID, log);
                string jsonDta = JsonConvert.SerializeObject(theItems);
                string responseMessage = string.IsNullOrEmpty(jsonDta)
                    ? "This HTTP triggered function executed successfully, however, no recommendations were found."
                    : $"{jsonDta}";
                return new OkObjectResult(responseMessage);
            }
            else
            {
                log.LogError("No location or item id provided.");
                return new BadRequestErrorMessageResult("unable to process request");
            }
        }
        /// <summary>
        /// Update if the user opens the big item image
        /// </summary>
        /// <param name="req"></param>
        /// <param name="locid"></param>
        /// <param name="inItemID"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("MenuItemViewed")]
        public static async Task<IActionResult> ItemViewed(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "itemViewed/{locid}/{inItemID}")] 
            HttpRequest req, string locid, int inItemID, ILogger log)
        {
            log.LogInformation("A menu item was viewed");
            Guid testGuid;
            testGuid = Guid.Parse(locid);
            sql_Procedures dta = new sql_Procedures();
            if (inItemID > 0 && testGuid != null)
            {
                await dta.UpdateMenuItemViewed(testGuid.ToString(), inItemID, log);
                return new OkResult();
            }
            else
            {
                log.LogError("No location or item id provided.");
                return new BadRequestErrorMessageResult("No location or item id provided, unable to process request");
            }

        }
        /// <summary>
        /// update the contact us table.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("ContactUs")]
        public static async Task<IActionResult> ContactUs(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "contact")] HttpRequest req, ILogger log)
        {
            log.LogInformation("Saving contact us info");
            sql_Procedures dta = new sql_Procedures();
            Contact_Us myclass = await System.Text.Json.JsonSerializer.DeserializeAsync<Contact_Us>(req.Body);

            try
            {
                log.LogInformation("Sending to SQL");
                if (await dta.save_Contact_Us(myclass, log))
                {
                    return new OkResult();
                }
                else
                {
                    return new BadRequestErrorMessageResult("Unable to save data");
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Save contact us");
                return new BadRequestErrorMessageResult("Unable to save data");
            }
        }
    }
}
