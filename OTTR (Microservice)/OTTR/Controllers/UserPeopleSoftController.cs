using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OTTR.Services;
using Microsoft.Extensions.Configuration;
using OTTR.Models;

namespace OTTR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserPeopleSoftController : BaseController
    {

        private readonly Dictionary<string, string> PeopleSoftApiConfig;
        private readonly IConfiguration configuration;

        public UserPeopleSoftController(IConfiguration configuration)
        {
            this.PeopleSoftApiConfig = configuration.GetSection("PeopleSoftApiConfig").Get<Dictionary<string, string>>();
        }

        //Get the List of All Region Information
        [HttpGet("{EID}")]
        public async Task<IActionResult> GetUserInformation(string EID)
        {
            try
            {
                var RoleList = HttpContext.Request.Query["RoleList"].ToString();
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

                JObject AccessToken = GetAccessToken();
                
                if (AccessToken != null)
                {
                    responseResource.code = 200;
                    responseResource.data = GetUserInfoForMultipleEID(AccessToken["access_token"].ToString(), EID.ToUpper());
                    return Ok(responseResource);
                }
                throw new Exception();


            }
            catch (Exception e)
            {
              
                return Error(e.Message, 500);
            }
        }

        public async Task<IActionResult> GetUserInformationForUpload(string EID)
        {
            try
            {
                JObject AccessToken = GetAccessToken();
                
                if (AccessToken != null)
                {
                    return Ok(GetUserInfo(AccessToken["access_token"].ToString(), EID.ToUpper()));
                }
                throw new Exception();


            }
            catch (Exception e)
            {
              
                return Error(e.Message, 500);
            }
        }



        public JObject GetAccessToken()
        {
            try
            {
                using (var httpClient = new System.Net.Http.HttpClient())
                {
                    //string url = "https://it.api.honeywell.com/v1/oauth/accesstoken?grant_type=client_credentials";
                    string url = "https://it.api.honeywell.com/v2/oauth/accesstoken";
                    //using (var request = new System.Net.Http.HttpRequestMessage(new System.Net.Http.HttpMethod("GET"), url))
                    //{
                    //    string ClientSecret = "";
                    //    string ClientId = "";
                    //    PeopleSoftApiConfig.TryGetValue("PeopleSoftApiclientSecret", out ClientSecret);
                    //    PeopleSoftApiConfig.TryGetValue("PeopleSoftApiClientId", out ClientId);
                    //    var contentList = new List<string>();
                    //    contentList.Add($"client_secret="+ ClientSecret);
                    //    contentList.Add($"client_id="+ ClientId);
                    //    request.Content = new System.Net.Http.StringContent(string.Join("&", contentList));

                    //    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
                    //    var response = httpClient.SendAsync(request).Result;
                    //    var data = response.Content.ReadAsStringAsync().Result;
                    //    var obj = JObject.Parse(data);
                    using (var request = new System.Net.Http.HttpRequestMessage(new System.Net.Http.HttpMethod("post"), url))
                    {
                        string ClientSecret = "";
                        string ClientId = "";
                        PeopleSoftApiConfig.TryGetValue("PeopleSoftApiclientSecret", out ClientSecret);
                        PeopleSoftApiConfig.TryGetValue("PeopleSoftApiClientId", out ClientId);
                        var contentList = new List<string>();
                        contentList.Add($"client_secret=" + ClientSecret);
                        contentList.Add($"client_id=" + ClientId);
                        contentList.Add("grant_type=client_credentials");
                        request.Content = new System.Net.Http.StringContent(string.Join("&", contentList));

                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
                        var response = httpClient.SendAsync(request).Result;
                        var data = response.Content.ReadAsStringAsync().Result;
                        var obj = JObject.Parse(data);
                        // var obj = JObject.Parse(data);
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            return obj;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }


            }
            catch (Exception e)
            {
                
                return  null;
            }
         }

        public string GetUserInfoForMultipleEID(string token,string EID)
        {
            try
            {
                string[] ListOfEID = EID.Split(";");
                List<string> FOPMName = new List<string>();
                foreach (string eid in ListOfEID)
                {

                    using (var httpClient = new System.Net.Http.HttpClient())
                    {
                        //string url = "https://it.api.honeywell.com/v1/employeeSearch?isconnectedquery=N&maxrows=0&prompt_psqueryname=QAS_H_IHAIL_DATA&prompt_uniquepromptname=BIND1&prompt_fieldvalue=" + eid;
                        string url = "https://it.api.honeywell.com/hw-orclehcm-sirva-sapgrc/v1/HON_HR_I_163/1.0/";
                        //using (var request = new System.Net.Http.HttpRequestMessage(new System.Net.Http.HttpMethod("GET"), url))
                        //{
                        //    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                        //    var response = httpClient.SendAsync(request).Result;
                        //    var data = response.Content.ReadAsStringAsync().Result;
                        //    var result = JObject.Parse(data);

                        using (var request = new System.Net.Http.HttpRequestMessage(new System.Net.Http.HttpMethod("POST"), url))
                        {
                            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                            // Create the JSON payload
                            var payload = new JObject
                            {
                                ["P_EMPLID"] = eid
                            };
                            request.Content = new System.Net.Http.StringContent(payload.ToString(), System.Text.Encoding.UTF8, "application/json");
                            var response = httpClient.SendAsync(request).Result;
                            var data = response.Content.ReadAsStringAsync().Result;
                            var result = JObject.Parse(data);

                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                //if (result != null && result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["numrows"].ToString() != "0")
                                //{
                                //    Dictionary<string, string> UserDetails = new Dictionary<string, string>();
                                //    string firstName = result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["row"]["A.PREF_FIRST_NAME"].ToString();
                                //    string lastName = result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["row"]["A.LAST_NAME"].ToString();
                                //    UserDetails.Add("UserName", firstName + " " + lastName);
                                //    UserDetails.Add("EmailId", result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["row"]["A.EMAIL_ADDR"].ToString());
                                //    FOPMName.Add(firstName + " " + lastName);
                                //}

                                if (result != null && result["data"]?["query"]?["numrows"]?.ToString() != "0")
                                {
                                    var rows = result["data"]["query"]["rows"];
                                    Dictionary<string, string> UserDetails = new Dictionary<string, string>();
                                    string firstName = rows[0]["PREF_FIRST_NAME"].ToString();
                                    string lastName = rows[0]["LAST_NAME"].ToString();
                                    UserDetails.Add("UserName", firstName + " " + lastName);
                                    UserDetails.Add("EmailId", rows[0]["EMAIL_ADDR"].ToString());
                                    //UserDetails.Add("EmailId", result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["row"]["A.EMAIL_ADDR"].ToString());
                                    FOPMName.Add(firstName + " " + lastName);
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
                    }
                    
                }
                return string.Join(";", FOPMName.ToArray());
            }
            catch (Exception e)
            {
               
                return null;
            }
        }

        public JObject GetUserInfo(string token, string EID)
        {
            try
            {
                using (var httpClient = new System.Net.Http.HttpClient())
                {
                    //string url = "https://it.api.honeywell.com/v1/employeeSearch?isconnectedquery=N&maxrows=0&prompt_psqueryname=QAS_H_IHAIL_DATA&prompt_uniquepromptname=BIND1&prompt_fieldvalue=" + EID;
                    string url = "https://it.api.honeywell.com/hw-orclehcm-sirva-sapgrc/v1/HON_HR_I_163/1.0/";

                    using (var request = new System.Net.Http.HttpRequestMessage(new System.Net.Http.HttpMethod("POST"), url))
                    {
                        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                        // Create the JSON payload
                        var payload = new JObject
                        {
                            ["P_EMPLID"] = EID
                        };
                        request.Content = new System.Net.Http.StringContent(payload.ToString(), System.Text.Encoding.UTF8, "application/json");
                        var response = httpClient.SendAsync(request).Result;
                        var data = response.Content.ReadAsStringAsync().Result;
                        var obj = JObject.Parse(data);
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            return obj;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }


            }
            catch (Exception e)
            {

                return null;
            }
        }

    }
}