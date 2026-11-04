using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OTTR.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using OTTR.Repositories;
using System.Text.RegularExpressions;
using OTTR.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.OleDb;
using Microsoft.Data.SqlClient;
using System.IO;
using ClosedXML.Excel;

namespace OTTR.Services
{
    public interface IFileUploadService 
    {
        Task<bool>  ParseFile(string save_path,string userEID,string Lob);
    }
    public class FileUploadService : IFileUploadService
    {
        private readonly IOttrMonitoringRepository ottrMonitoringRepository;
        private readonly IAttributeToRepository attributeToRepository;
        private readonly IGESGroupRepository gesGroupRepository;
        private readonly IRegionRepository regionRepository;
        private readonly IProductRepository productRepository;
        private readonly IGesgroupdeliveryRepository gesgroupdeliveryRepository;
        private readonly IPoleRepository poleRepository;
        private readonly ICountryRepository countryRepository;
        private readonly IInternalExternalAttributeToRepository internalExternalAttributeToRepository;
        private readonly IGESGroupProductMappingRepository gESGroupProductMappingRepository;
        private readonly ILogger<UserPeopleSoftController> logger;
        private readonly IConfiguration configuration;
        private readonly IRegionCountryMappingRepository regionCountryMappingRepository;

        public FileUploadService(IConfiguration configuration, ILoggerFactory logger, IGesgroupdeliveryRepository gesgroupdeliveryRepository, IPoleRepository poleRepository, ICountryRepository countryRepository, IOttrMonitoringRepository ottrMonitoringRepository, IAttributeToRepository attributeToRepository,
            IGESGroupRepository gesGroupRepository, IRegionRepository regionRepository, IGESGroupProductMappingRepository gESGroupProductMappingRepository, IProductRepository productRepository, IInternalExternalAttributeToRepository internalExternalAttributeToRepository, IRegionCountryMappingRepository regionCountryMappingRepository)
        {
            this.ottrMonitoringRepository = ottrMonitoringRepository;
            this.attributeToRepository = attributeToRepository;
            this.gesGroupRepository = gesGroupRepository;
            this.regionRepository = regionRepository;
            this.productRepository = productRepository;
            this.internalExternalAttributeToRepository = internalExternalAttributeToRepository;
            this.gESGroupProductMappingRepository = gESGroupProductMappingRepository;
            this.countryRepository = countryRepository;
            this.poleRepository = poleRepository;
            this.gesgroupdeliveryRepository = gesgroupdeliveryRepository;
            this.logger = logger.CreateLogger<UserPeopleSoftController>();
            this.configuration = configuration;
            this.regionCountryMappingRepository = regionCountryMappingRepository;
        }

