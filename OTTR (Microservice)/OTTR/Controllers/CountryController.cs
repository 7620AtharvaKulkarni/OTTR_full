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

    public class CountryController : BaseController
    {
        private readonly ICountryService countryService;
        private readonly ILogger<CountryController> logger;

        public CountryController(ICountryService countryService, ILoggerFactory logger)
        {
            this.countryService = countryService;
            this.logger = logger.CreateLogger<CountryController>();
        }

        //Get the List of All Region Information
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
                logger.LogInformation("Call GetListOfRegionInfo for retrive all data of Region Entity");
                var listOfCountryInfo = await countryService.GetListOfCountryInfo();
                if (listOfCountryInfo.Any())
                {
                    return Ok(listOfCountryInfo);
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


    }
}