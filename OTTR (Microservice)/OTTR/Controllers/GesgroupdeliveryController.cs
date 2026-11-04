using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OTTR.Models;
using OTTR.Services;

namespace OTTR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GesgroupdeliveryController : BaseController
    {
        private readonly IGesgroupdeliveryService gesgroupdeliveryService;
        private readonly ILogger<GesgroupdeliveryController> logger;
        public GesgroupdeliveryController(IGesgroupdeliveryService gesgroupdeliveryService,ILoggerFactory logger)
        {
            this.gesgroupdeliveryService = gesgroupdeliveryService;
            this.logger = logger.CreateLogger<GesgroupdeliveryController>();
        }

        //Get the List of All Regions
        [HttpGet]
        public async Task<IActionResult> Read(String RoleList)
        {
            try
            {
                ResponseResource responseResource = new ResponseResource();
                if (String.IsNullOrEmpty(RoleList))
                {
                    responseResource.code = 401;
                    responseResource.data = "Unauthorized Access";
                    return Ok(responseResource);
                }
                List<string> listofFERoles = Newtonsoft.Json.Linq.JArray.Parse(RoleList).ToObject<IList<string>>().ToList();
                bool isRoleAuthorized = listofFERoles.Any(s => s.Contains("OTTR-"));
                if (!isRoleAuthorized)
                {
                    responseResource.code = 401;
                    responseResource.data = "Unauthorized Access";
                    return Ok(responseResource);
                }
                var listOfRegion = await gesgroupdeliveryService.GetListOfRegion();
                if (listOfRegion.Any())
                {
                    return Ok(listOfRegion);
                }
                else
                {
                    return Error("No data available");
                }
            }
            catch (Exception e)
            {
                logger.LogError("Error occured", e);
                return Error("Error occured" + e.Message);
            }
        }
    }
}