        public async Task<bool> ParseFile(string save_path, string userEID, string Lob)
        {

            ResponseResource responseResource = new ResponseResource();
            List<OttrMonitoring> ottrMonitorings = new List<OttrMonitoring>();
            DataTable dt = ConvertExcelToDataTable(save_path);
            int rowCount = 2;

            foreach (DataRow row in dt.Rows)
            {
                int gesGroupId = 0;
                int productId = 0;
                int AttributeToId = 0;
                int RegionId = 0;
                string GESPM_name = "";
                string GESResource_name = "";
                string Fopm_name = "";
                string Fopm_emailId = "";
                if (string.IsNullOrEmpty(row["GES_PM_EID"].ToString().Trim()))
                {
                    throw new FileValidationException(FileValidationException.Cause.backend, "GES_PM_EID is Missing in Row Number "+ rowCount);
                }
                else
                {
                    Dictionary<string, string> userDetails = getUserDetails(row["GES_PM_EID"].ToString()).Result;
                    if(userDetails == null)
                    {
                        throw new FileValidationException(FileValidationException.Cause.backend, "GES_PM_EID is invalid in Row Number " + rowCount);
                    }
                    else
                    {
                        GESPM_name = userDetails["UserName"];
                    }
                }
                if (string.IsNullOrEmpty(row["Project ID"].ToString().Trim()))
                {
                    throw new FileValidationException(FileValidationException.Cause.backend, "Project ID is Missing in Row Number "+ rowCount);
                }
                if (string.IsNullOrEmpty(row["Project Name"].ToString().Trim()))
                {
                    throw new FileValidationException(FileValidationException.Cause.backend, "Project Name is Missing in Row Number "+ rowCount);
                }
                if (string.IsNullOrEmpty(row["Deliverables / Milestone"].ToString().Trim()))
                {
                    throw new FileValidationException(FileValidationException.Cause.backend, "Deliverables / Milestone is Missing in Row Number " + rowCount);
                }
                if (String.IsNullOrEmpty(row["Base Date"].ToString().Trim()))
                {
                    throw new FileValidationException(FileValidationException.Cause.backend, "Base Date is Missing in Row Number " + rowCount);
                }
                if (string.IsNullOrEmpty(row["Plan Date"].ToString().Trim()))
                {
                    throw new FileValidationException(FileValidationException.Cause.backend, "Plan Date is Missing in Row Number " + rowCount);
                }
                if (string.IsNullOrEmpty(row["Survey Request"].ToString().Trim()))
                {
                    throw new FileValidationException(FileValidationException.Cause.backend, "Survey Request is Missing in Row Number " + rowCount);
                }
                else
                {
                    if (row["Survey Request"].ToString().ToLower().Equals("Required".ToLower()))
                    {
                        if (string.IsNullOrEmpty(row["Pole"].ToString().Trim()))
                        {
                            throw new FileValidationException(FileValidationException.Cause.backend, "Pole is Required if Survey Request is " + "\"Required\"" + " in Row Number " + rowCount);
                        }
                        if (string.IsNullOrEmpty(row["Region"].ToString().Trim()))
                        {
                            throw new FileValidationException(FileValidationException.Cause.backend, "Region is Required if Survey Request is " + "\"Required\"" + " in Row Number " + rowCount);
                        }
                        if (string.IsNullOrEmpty(row["Country"].ToString().Trim()))
                        {
                            throw new FileValidationException(FileValidationException.Cause.backend, "Country is Required if Survey Request is " + "\"Required\"" + " in Row Number " + rowCount);
                        }
                        if (string.IsNullOrEmpty(row["Survey Type"].ToString().Trim()))
                        {
                            throw new FileValidationException(FileValidationException.Cause.backend, "Survey Type is Required if Survey Request is " + "\"Required\"" + " in Row Number " + rowCount);
                        }
                        else
                        {
                            if (row["Survey Type"].ToString().Replace(" ", "").ToLower().Equals("Front office".ToLower().Replace(" ", "")))
                            {
                                if (string.IsNullOrEmpty(row["GES Resource EID"].ToString().Trim()))
                                {
                                    throw new FileValidationException(FileValidationException.Cause.backend, "GES Resource EID is Required if Survey Request is " + "\"Required\"" + " and Survey Type is " + "\" Front Office\"" + "in Row Number " + rowCount);
                                }
                                else
                                {
                                    Dictionary<string, string> userDetails = getUserDetails(row["GES Resource EID"].ToString()).Result;
                                    if (userDetails == null)
                                    {
                                        throw new FileValidationException(FileValidationException.Cause.backend, "GES Resource EID is invalid in Row Number " + rowCount);
                                    }
                                    else
                                    {
                                        GESResource_name = userDetails["UserName"];
                                    }
                                }

                            }
                            else
                            {
                                if (!row["Survey Type"].ToString().Replace(" ", "").ToLower().Equals("back office".ToLower().Replace(" ", "")))
                                {
                                    throw new FileValidationException(FileValidationException.Cause.backend, "Survey Type is invalid in Row Number " + rowCount);
                                }
                            }

                        }
                        if (string.IsNullOrEmpty(row["FOPM EID"].ToString().Trim()))
                        {
                            throw new FileValidationException(FileValidationException.Cause.backend, "FOPM EID is Required if Survey Request is " + "\"Required\"" + " in Row Number " + rowCount);
                        }
                        

                    }
                    else
                    {
                        if (!row["Survey Request"].ToString().ToLower().Equals("na".ToLower()))
                        {
                            throw new FileValidationException(FileValidationException.Cause.backend, "Survey Request is invalid in Row Number " + rowCount);
                        }
                    }

                    if (!string.IsNullOrEmpty(row["FOPM EID"].ToString()))
                    {
                        var list = new List<string>();
                        string[] ListOfEID = row["FOPM EID"].ToString().Split(";");

                        foreach (string eid in ListOfEID)
                        {
                            if (list.Any(s => s.Trim().Equals(eid, StringComparison.OrdinalIgnoreCase)))
                            {
                                throw new FileValidationException(FileValidationException.Cause.backend, "please Enter unique FOPM EID " + "in Row Number " + rowCount);

                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(eid))
                                {
                                    list.Add(eid);
                                }
                            }
                        }
                        if (list.Count() > 5)
                        {
                            throw new FileValidationException(FileValidationException.Cause.backend, "please Enter only 5 FOPM EID" + " in Row Number " + rowCount);
                        }
                        Dictionary<string, string> userDetails = GetUserInfoForMultipleEIDAsync(row["FOPM EID"].ToString()).Result;
                        if (userDetails == null)
                        {
                            throw new FileValidationException(FileValidationException.Cause.backend, "FOPM EID is invalid in Row Number " + rowCount);
                        }
                        else
                        {
                            Fopm_name = userDetails["UserName"];
                            Fopm_emailId = userDetails["EmailId"];
                        }
                    }
                    
                }
                if (string.IsNullOrEmpty(row["GES Delivery Group"].ToString().Trim()))
                {
                    throw new FileValidationException(FileValidationException.Cause.backend, "GES Delivery Group is Missing in Row Number " + rowCount);
                }
                else
                {
                    RegionId = gesgroupdeliveryRepository.GetRegionIdByRegionName(row["GES Delivery Group"].ToString());
                    if (RegionId == 0)
                    {
                        throw new FileValidationException(FileValidationException.Cause.backend, "GES Delivery Group is invalid in Row Number " + rowCount);
                    }
                }

                if (string.IsNullOrEmpty(row["GES Group"].ToString().Trim()))
                {
                    throw new FileValidationException(FileValidationException.Cause.backend, "GES Group is Missing in Row Number " + rowCount);
                }
                else
                {
                    gesGroupId = gesGroupRepository.GetGESGroupIDByCountryAndSubGroup(Lob, row["GES Group"].ToString());
                    if (gesGroupId == 0)
                    {
                        throw new FileValidationException(FileValidationException.Cause.backend, "GES LOB in tool and GES Group is not matching in Row Number " + rowCount);
                    }

                }
                if (string.IsNullOrEmpty(row["Product / Platform"].ToString().Trim()))
                {
                    throw new FileValidationException(FileValidationException.Cause.backend, "Product / Platform is Missing in Row Number " + rowCount);
                }
                else
                {
                    productId = productRepository.GetProductIdByProductName(row["Product / Platform"].ToString());
                    if (productId == 0)
                    {
                        throw new FileValidationException(FileValidationException.Cause.backend, "Product / Platform is invalid in Row Number " + rowCount);
                    }
                    else
                    {
                        if (!gESGroupProductMappingRepository.checkGroupIdAndProductId(gesGroupId, productId))
                        {
                            throw new FileValidationException(FileValidationException.Cause.backend, "Product / Platform is Not Associated With GES Group in Row Number " + rowCount);
                        }
                    }

                }
                if (!string.IsNullOrEmpty(row["Attribute to"].ToString().Trim()))
                {
                    AttributeToId = attributeToRepository.GetAttributeToIdByAttributeToName(row["Attribute to"].ToString());
                    if (AttributeToId == 0)
                    {
                        throw new FileValidationException(FileValidationException.Cause.backend, "Attribute to is invalid in Row Number " + rowCount);
                    }
                }

                OttrMonitoring ottr = new OttrMonitoring();
                ottr.gespmEID = row["GES_PM_EID"].ToString();
                ottr.ProjectId = row["Project ID"].ToString();
                ottr.ProjectName = row["Project Name"].ToString();
                ottr.Deliverables = row["Deliverables / Milestone"].ToString();
                ottr.BaseDate = DateTime.Parse(row["Base Date"].ToString());
                ottr.PlanDate = DateTime.Parse(row["Plan Date"].ToString());
                ottr.ActualDate = String.IsNullOrEmpty(row["Actual Date"]?.ToString()) ? (DateTime?)null : DateTime.Parse(row["Actual Date"]?.ToString());
                ottr.GESGroupId = gesGroupId;
                ottr.RegionId = RegionId;
                ottr.SurveyRequest = row["Survey Request"].ToString();
                ottr.ProductId = productId;
                ottr.AttributeToId = AttributeToId;
                ottr.TrackingToDesignInput = row["Tracking to Design Input"]?.ToString();
                ottr.Comments = row["Comments / Remarks"]?.ToString();
                if (!String.IsNullOrEmpty(row["Internal / External Attribute"].ToString()))
                {
                    ottr.InternalExternalAttributeId = internalExternalAttributeToRepository.GetAllAttributeToInformation().Result.Where(x => x.InternalExternalAttributeToName.ToString().Trim().ToUpper().Equals(row["Internal / External Attribute"].ToString().Trim().ToUpper())).FirstOrDefault().InternalExternalAttributeToId;
                }
                if (ottr.SurveyRequest.ToLower().Equals("Required".ToLower()))
                {
                    
                    ottr.surveyType = row["Survey Type"]?.ToString();
                    
                    
                    if (ottr.surveyType.ToLower().Replace(" ", "").Equals("Front office".ToLower().Replace(" ", "")))
                    {
                        if (!string.IsNullOrEmpty(row["GES Resource EID"].ToString()))
                        {
                            ottr.gesResourceId = row["GES Resource EID"].ToString();
                            ottr.gesResourceName = GESResource_name;
                        }
                    }
                    int poleId = poleRepository.GetPoleIdByPoleName(row["Pole"].ToString());
                    if (poleId == 0)
                    {
                        throw new FileValidationException(FileValidationException.Cause.backend, "Pole is invalid in Row Number " + rowCount);
                    }
                    int regionId = regionRepository.GetRegionIdByRegionName(row["Region"].ToString());
                    if (regionId == 0)
                    {
                        throw new FileValidationException(FileValidationException.Cause.backend, "Region is invalid in Row Number " + rowCount);
                    }
                    int countryId = countryRepository.GetCountryIdByCountryName(row["Country"].ToString());
                    if (countryId == 0)
                    {
                        throw new FileValidationException(FileValidationException.Cause.backend, "Country is invalid in Row Number " + rowCount);
                    }
                    if (!regionRepository.checkPoleAndRegion(poleId, regionId))
                    {
                        throw new FileValidationException(FileValidationException.Cause.backend, "Please enter Valid pole and Region in Row Number " + rowCount);
                    }
                    if (!regionRepository.checkPoleAndRegion(poleId, regionId))
                    {
                        throw new FileValidationException(FileValidationException.Cause.backend, "Please enter Valid pole and Region in Row Number " + rowCount);
                    }
                    if (!regionCountryMappingRepository.checkRegionAndCountry(regionId, countryId))
                    {
                        throw new FileValidationException(FileValidationException.Cause.backend, "Please enter Valid pole and Region and Countrry in Row Number " + rowCount);
                    }
                    ottr.SpendPlan_Pole = poleId;
                    ottr.SpendPlan_Region = regionId;
                    ottr.SpendPlan_Country = countryId;
                }
                if (!string.IsNullOrEmpty(row["FOPM EID"].ToString()))
                {
                    ottr.FOPMId = row["FOPM EID"].ToString();
                    ottr.FOPMName = Fopm_name;
                    ottr.FOPMEmailID = Fopm_emailId;
                }
                ottr.GESPM = GESPM_name;
                ottr.CreatedBy = userEID;
                ottr.ModifiedBy = userEID;
                ottr.CreatedOn = DateTime.Now;
                ottr.ModifiedOn = DateTime.Now;
                ottr.IsActive = true;
                ottr.OttrId = 0;
                ottr.Month = ottr.PlanDate.ToString("MMM-yyyy");
                ottrMonitorings.Add(ottr);
                rowCount++;
            }
            if (ottrMonitorings.Count() > 0)
            {
                await ottrMonitoringRepository.AddOTTRInformationAll(ottrMonitorings);
            }
            return true;
        }
    

