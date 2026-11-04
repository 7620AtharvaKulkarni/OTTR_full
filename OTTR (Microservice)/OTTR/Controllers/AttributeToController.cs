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
    public class AttributeToController : BaseController
    {
        private readonly IAttributeToService attributeToService;
        private readonly ILogger<AttributeToController> logger;
        public AttributeToController(IAttributeToService attributeToService, ILoggerFactory logger)
        {
            this.attributeToService = attributeToService;
            this.logger = logger.CreateLogger<AttributeToController>();
        }

        //Get the List of All AttributeTo Information
        [HttpGet]
        public async Task<IActionResult> Read(String RoleList)
        {
            ResponseResource responseResource = new ResponseResource();
            try
            {
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
                var listOfAttributeTo = await attributeToService.GetListOfAttributeToInfo();
                if (listOfAttributeTo.Any())
                {
                    return Ok(listOfAttributeTo);
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
