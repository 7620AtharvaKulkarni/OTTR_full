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
    public class GESGroupProductMappingController : BaseController
    {
        private readonly IGESGroupProductMappingService gesGroupProductMappingService;
        private readonly ILogger<GESGroupProductMappingController> logger;
        public GESGroupProductMappingController(IGESGroupProductMappingService gesGroupProductMappingService, ILoggerFactory logger)
        {
            this.gesGroupProductMappingService = gesGroupProductMappingService;
            this.logger = logger.CreateLogger<GESGroupProductMappingController>();
        }

        //Get the List of All GESGroupProductMapping Information
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
                var listOfGESGroupProductMapping = await gesGroupProductMappingService.GetListOfGESGroupProductMappingInfo();
                List<string> listofFERoles = Newtonsoft.Json.Linq.JArray.Parse(RoleList).ToObject<IList<string>>().ToList();
                bool isRoleAuthorized = listofFERoles.Any(s => s.Contains("OTTR-"));
                if (!isRoleAuthorized)
                {
                    responseResource.code = 401;
                    responseResource.data = "Unauthorized Access";
                    return Ok(responseResource);
                }
                if (listOfGESGroupProductMapping.Any())
                {
                    return Ok(listOfGESGroupProductMapping);
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
