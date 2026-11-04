using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OTTR.Models;
using OTTR.Services;
using Microsoft.Extensions.Configuration;

namespace OTTR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OttrMonitoringController : BaseController
    {
        private readonly IOttrMonitoringService ottrMonitoringService;
        private readonly IAuditOttrMonitoringService auditOttrMonitoringService;
        private readonly IExcelFileGenerator excelFileGenerator;
        private readonly ILogger<OttrMonitoringController> logger;
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext context;

        public OttrMonitoringController(IOttrMonitoringService ottrMonitoringService, 
            IAuditOttrMonitoringService auditOttrMonitoringService, IExcelFileGenerator excelFileGenerator,
            ILoggerFactory logger, IConfiguration configuration, ApplicationDbContext context)
        {
            this.context = context;
            this.ottrMonitoringService = ottrMonitoringService;
            this.auditOttrMonitoringService = auditOttrMonitoringService;
            this.excelFileGenerator = excelFileGenerator;
            this.logger = logger.CreateLogger<OttrMonitoringController>();
            this.configuration = configuration;
        }

        //Get the List of All OttrMonitoring Information
        [HttpGet]
        [Route("GetAll/{GESGroupID}/{Country}")]
        public async Task<IActionResult> Read(int GESGroupID, string Country,string RoleList)
        {
            ResponseResource responseResource = new ResponseResource();
            if (!String.IsNullOrEmpty(RoleList))
            {
                List<string> listOfRoles = getListOfActualRoleNames(RoleList);
                if (listOfRoles.Contains("OTTR-Staff") || listOfRoles.Contains("OTTR-ViewOnly") || listOfRoles.Contains("OTTR-Admin"))
                {
                    try
                    {
                        var listOfOttrMonitoringInfo = await ottrMonitoringService.GetListOfOttrMonitoringInfo(GESGroupID,Country, listOfRoles);
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
                else
                {
                    responseResource.code = 401;
                    responseResource.data = "Unauthorized Access";
                    return Ok(responseResource);
                }
            }
            else
            {
                responseResource.code = 401;
                responseResource.data = "Unauthorized Access";
                return Ok(responseResource);
            }
        }
        public List<string> getListOfActualRoleNames(string RoleList)
        {
            
            List<string> listofFERoles = Newtonsoft.Json.Linq.JArray.Parse(RoleList).ToObject<IList<string>>().ToList();
            List<string> listOfActualRoleNames = new List<string>();
            foreach (var role in listofFERoles)
            {
                try
                {
                    listOfActualRoleNames.Add(role);
                }
                catch { }
            }
            return listOfActualRoleNames;
        }

        [HttpPost]
        [Route("Download/{GESGroupID}/{Country}")]
        public async Task<IActionResult> Download([FromBody] Dictionary<string,Filers> ottrMonitoringFilter, int GESGroupID,string Country, string RoleList)
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
                List<string> listOfRoles = getListOfActualRoleNames(RoleList);
                bool isRoleAuthorized = listOfRoles.Any(s => s.Contains("OTTR-"));
                if (!isRoleAuthorized)
                {
                    responseResource.code = 401;
                    responseResource.data = "Unauthorized Access";
                    return Ok(responseResource);
                }
                var listOfOttrMonitoringInfo = await ottrMonitoringService.CreateListOfOttrMonitoringForExcel(ottrMonitoringFilter, GESGroupID,Country, listOfRoles);
                
                var downloadpath = configuration.GetSection("DownloadFilesPath").Value.ToString();

                System.IO.Directory.CreateDirectory(downloadpath);
                var path = excelFileGenerator.CreateExcelFile(listOfOttrMonitoringInfo, downloadpath);
                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Path.GetFileName(path));
            }
            catch (Exception e)
            {
                logger.LogError("Exception Message" + e.Message);
                logger.LogError("Exception StackTrace" + e.StackTrace);
                return Error("Error occured" + e.Message);
            }
        }

        //Get the Information of Specific Ottr_Monitoring
        [HttpGet("{OttrId}")]
        public async Task<IActionResult> GetByOttrId(int OttrId, string RoleList)
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
                List<string> listOfRoles = getListOfActualRoleNames(RoleList);
                bool isRoleAuthorized = listOfRoles.Any(s => s.Contains("OTTR-"));
                if (!isRoleAuthorized)
                {
                    responseResource.code = 401;
                    responseResource.data = "Unauthorized Access";
                    return Ok(responseResource);
                }
                if (OttrId > 0)
                {
                    var employeeInformation = await ottrMonitoringService.GetSpecificOttrMonitoringInfoByOttrId(OttrId);
                    if (employeeInformation != null)
                    {
                        return Ok(employeeInformation);
                    }
                    else
                    {
                        return Error("No data available");
                    }
                }
                else
                {
                    return Error("Please Enter the Valid OTTRId");
                }
            }
            catch (Exception e)
            {
                logger.LogError("Exception Message" + e.Message);
                logger.LogError("Exception StackTrace" + e.StackTrace);
                return Error("Error occured" + e.Message);
            }
        }

        //Add the Information of New OttrMonitoring Information
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Post([FromQuery(Name = "userEID")] string userEID, [FromBody] OttrMonitoring ottrMonitoring,String RoleList)
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
                List<string> listOfRoles = getListOfActualRoleNames(RoleList);
                bool isRoleAuthorized = listOfRoles.Any(s => s.Contains("OTTR-Staff"));
                if (!isRoleAuthorized)
                {
                    responseResource.code = 401;
                    responseResource.data = "Unauthorized Access";
                    return Ok(responseResource);
                }
                if (ottrMonitoring != null)    
                {
                    ottrMonitoring.IsActive = true;
                    ottrMonitoring.CreatedBy = userEID;
                    ottrMonitoring.CreatedOn = DateTime.Now;
                    ottrMonitoring.ModifiedBy = userEID;
                    ottrMonitoring.ModifiedOn = DateTime.Now;
                    Dictionary<string, string> CreatedByuserdetails = getUserDetails(ottrMonitoring.CreatedBy).Result;
                    ottrMonitoring.CreatedByEmail = CreatedByuserdetails["EmailId"];
                    ottrMonitoring.CreatedByManagerEmailId = CreatedByuserdetails["ManagerEmailId"];

                    if (!string.IsNullOrEmpty(ottrMonitoring.gespmEID))
                    {
                        Dictionary<string, string> Gespmuserdetails = getUserDetails(ottrMonitoring.gespmEID).Result;
                        ottrMonitoring.GESPMEmail = Gespmuserdetails["EmailId"];
                        ottrMonitoring.GESPMManagerEmail = Gespmuserdetails["ManagerEmailId"];
                    }
                    if (!string.IsNullOrEmpty(ottrMonitoring.FOPMId)){

                        Dictionary<string, string> userdetails = GetUserInfoForMultipleEIDAsync(ottrMonitoring.FOPMId).Result;
                        ottrMonitoring.FOPMEmailID = userdetails["EmailId"];
                        ottrMonitoring.FOPMName = userdetails["UserName"];
                    }
                    if (!string.IsNullOrEmpty(ottrMonitoring.gesResourceId)){
                        ottrMonitoring.gesResourceName = getUserDetails(ottrMonitoring.gesResourceId).Result["UserName"];
                    }
                    var addedOttrMonitoringInformation = await ottrMonitoringService.AddOttrMonitoringInfo(ottrMonitoring);
                    if (addedOttrMonitoringInformation != null)
                    {
                        return Ok(addedOttrMonitoringInformation);
                    }
                    else
                    {
                        return Error("No data available");
                    }
                }
                else
                {
                    return Error("Please Enter the valid OTTR Information");
                }
            }
            catch (Exception e)
            {
                logger.LogError("--Error occured --", e);
                logger.LogError("Exception Message" + e.Message);
                logger.LogError("Exception StackTrace" + e.StackTrace);
                return Error( e.Message+ "\n --Error occured--  ");
            }
        }

        //Update the Information of  Created By EMail Information
        [HttpGet]
        [Route("UpdateCeatedByEmail")]
        public async Task<IActionResult> UpdateCeatedByEmail()
        {
            try
            {
                ResponseResource responseResource = new ResponseResource();

                var addedOttrMonitoringInformation = await ottrMonitoringService.GetAllOTTRInformation();
                if (addedOttrMonitoringInformation != null)
                {
                    return Ok(addedOttrMonitoringInformation);
                }
                else
                {
                    return Error("No data for update");
                }
            }
            catch (Exception e)
            {
                logger.LogError("--Error occured --", e);
                logger.LogError("Exception Message" + e.Message);
                logger.LogError("Exception StackTrace" + e.StackTrace);
                return Error(e.Message + "\n --Error occured--  ");
            }
        }


        public async Task<Dictionary<string, string>> GetUserInfoForMultipleEIDAsync(string EID)
        {
            try
            {
                string[] ListOfEID = EID.Split(";");
                List<string> FOPMName = new List<string>();
                List<string> FOPMEmailId = new List<string>();
                foreach (string eid in ListOfEID)
                {

                    UserPeopleSoftController user = new UserPeopleSoftController(configuration);
                    var userName = await user.GetUserInformationForUpload(eid.ToUpper()) as Microsoft.AspNetCore.Mvc.OkObjectResult;
                    Newtonsoft.Json.Linq.JObject result = (Newtonsoft.Json.Linq.JObject)userName.Value;

                    if (result != null && result["data"]?["query"]?["numrows"]?.ToString() != "0")
                    {
                        var rows = result["data"]["query"]["rows"];
                        string firstName = rows[0]["PREF_FIRST_NAME"].ToString();
                        string lastName = rows[0]["LAST_NAME"].ToString();
                        
                        //UserDetails.Add("EmailId", result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["row"]["A.EMAIL_ADDR"].ToString());
                        FOPMName.Add(firstName + " " + lastName);
                        FOPMEmailId.Add( rows[0]["EMAIL_ADDR"].ToString());
                    }

                    //if (result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["numrows"].ToString() != "0")
                    //{

                    //    string firstName = result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["row"]["A.PREF_FIRST_NAME"].ToString();
                    //    string lastName = result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["row"]["A.LAST_NAME"].ToString();

                    //    FOPMName.Add(firstName + " " + lastName);
                    //    FOPMEmailId.Add(result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["row"]["A.EMAIL_ADDR"].ToString());
                    //}
                    else
                    {
                        return null;
                    }


                }
                Dictionary<string, string> UserDetails = new Dictionary<string, string>();
                UserDetails.Add("UserName", string.Join(";", FOPMName.ToArray()));
                UserDetails.Add("EmailId", string.Join(";", FOPMEmailId.ToArray()));
                return UserDetails;
            }
            catch (Exception e)
            {

                return null;
            }
        }
        public async Task<Dictionary<string, string>> getUserDetails(string EID)
        {
            if (!String.IsNullOrEmpty(EID))
            {
                UserPeopleSoftController user = new UserPeopleSoftController(configuration);
                var userName = await user.GetUserInformationForUpload(EID) as Microsoft.AspNetCore.Mvc.OkObjectResult;
                if (userName != null)
                {
                    Newtonsoft.Json.Linq.JObject result = (Newtonsoft.Json.Linq.JObject)userName.Value;
                    //if (result != null && result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["numrows"].ToString() != "0")
                    //{
                    //    Dictionary<string, string> UserDetails = new Dictionary<string, string>();
                    //    string firstName = result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["row"]["A.PREF_FIRST_NAME"].ToString();
                    //    string lastName = result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["row"]["A.LAST_NAME"].ToString();
                    //    UserDetails.Add("UserName", firstName + " " + lastName);
                    //    UserDetails.Add("EmailId", result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["row"]["A.EMAIL_ADDR"].ToString());
                    //    UserDetails.Add("ManagerEmailId", result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["row"]["B.EMAIL_ADDR"].ToString()); 
                    //    return UserDetails;
                    //}
                    if (result != null && result["data"]?["query"]?["numrows"]?.ToString() != "0")
                    {
                        var rows = result["data"]["query"]["rows"];
                        Dictionary<string, string> UserDetails = new Dictionary<string, string>();
                        string firstName = rows[0]["PREF_FIRST_NAME"].ToString();
                        string lastName = rows[0]["LAST_NAME"].ToString();
                        UserDetails.Add("UserName", firstName + " " + lastName);
                        UserDetails.Add("EmailId", rows[0]["EMAIL_ADDR"].ToString());
                        var ManagerEmail = rows[0]["MGR_EMAIL_ADDR"].ToString();
                        UserDetails.Add("ManagerEmailId", ManagerEmail);
                        //UserDetails.Add("EmailId", result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["row"]["A.EMAIL_ADDR"].ToString());
                        return UserDetails;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        //Edit the Information of New OttrMonitoring Information
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Put([FromQuery(Name = "userEID")] string userEID, [FromBody] OttrMonitoring ottrMonitoring,String RoleList)
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
                List<string> listOfRoles = getListOfActualRoleNames(RoleList);
                bool isRoleAuthorized = listOfRoles.Any(s => s.Contains("OTTR-Staff"));
                if (!isRoleAuthorized)
                {
                    responseResource.code = 401;
                    responseResource.data = "Unauthorized Access";
                    return Ok(responseResource);
                }
                if (ottrMonitoring != null && ottrMonitoring.OttrId > 0)
                {

                    var ottrMonitoringInfo = await ottrMonitoringService.GetSpecificOttrMonitoringInfoByOttrId(ottrMonitoring.OttrId);
                    //if (ottrMonitoringInfo.PlanDate.CompareTo(ottrMonitoring.PlanDate) != 0)
                    //{
                    ottrMonitoring.IsActive = true;
                    ottrMonitoringInfo.CreatedBy = userEID;
                    ottrMonitoringInfo.CreatedOn = DateTime.Now;
                    ottrMonitoringInfo.ModifiedBy = userEID;
                    ottrMonitoringInfo.ModifiedOn = DateTime.Now;
                    ottrMonitoring.surveyFor = ottrMonitoring.surveyFor;

                    if (ottrMonitoring.surveyFor=="External")
                    {
                        ottrMonitoring.exCustEmailId = ottrMonitoring.exCustEmailId;
                        ottrMonitoring.exCustName = ottrMonitoring.exCustName;
                    }
                    

                    if (!string.IsNullOrEmpty(ottrMonitoring.FOPMId))
                    {
                        Dictionary<string, string> userdetails = GetUserInfoForMultipleEIDAsync(ottrMonitoring.FOPMId).Result;
                        ottrMonitoringInfo.FOPMEmailID = userdetails["EmailId"];
                        ottrMonitoringInfo.FOPMName = userdetails["UserName"];
                        ottrMonitoring.FOPMEmailID = ottrMonitoringInfo.FOPMEmailID;

                    }
                    if (!string.IsNullOrEmpty(ottrMonitoring.gesResourceId))
                    {
                        ottrMonitoringInfo.gesResourceId = ottrMonitoring.gesResourceId;
                        ottrMonitoring.gesResourceName = getUserDetails(ottrMonitoring.gesResourceId).Result["UserName"];
                        ottrMonitoringInfo.gesResourceName = ottrMonitoring.gesResourceName;
                        ottrMonitoring.gesResourceId = ottrMonitoringInfo.gesResourceId;
                        ottrMonitoring.gesResourceName = ottrMonitoringInfo.gesResourceName;

                    }

                    var addedAuditOttrMonitoring = await auditOttrMonitoringService.AddAuditOttrMonitoringInfo(ottrMonitoringInfo);
                    //}

                    ottrMonitoring.CreatedBy = ottrMonitoringInfo.CreatedBy;
                    ottrMonitoring.CreatedOn = ottrMonitoringInfo.ModifiedOn;
                    ottrMonitoring.ModifiedOn = DateTime.Now;


                    var addedOttrMonitoringInformation = await ottrMonitoringService.UpdateOttrMonitoringInfo(ottrMonitoring);
                    if (addedOttrMonitoringInformation != null)
                    {
                        return Ok(addedOttrMonitoringInformation);
                    }
                    else
                    {
                        return Error("No data available");
                    }
                }
                else
                {
                    return Error("Please Enter the valid OTTR Id");
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

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete([FromQuery(Name = "userEID")] string userEID, [FromBody] OTTRList ottrMonitoring, String RoleList)
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
                List<string> listOfRoles = getListOfActualRoleNames(RoleList);
                bool isRoleAuthorized = listOfRoles.Any(s => s.Contains("OTTR-Admin"));
                if (!isRoleAuthorized)
                {
                    responseResource.code = 401;
                    responseResource.data = "Unauthorized Access";
                    return Ok(responseResource);
                }
                if (ottrMonitoring != null)
                {
                    var addedOttrMonitoringInformation = await ottrMonitoringService.DeleteOTTRMonitoring(ottrMonitoring, userEID);
                    if (addedOttrMonitoringInformation != null)
                    {
                        return Ok(addedOttrMonitoringInformation);
                    }
                    else
                    {
                        return Error("Unable to Delete OTTR");
                    }
                }
                else
                {
                    return Error("Please Enter the valid OTTR Id");
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
        [Route("updateEmailId")]
        public async Task<string> updateEmailIdAsync()
        {
            try
            {
               List<OttrMonitoring> ottrs= context.OttrMonitorings.Where(x => x.IsActive == true && String.IsNullOrEmpty(x.FOPMEmailID) && !String.IsNullOrEmpty(x.FOPMId)).OrderByDescending(y => y.ModifiedOn).ToList();
               
                foreach(OttrMonitoring ottr in ottrs)
                {
                    if (!String.IsNullOrEmpty(ottr.FOPMId))
                    {
                        Dictionary<string,string> userDetails= getUserDetails(ottr.FOPMId).Result;
                        if (userDetails!=null)
                        {
                            ottr.FOPMEmailID = userDetails["EmailId"];
                            context.OttrMonitorings.Update(ottr);
                            await context.SaveChangesAsync();
                        }
                    }
                }
                return "Success";
            }
            catch (Exception e)
            {
                logger.LogError("--Error occured --", e);
                logger.LogError("Exception Message" + e.Message);
                logger.LogError("Exception StackTrace" + e.StackTrace);
                return "error";
            }
        }

    }
}
