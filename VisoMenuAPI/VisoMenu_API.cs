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
        [FunctionName("GetLocationMenuData")]
        public static async Task<IActionResult> GetMenuData(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Location_V1/{inLocid}")] HttpRequest req,
            int inLocid, ILogger log)
        {
            //FULL menu for location
            sql_Procedures dta = new sql_Procedures();
            int LocID = inLocid;

            if (LocID > 0)
            {
                List<vw_LocationsMenu> locMenu = await dta.GetLocationMenu(LocID);

                string jsonDta = JsonConvert.SerializeObject(locMenu);
                string responseMessage = string.IsNullOrEmpty(jsonDta)
                    ? "This HTTP triggered function executed successfully, however, no data was found."
                    : $"{jsonDta}";
                return new OkObjectResult(responseMessage);
            }
            else
            {

                return new BadRequestErrorMessageResult("unable to process request");
            }
        }


        [FunctionName("GetMenu")]
        public static async Task<IActionResult> GetLocationMenu(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Location/{inLocid}")] HttpRequest req,
            int inLocid, ILogger log)
        {
            //Top level menues only
            log.LogInformation("Running getLocationMenu");
            sql_Procedures dta = new sql_Procedures();
            int LocID = inLocid;

            if (LocID > 0)
            {
                List<locationMenus> locMenu = await dta.rtn_LocationMenus(LocID, log);

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

        [FunctionName("GetSubMenu")]
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

        [FunctionName("Recommendations")]
        public static async Task<IActionResult> GetRecommendations(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "recommend/{locid}/{inItemID}")]
            HttpRequest req, int locid, int inItemID, ILogger log)
        {
            log.LogInformation("Getting the recommendations");
            sql_Procedures dta = new sql_Procedures();
            List<MenuItems> theItems = new List<MenuItems>();
            if (inItemID > 0 && locid > 0)
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
        [FunctionName("MenuItemViewed")]
        public static async Task<IActionResult> ItemViewed(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "itemViewed/{locid}/{inItemID}")] 
            HttpRequest req, int locid, int inItemID, ILogger log)
        {
            log.LogInformation("A menu item was viewed");
            sql_Procedures dta = new sql_Procedures();
            if (inItemID > 0 && locid > 0)
            {
                await dta.UpdateMenuItemViewed(locid, inItemID, log);
                return new OkResult();
            }
            else
            {
                log.LogError("No location or item id provided.");
                return new BadRequestErrorMessageResult("No location or item id provided, unable to process request");
            }

        }

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
