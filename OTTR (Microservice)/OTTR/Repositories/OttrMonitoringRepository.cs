using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;
using OTTR.Controllers;
using OTTR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTTR.Repositories
{
    public class OttrMonitoringRepository : IOttrMonitoringRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IConfiguration configuration;
        public OttrMonitoringRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        //Adding New OttrMonitoring Information
        public async Task<OttrMonitoring> AddOTTRInformation(OttrMonitoring ottrMonitoring)
        {
            try
            {
                ottrMonitoring.OttrId = 0;
                context.OttrMonitorings.Add(ottrMonitoring);
                context.SaveChanges();
                return ottrMonitoring;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<List<OttrMonitoring>> AddOTTRInformationAll(List<OttrMonitoring> ottrMonitoring)
        {
            try
            {
                foreach (OttrMonitoring ott in ottrMonitoring)
                {
                    try
                    {
                        ott.OttrId = 0;
                        await   AddOTTRInformation(ott);
                      
                    }
                    catch (Exception e)
                    {
                       
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                return ottrMonitoring;
            }
        }

        //Get the List of OttrMonitoring Information

       
        public async Task<IEnumerable<OttrMonitoring>> GetAllOTTRInformation(int GESGroupID,string Country, List<string> listOfRoles)
        {
            
            if (GESGroupID == 0)
            {
                if (listOfRoles.Contains("OTTR-Admin")){
                        return context.OttrMonitorings.Where(x => x.IsActive == true ).OrderByDescending(y => y.ModifiedOn).ToList();
                }
                else if(listOfRoles.Contains("OTTR-Staff") || listOfRoles.Contains("OTTR-ViewOnly"))
                {
                    string sqlScript = "Select GES_Group_Id from tblOTTR_GESGroup where GES_Group like'%"+Country+"%'";
                    List<int> ids = context.OttrMonitorings.FromSqlRaw(sqlScript).Select(x=>x.GESGroupId).ToList();
                    return context.OttrMonitorings.Where(x => (ids.Contains(x.GESGroupId)) && (x.IsActive == true)).OrderByDescending(y => y.ModifiedOn).ToList();
                }
                else
                {
                    return context.OttrMonitorings.Where(x => x.IsActive == true).OrderByDescending(y => y.ModifiedOn).ToList();
                }
            }
            else
            {
                GESGroupRepository groupRepository = new GESGroupRepository(context);
                Task<string> gesGroupName = groupRepository.GetGESGroupNameByGESGroupId(GESGroupID);
              
                    var ids = groupRepository.GetAllGESGroupIdByGESGroupName(gesGroupName.Result);
                    var listOfOttrInformation = context.OttrMonitorings.Where(x => (ids.Contains(x.GESGroupId)) && (x.IsActive == true) ).OrderByDescending(y => y.ModifiedOn).ToList();
                    return listOfOttrInformation;
            }
        }

        public async Task<List<OttrMonitoring>> GetAllOTTRInformation()
        {
            var OTTRRecordList= context.OttrMonitorings.Where(x => x.IsActive == true).Where(x=>x.GESPMEmail ==null && x.gespmEID != null) .OrderByDescending(y => y.ModifiedOn).ToList();
            var userDetailsCache = new Dictionary<string, Dictionary<string, string>>();
            
            //foreach (var OTTRrecord in OTTRRecordList)
            //{
            //    //if (!userDetailsCache.TryGetValue(OTTRrecord.CreatedBy, out var userdetails))
            //    //{
            //    //    userdetails = await getUserDetails(OTTRrecord.CreatedBy);
            //    //    userDetailsCache[OTTRrecord.CreatedBy] = userdetails;
            //    //}

            //    //if (userdetails != null)
            //    //{
            //    //    OTTRrecord.CreatedByEmail = userdetails["EmailId"];
            //    //    OTTRrecord.CreatedByManagerEmailId = userdetails["ManagerEmailId"];
            //    //}

            //    if (!userDetailsCache.TryGetValue(OTTRrecord.gespmEID, out var userdetails))
            //    {
            //        userdetails = await getUserDetails(OTTRrecord.gespmEID);
            //        userDetailsCache[OTTRrecord.gespmEID] = userdetails;
            //    }
            //    if (userdetails != null)
            //    {
            //        OTTRrecord.GESPMEmail = userdetails["EmailId"];
            //        OTTRrecord.GESPMManagerEmail = userdetails["ManagerEmailId"];
            //    }                
            //}

            const int batchSize = 500;
            var totalRecords = OTTRRecordList.Count;
            try
            {
                for (int i = 0; i < totalRecords; i += batchSize)
                {
                    var batch = OTTRRecordList.Skip(i).Take(batchSize).ToList();

                    foreach (var OTTRrecord in batch)
                    {
                        if (!userDetailsCache.TryGetValue(OTTRrecord.gespmEID, out var userdetails))
                        {
                            userdetails = await getUserDetails(OTTRrecord.gespmEID);
                            userDetailsCache[OTTRrecord.gespmEID] = userdetails;
                        }
                        if (userdetails != null)
                        {
                            OTTRrecord.GESPMEmail = userdetails["EmailId"];
                            OTTRrecord.GESPMManagerEmail = userdetails["ManagerEmailId"];
                        }
                    }

                    context.OttrMonitorings.UpdateRange(batch);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            
            //context.OttrMonitorings.UpdateRange(OTTRRecordList);
            //await context.SaveChangesAsync();
            return OTTRRecordList;
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

        public async Task<List<OttrMonitoringModel>> GetAllOTTRInformationWithFilters(Dictionary<string, Filers> ottrFilters, int gesGroupId,string country, List<string> listOfRoles)
        {

            GESGroupRepository groupRepository = new GESGroupRepository(context);
            string stringOfIds = null;
            if (gesGroupId != 0)
            {
                Task<string> gesGroupName = groupRepository.GetGESGroupNameByGESGroupId(gesGroupId);

                var ids = groupRepository.GetAllGESGroupIdByGESGroupName(gesGroupName.Result);
                stringOfIds = String.Join(",", ids);
            }
            else
            {
                if (listOfRoles.Contains("OTTR-Admin"))
                {
                    List<int> ids = context.GESGroups.Where(x => x.IsActive == true).Select(x => x.GESGroupId).ToList();
                    stringOfIds = String.Join(",", ids);
                }
                else if (listOfRoles.Contains("OTTR-Staff") || listOfRoles.Contains("OTTR-ViewOnly"))
                {
                    string sqlScriptForId = "Select GES_Group_Id from tblOTTR_GESGroup where GES_Group like'%" + country + "%'";
                    List<int> ids = context.OttrMonitorings.FromSqlRaw(sqlScriptForId).Select(x => x.GESGroupId).ToList();
                    stringOfIds = String.Join(",", ids);
                }
            }

            string sqlScript = "Select o.OTTR_ID as OttrId,o.GES_PM as GESPM,o.GES_PM_EID as gespmEID,o.ProjectId,o.ProjectName,"+
                "o.Deliverables,o.Month,o.BaseDate,o.PlanDate,o.ActualDate,o.ActualDateModifiedOn,o.ActualDateModifiedBy,o.TrackingToDesignInput,o.Comments,o.CustomerSurvey," +
                "o.survey_For as surveyFor,o.exCust_Name as exCustName,o.exCust_EmailId as exCustEmailId,o.FO_PM_Name as FOPMName,o.FO_PM_ID as FOPMId,o.FO_PM_EmailId as FOPMEmailId,o.CustomerSurveyTemplate," +
                "o.Survey_Request as SurveyRequest,o.Survey_Type as surveyType,o.GES_Resource_EID as gesResourceId,"+
                "o.GES_Resource_Name as gesResourceName, o.CreatedBy,o.CreatedOn,g.GES_Sub_Group as GESGroup,g.GES_Group as GESCenter,r.RegionName as Region,p.ProductName as Product,"+
                "a.AttributeTo,i.InternalExternalAttributeTo as InternalExternalAttributeId,"+
                "c.CountryName as SpendPlan_Country,sp.PoleName as SpendPlan_Pole,sr.SPregionName as SpendPlan_Region, DeliveryHours,ReworkHours,CreatedByEmail, CreatedByManagerEmailId, GESPMEmail, GESPMManagerEmail from tblOTTR_Monitoring AS o " +
                " left Outer JOIN tblOTTR_GESGroup AS g ON o.GES_Group_Id=g.GES_Group_Id " +
                " left Outer JOIN tblOTTR_Region AS r ON o.RegionId=r.RegionId" +
                " left Outer JOIN tblOTTR_Product AS p ON o.ProductId=p.ProductId" +
                " left Outer JOIN tblOTTR_AttributeTo AS a ON o.AttributeToId=a.AttributeToId" +
                " left Outer JOIN tblOTTR_InternalExternalAttributeTo AS i ON o.InternalExternalAttributeId=i.InternalExternalAttributeToId" +
                " left Outer JOIN tblSpendPlan_Country AS c ON o.SpendPlan_Country=c.CountryID" +
                " left Outer JOIN tblSpendPlan_Pole AS sp ON  o.SpendPlan_Pole=sp.PoleID" +
                " left Outer JOIN tblSpendPlan_Region AS sr ON o.SpendPlan_Region=sr.RegionID where OTTR_ID is not NULL AND o.IsActive=1 " +
                " AND o.GES_Group_Id IN (" + stringOfIds + ")";
            
            foreach (var ottrFilter in ottrFilters) {
               
                if (ottrFilter.Value.@operator != null)
                {

                    if (ottrFilter.Value.@operator.Equals("AND"))
                    {
                        sqlScript += AppendCondition1(ottrFilter, "AND");

                    }
                    else if (ottrFilter.Value.@operator.Equals("OR"))
                    {
                        sqlScript += AppendCondition1(ottrFilter, "OR");
                    }

                }
                else
                {
                    string key = ottrFilter.Key;
                    
                    if (key.Equals("planDate"))
                    {
                        key = "FORMAT(o.planDate, 'dd-MMM-yy')";
                    }
                    if (key.Equals("baseDate"))
                    {
                        key = "FORMAT(o.baseDate, 'dd-MMM-yy')";
                    }
                    if (key.Equals("actualDate"))
                    {
                        key = "FORMAT(o.actualDate, 'dd-MMM-yy')";
                    }
                    if (key.Equals("month"))
                    {
                        key = "FORMAT(o.planDate, 'MMM-yyyy')";
                    }
                    if (ottrFilter.Value.type.Equals("contains"))
                    {
                        sqlScript += " And " + key + " LIKE '%" + ottrFilter.Value.filter + "%'";

                    }
                    else if (ottrFilter.Value.type.Equals("notContains"))
                    {
                        sqlScript += " And " +"("+ key + " NOT LIKE '%" + ottrFilter.Value.filter + "%'";
                    }
                    else if (ottrFilter.Value.type.Equals("equals"))
                    {
                        sqlScript += " And " + key + "=" + "'"+ottrFilter.Value.filter+"'";
                    }
                    else if (ottrFilter.Value.type.Equals("notEqual"))
                    {
                        sqlScript += " And " + key + " <> " + "'"+ottrFilter.Value.filter+"'";
                    }
                    else if (ottrFilter.Value.type.Equals("startsWith"))
                    {
                        sqlScript += " And " + key + " LIKE '" + ottrFilter.Value.filter + "%'";
                    }
                    else if (ottrFilter.Value.type.Equals("endsWith"))
                    {
                        sqlScript += " And " + key + " LIKE '%" + ottrFilter.Value.filter + "'";
                    }
                }
            }



            List<OttrMonitoringModel> listOfOttrInformation = context.OttrMonitoringModels.FromSqlRaw(sqlScript).ToList();
            

            return listOfOttrInformation;
        }
        public string AppendCondition2(KeyValuePair<string,Filers> ottrFilter,string condition)
        {
            string sqlScript = null;
            string key = ottrFilter.Key;

            if (key.Equals("planDate"))
            {
                key = "FORMAT(o.planDate, 'dd-MMM-yy')";
            }
            if (key.Equals("baseDate"))
            {
                key = "FORMAT(o.baseDate, 'dd-MMM-yy')";
            }
            if (key.Equals("actualDate"))
            {
                key = "FORMAT(o.actualDate, 'dd-MMM-yy')";
            }
            if (key.Equals("month"))
            {
                key = "FORMAT(o.planDate, 'MMM-yyyy')";
            }
            if (ottrFilter.Value.condition2.type.Equals("contains"))
            {
                sqlScript += " "+condition +" "+ key + " LIKE '%" + ottrFilter.Value.condition2.filter + "%'";

            }
            else if (ottrFilter.Value.condition2.type.Equals("notContains"))
            {

                sqlScript += " " + condition + " "+ key + " NOT LIKE '%" + ottrFilter.Value.condition2.filter + "%'";
            }
            else if (ottrFilter.Value.condition2.type.Equals("equals"))
            {
                sqlScript += " " + condition + " " + key + "="+"'"+ottrFilter.Value.condition2.filter+"'";
            }
            else if (ottrFilter.Value.condition2.type.Equals("notEqual"))
            {
                sqlScript += " " + condition + " " + key + "<>" + "'"+ottrFilter.Value.condition2.filter+"'";
            }
            else if (ottrFilter.Value.condition2.type.Equals("startsWith"))
            {
                sqlScript += " " + condition + " " + key + " LIKE '" + ottrFilter.Value.condition2.filter + "%'";
            }
            else if (ottrFilter.Value.condition2.type.Equals("endsWith"))
            {
                sqlScript += " " + condition + " " + key + " LIKE '%" + ottrFilter.Value.condition2.filter + "'";
            }
            return sqlScript;
        }

        public string AppendCondition1(KeyValuePair<string, Filers> ottrFilter, string condition)
        {
            string sqlScript = null;
            string key = ottrFilter.Key;

            if (key.Equals("planDate"))
            {
                key = "FORMAT(o.planDate, 'dd-MMM-yy')";
            }
            if (key.Equals("baseDate"))
            {
                key = "FORMAT(o.baseDate, 'dd-MMM-yy')";
            }
            if (key.Equals("actualDate"))
            {
                key = "FORMAT(o.actualDate, 'dd-MMM-yy')";
            }
            if (key.Equals("month"))
            {
                key = "FORMAT(o.planDate, 'MMM-yyyy')";
            }
            string bracket = ottrFilter.Value.condition2 != null ? "(" : " ";
            if (ottrFilter.Value.condition1.type.Equals("contains"))
                {
                        
                    sqlScript += " AND "+ bracket + key + " LIKE '%" + ottrFilter.Value.condition1.filter + "%'";
                    if (ottrFilter.Value.condition2 != null)
                    {
                        sqlScript += AppendCondition2(ottrFilter, condition)+")";

                    }
                }
                else if (ottrFilter.Value.condition1.type.Equals("notContains"))
                {
                    sqlScript += " AND " + bracket +"("+ key + " NOT LIKE '%" + ottrFilter.Value.condition1.filter + "%')";
                if (ottrFilter.Value.condition2 != null)
                    {
                    sqlScript += AppendCondition2(ottrFilter, condition) + ")";
                }
                }
                else if (ottrFilter.Value.condition1.type.Equals("equals"))
                {
                    sqlScript += " AND " + bracket + key + "=" + "'"+ottrFilter.Value.condition1.filter+"'";
                    if (ottrFilter.Value.condition2 != null)
                    {
                    sqlScript += AppendCondition2(ottrFilter, condition) + ")";
                }
                }
                else if (ottrFilter.Value.condition1.type.Equals("notEqual"))
                {
                    sqlScript += " AND " + bracket + key + "<>" + "'"+ottrFilter.Value.condition1.filter+"'";
                    if (ottrFilter.Value.condition2 != null)
                    {
                    sqlScript += AppendCondition2(ottrFilter, condition) + ")";
                }
                }
                else if (ottrFilter.Value.condition1.type.Equals("startsWith"))
                {
                    sqlScript += " AND " + bracket + key + " LIKE '" + ottrFilter.Value.condition1.filter + "%'";
                    if (ottrFilter.Value.condition2 != null)
                    {
                    sqlScript += AppendCondition2(ottrFilter, condition) + ")";
                }
                }
                else if (ottrFilter.Value.condition1.type.Equals("endsWith"))
                {
                    sqlScript += " AND " + bracket + key + " LIKE '%" + ottrFilter.Value.condition1.filter + "'";
                    if (ottrFilter.Value.condition2 != null)
                    {
                    sqlScript += AppendCondition2(ottrFilter, condition) + ")";
                }

                }
                return sqlScript;
        }

        //Get Particular OttrMonitoring Information
        public async Task<OttrMonitoring> GetSpecificOttrInfo(int ottrId)
        {
            try
            {
                var ottrMonitoring = context.OttrMonitorings.Where(x => (x.OttrId == ottrId) && (x.IsActive == true)).FirstOrDefault();
                return ottrMonitoring;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        
         
        public List<OttrMonitoring> GetOTTRList()
        {
            try
            {
                List< OttrMonitoring> OttrList = context.OttrMonitorings.Where(x =>(x.ActualDate==null) && (x.IsActive == true)).ToList();
                return OttrList;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        //Update Particular OttrMonitoring Information
        public async Task<OttrMonitoring> UpdateOttrInformation(OttrMonitoring ottrMonitoring)
        {
            try
            {
                var ottroMonitoringList = context.OttrMonitorings.Where(x => x.OttrId == ottrMonitoring.OttrId).ToList();
                if(ottroMonitoringList.Any())
                {
                    var ottrMonitoringInfo = ottroMonitoringList.ElementAt(0);
                    ottrMonitoringInfo.GESPM = ottrMonitoring.GESPM;
                    ottrMonitoringInfo.gespmEID = ottrMonitoring.gespmEID;
                    ottrMonitoringInfo.ProjectId = ottrMonitoring.ProjectId;
                    ottrMonitoringInfo.ProjectName = ottrMonitoring.ProjectName;
                    ottrMonitoringInfo.Deliverables = ottrMonitoring.Deliverables;
                    ottrMonitoringInfo.Month = ottrMonitoring.Month;
                    ottrMonitoringInfo.BaseDate = ottrMonitoring.BaseDate;
                    ottrMonitoringInfo.PlanDate = ottrMonitoring.PlanDate;
                    if (ottrMonitoringInfo.ActualDate != ottrMonitoring.ActualDate)
                    {
                        ottrMonitoringInfo.ActualDateModifiedBy = ottrMonitoring.ModifiedBy;
                        ottrMonitoringInfo.ActualDateModifiedOn = DateTime.Now;
                    }
                    ottrMonitoringInfo.ActualDate = ottrMonitoring.ActualDate;
                    ottrMonitoringInfo.GESGroupId = ottrMonitoring.GESGroupId;
                    ottrMonitoringInfo.RegionId = ottrMonitoring.RegionId;
                    ottrMonitoringInfo.ProductId = ottrMonitoring.ProductId;
                    ottrMonitoringInfo.AttributeToId = ottrMonitoring.AttributeToId;
                    ottrMonitoringInfo.Comments = ottrMonitoring.Comments;
                    ottrMonitoringInfo.CustomerSurvey = ottrMonitoring.CustomerSurvey;
                    ottrMonitoringInfo.FOPMName = ottrMonitoring.FOPMName;
                    ottrMonitoringInfo.FOPMId = ottrMonitoring.FOPMId;
                    ottrMonitoringInfo.FOPMEmailID = ottrMonitoring.FOPMEmailID;
                    ottrMonitoringInfo.CustomerSurveyTemplate = ottrMonitoring.CustomerSurveyTemplate;
                    ottrMonitoringInfo.IsActive = ottrMonitoring.IsActive;
                    ottrMonitoringInfo.CreatedBy = ottrMonitoring.CreatedBy;
                    ottrMonitoringInfo.CreatedOn = ottrMonitoring.CreatedOn;
                    ottrMonitoringInfo.ModifiedBy = ottrMonitoring.ModifiedBy;
                    ottrMonitoringInfo.ModifiedOn = ottrMonitoring.ModifiedOn;
                    ottrMonitoringInfo.TrackingToDesignInput = ottrMonitoring.TrackingToDesignInput;
                    ottrMonitoringInfo.InternalExternalAttributeId = ottrMonitoring.InternalExternalAttributeId;
                    ottrMonitoringInfo.SpendPlan_Country = ottrMonitoring.SpendPlan_Country;
                    ottrMonitoringInfo.SpendPlan_Pole = ottrMonitoring.SpendPlan_Pole;
                    ottrMonitoringInfo.SpendPlan_Region = ottrMonitoring.SpendPlan_Region;
                    ottrMonitoringInfo.SurveyRequest = ottrMonitoring.SurveyRequest;
                    ottrMonitoringInfo.surveyType = ottrMonitoring.surveyType;
                    ottrMonitoringInfo.gesResourceId = ottrMonitoring.gesResourceId;
                    ottrMonitoringInfo.gesResourceName = ottrMonitoring.gesResourceName;
                    ottrMonitoringInfo.DeliveryHours = ottrMonitoring.DeliveryHours;
                    ottrMonitoringInfo.ReworkHours = ottrMonitoring.ReworkHours;
                    ottrMonitoringInfo.surveyFor = ottrMonitoring.surveyFor;
                    ottrMonitoringInfo.exCustName = ottrMonitoring.exCustName;
                    ottrMonitoringInfo.exCustEmailId = ottrMonitoring.exCustEmailId;
                    context.OttrMonitorings.Update(ottrMonitoringInfo);
                    await context.SaveChangesAsync();
                    return ottrMonitoringInfo;
                }
                return null;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<List<OttrMonitoring>> Delete(List<OttrMonitoring> ottrMonitoring)
        {
            try
            {
                context.OttrMonitorings.UpdateRange(ottrMonitoring);
                await context.SaveChangesAsync();
                return ottrMonitoring;
                
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        
    }
}

