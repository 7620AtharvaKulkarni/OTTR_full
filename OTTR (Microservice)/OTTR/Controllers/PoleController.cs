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
    public class PoleController : BaseController
    {
        private readonly IPoleService poleService;
        private readonly ILogger<PoleController> logger;

        public PoleController(IPoleService poleService, ILoggerFactory logger)
        {
            this.poleService = poleService;
            this.logger = logger.CreateLogger<PoleController>();
        }

        //Get the List of All Pole Information
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
                logger.LogInformation("Call GetListOfPoleInfo function for retrive all data of Pole Entity");
                var listOfPoleInfo = await poleService.GetListOfPoleInfo();
                if (listOfPoleInfo.Any())
                {
                    return Ok(listOfPoleInfo);
                }
                else
                {
                    return NoContent("No records are availeble for Pole");
                }
            }
            catch (Exception e)
            {
                logger.LogError("Error occured", e);
                return Error(e.Message, 500);
            }
        }
    }
}