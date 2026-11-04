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
    
    public class RegionController : BaseController
    {
        private readonly IRegionService regionService;
        private readonly ILogger<RegionController> logger;

        public RegionController(IRegionService regionService, ILoggerFactory logger)
        {
            this.regionService = regionService;
            this.logger = logger.CreateLogger<RegionController>();
        }

        //Get the List of All Region Information
        [HttpGet]
        public async Task<IActionResult> Read(String Region,String RoleList)
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
                logger.LogInformation("Call GetListOfRegionInfo for retrive all data of Region Entity");
                var listOfRegionInfo = await regionService.GetListOfRegionInfo();
                if (listOfRegionInfo.Any())
                {
                    return Ok(listOfRegionInfo);
                }
                else
                {
                    return NoContent("No records are availeble for Region");
                }
            }
            catch (Exception e)
            {
                logger.LogError("Error occured", e);
                return Error(e.Message, 500);
            }
        }

        //Get the List of All Region Information by Pole ID
        [HttpGet("{poleID}")]
        public async Task<IActionResult> GetByRegionID(int poleID, String RoleList)
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
                logger.LogInformation("Call GetListOfRegionByPoleId for retrive a data of Region Entity");
                var listOfRegion = await regionService.GetListOfRegionByPoleId(poleID);
                if (listOfRegion.Any())
                {
                    return Ok(listOfRegion);
                }

                return NoContent("No records are availeble for Region");
            }
            catch (Exception e)
            {
                logger.LogError("Error occured", e);
                return Error(e.Message, 500);
            }
        }

    }
}