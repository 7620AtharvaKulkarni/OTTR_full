using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OTTR.Models;
using OTTR.Services;

namespace OTTR.Controllers
{ 
    [ApiController]
    [Route("api/[controller]")]
    public class RegionCountryMappingController : BaseController
    {
        private readonly IRegionCountryMappingService regionCountryMappingService;
        private readonly ILogger<RegionCountryMappingController> logger;

        public RegionCountryMappingController(IRegionCountryMappingService regionCountryMappingService, ILoggerFactory logger)
        {
            this.regionCountryMappingService = regionCountryMappingService;
            this.logger = logger.CreateLogger<RegionCountryMappingController>();
        }

        //Get the List of All Countries by Region Id
        [HttpGet("{RegionId}")]
        public async Task<IActionResult> Read(int RegionId,String RoleList)
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
                var listOfCountries = await regionCountryMappingService.GetListOfCountriesByRegionId(RegionId);
                if (listOfCountries.Any())
                {
                    return Ok(listOfCountries);
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