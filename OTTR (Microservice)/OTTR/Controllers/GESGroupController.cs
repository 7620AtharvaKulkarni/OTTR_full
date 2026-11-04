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
    public class GESGroupController : BaseController
    {
        private readonly IGESGroupService gesGroupService;
        private readonly ILogger<GESGroupController> logger;
        public GESGroupController(IGESGroupService gesGroupService, ILoggerFactory logger)
        {
            this.gesGroupService = gesGroupService;
            this.logger = logger.CreateLogger<GESGroupController>();
        }

        //Get the List of All GESGroup Information
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
                var listOfGESGroup = await gesGroupService.GetListOfGESGroupInfo();
                if (listOfGESGroup.Any())
                {
                    return Ok(listOfGESGroup);
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
    
        [HttpGet]
        [Route("GetSubGroup/{GESGroupName}")]
        public async Task<IActionResult> Read(string GESGroupName,String RoleList)
        {
            try
            {
                if (String.IsNullOrEmpty(RoleList))
                    return Error("Unauthorized Access", 401);
                List<string> listofFERoles = Newtonsoft.Json.Linq.JArray.Parse(RoleList).ToObject<IList<string>>().ToList();
                bool isRoleAuthorized = listofFERoles.Any(s => s.Contains("OTTR-"));
                if (!isRoleAuthorized)
                    return Error("Unauthorized Access", 401);
                var listOfOttrMonitoringInfo = await gesGroupService.GetGESubGroupByGESGroupName(GESGroupName);
                if (listOfOttrMonitoringInfo.Any())
                {
                    return Ok(listOfOttrMonitoringInfo);
                }
                else
                {
                    return Ok(listOfOttrMonitoringInfo);
                }
            }
            catch (Exception e)
            {
                logger.LogError("--Error occured --", e);
                logger.LogError("Exception Message" + e.Message);
                logger.LogError("Exception StackTrace" + e.StackTrace);
                return Error("Error occured" + e.Message);
            }
        }

        [HttpGet]
        [Route("GetSubGroupWithGroup")]
        public async Task<IActionResult> GetSubGroupWithGroup(String RoleList)
        {
            try
            {
                if (String.IsNullOrEmpty(RoleList))
                    return Error("Unauthorized Access", 401);
                List<string> listofFERoles = Newtonsoft.Json.Linq.JArray.Parse(RoleList).ToObject<IList<string>>().ToList();
                bool isRoleAuthorized = listofFERoles.Any(s => s.Contains("OTTR-"));
                if (!isRoleAuthorized)
                    return Error("Unauthorized Access", 401);
                var ListofSubGroupsWithGroups = await gesGroupService.GetSubGroupWithGroup();
                if (ListofSubGroupsWithGroups != null)
                {
                    return Ok(ListofSubGroupsWithGroups);
                }
                else
                {
                    return Ok(ListofSubGroupsWithGroups);
                }
            }
            catch (Exception e)
            {
                logger.LogError("--Error occured --", e);
                logger.LogError("Exception Message" + e.Message);
                logger.LogError("Exception StackTrace" + e.StackTrace);
                return Error("Error occured" + e.Message);
            }
        }
    }
}