        public async Task<Dictionary<string, string>> getUserDetails(string EID)
        {
            if (!String.IsNullOrEmpty(EID))
            {
                UserPeopleSoftController user = new UserPeopleSoftController(configuration);
                var userName = await user.GetUserInformationForUpload(EID) as Microsoft.AspNetCore.Mvc.OkObjectResult;
                 Newtonsoft.Json.Linq.JObject result = (Newtonsoft.Json.Linq.JObject)userName.Value;
                //if (result !=null && result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["numrows"].ToString() != "0")
                //{
                //    Dictionary<string, string> UserDetails = new Dictionary<string, string>();
                //    string firstName = result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["row"]["A.PREF_FIRST_NAME"].ToString();
                //    string lastName = result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["row"]["A.LAST_NAME"].ToString();
                //    UserDetails.Add("UserName", firstName + " " + lastName);
                //    UserDetails.Add("EmailId", result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["row"]["A.EMAIL_ADDR"].ToString());
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

        public async Task<Dictionary<string, string>> GetUserInfoForMultipleEIDAsync(string EID)
        {
            try
            {
                string[] ListOfEID = EID.Split(";");
                List<string> FOPMName = new List<string>();
                List<string> FOPMEmailId = new List<string>();
                foreach (string eid in ListOfEID)
                {
                    if (!string.IsNullOrEmpty(eid))
                    {
                        UserPeopleSoftController user = new UserPeopleSoftController(configuration);
                        var userName = await user.GetUserInformationForUpload(eid.ToUpper()) as Microsoft.AspNetCore.Mvc.OkObjectResult;
                        Newtonsoft.Json.Linq.JObject result = (Newtonsoft.Json.Linq.JObject)userName.Value;

                        if (result != null && result["data"]?["query"]?["numrows"]?.ToString() != "0")
                        {
                            var rows = result["data"]["query"]["rows"];
                            
                            string firstName = rows[0]["PREF_FIRST_NAME"].ToString();
                            string lastName = rows[0]["LAST_NAME"].ToString();
                            FOPMName.Add(firstName + " " + lastName);
                            FOPMEmailId.Add(rows[0]["EMAIL_ADDR"].ToString());
                        }

                        //if (result !=null && result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["numrows"].ToString() != "0")
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


        public DataTable ConvertExcelToDataTable(string filePath)
        {
            DataTable dt = new DataTable();
            List<OttrMonitoring> ottrMonitorings = new List<OttrMonitoring>();
            using (XLWorkbook workBook = new XLWorkbook(filePath))
            {
                //Read the first Sheet from Excel file.

                IXLWorksheet workSheet = workBook.Worksheet(1);
                bool firstRow = true;

                foreach (IXLRow row in workSheet.RowsUsed())
                {
                    if (firstRow)
                    {
                        foreach (IXLCell cell in row.Cells())
                        {
                            dt.Columns.Add(cell.Value.ToString());
                        }
                        firstRow = false;
                    }
                    else
                    {
                        if (!row.IsEmpty())
                        {
                            dt.Rows.Add();
                            int i = 0; foreach (IXLCell cell in row.Cells(1, dt.Columns.Count))
                            {

                                dt.Rows[dt.Rows.Count - 1][i] = cell.Value?.ToString();
                                i++;
                            }
                        }
                    }
                }
            }
            return dt;
        }
     }
}