using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OTTR.Models;
using OTTR.Repositories;
using Newtonsoft.Json;

namespace OTTR.Services
{
    public interface IEmailNotificationService
    {
        Task DoWork(CancellationToken cancellationToken);
       // Task<UserInfo> getUserInfo(string coordinator);
        Task<object> sendMail(string from, string to, string cc, string subject, string body);
        //Task<NotificationEvents> GetNotificationEvents(string events);
        //Task<string> bodyReplaceAsync(string body, ServiceRequest serviceRequest, UserInfo primaryCoordinator, UserInfo secondaryCoordinator, UserInfo requestor, UserInfo emailIdOfLoggedByGESPersonId, UserInfo GESEngineer, ServiceStatusComments serviceStatusComments, UserInfo LoggedInUser, ServiceRequestGetModel obj);
        //string emailIdReplace(string cC, UserInfo primaryCoordinator, UserInfo secondaryCoordinator, UserInfo requestor, UserInfo emailIdOfLoggedByGESPersonId, UserInfo GESEngineer);
    }
       
    public class EmailNotificationService : IEmailNotificationService
    {       
        private readonly ILogger<EmailNotificationService> logger;
        private readonly IConfiguration configuration;
        private readonly Dictionary<string, string> requestProcessorConfig;
        private readonly IOttrMonitoringRepository ottrMonitoringRepository;

        public EmailNotificationService(ILoggerFactory logger, IConfiguration configuration, IOttrMonitoringRepository ottrMonitoringRepository)
        {
            this.configuration = configuration;
            this.requestProcessorConfig = configuration.GetSection("RequestProcessorConfig").Get<Dictionary<string, string>>();
            this.ottrMonitoringRepository = ottrMonitoringRepository;
            this.logger = logger.CreateLogger<EmailNotificationService>();
        }

        public async Task DoWork(CancellationToken cancellationToken)
        {

              await executeMailJob();
            await Task.Delay(1000 * 20, cancellationToken);
        }

        private async Task executeMailJob()
        {
            try
            {
                //logger.LogInformation("In Email Service");
                List<OttrMonitoring> ottrList = ottrMonitoringRepository.GetOTTRList();
               
                List<string> OttrIdCreatedBY = ottrList.Select(x => x.CreatedBy).Distinct().ToList();

                //Dictionary<int,Tuple<List<OttrMonitoring>, List<OttrMonitoring>> ottrs = new Tuple<List<OttrMonitoring>, List<OttrMonitoring>>();
                var ottrs = new Dictionary<string, Tuple<List<OttrMonitoring>, List<OttrMonitoring>>>();

                foreach (var createdBy in OttrIdCreatedBY)
                {
                    List<OttrMonitoring> ottrListBYuserForUpComing = ottrList.Where(x => (x.PlanDate >= DateTime.Now) && x.CreatedBy.Equals(createdBy)).ToList();
                    List<OttrMonitoring> ottrListBYuserForGoan = ottrList.Where(x => (x.PlanDate < DateTime.Now) && x.CreatedBy.Equals(createdBy)).ToList();
                    ottrs.Add(createdBy, new Tuple<List<OttrMonitoring>, List<OttrMonitoring>>(ottrListBYuserForUpComing, ottrListBYuserForGoan));
                }
                string ExpiredOttrMessage = "";
                string UpcomingOttrMessage = "";
                string NumberOfDaysBeforeEmailToSent = "";
                string Subject = "";
                requestProcessorConfig.TryGetValue("DateOverOttrMessage", out ExpiredOttrMessage);
                requestProcessorConfig.TryGetValue("UpcomingOttrMessage", out UpcomingOttrMessage);
                requestProcessorConfig.TryGetValue("NumberOfDaysBeforeEmailToSent", out NumberOfDaysBeforeEmailToSent);
                requestProcessorConfig.TryGetValue("Subject", out Subject);
            /*    foreach (var ottr in ottrs)
                {
                    
                   
                    int totalCountForGoanOTTR = 0;
                    int totalCountForUpcomingOTTR = 0;
                    string HtmlBody = null;
                    UserInfo LoggedByGESPersonId = null;
                    LoggedByGESPersonId = await getUserInfo(ottr.Key);
                    HtmlBody = "<html><head><style>body {font-family:arial;font-size:10pt;} td {font-family:arial;font-size:9pt;}</style></head>" +
                                    "<div><p style=\"font-family:arial;font-size:10pt\">" + "Hi " + LoggedByGESPersonId.NameFirst + " " + LoggedByGESPersonId.NameLast + ",</p></div>";
                    if (ottr.Value.Item2.Count > 0)
                    {
                        foreach (var ottrlist in ottr.Value.Item2)
                        {

                            if (totalCountForGoanOTTR == 0)
                            {

                                HtmlBody += "<p style=\"font-family:arial;font-size:10pt\">"+ ExpiredOttrMessage +"</p>" +

                                    "<table style=\"border-collapse: collapse;width: 100%\">" +
                                     "<tr>" +
                                     "<th style=\"border: 1px solid #ddd; padding: 8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #336699;color: white\">OTTR ID</th>" +
                                     "<th style=\"border: 1px solid #ddd; padding: 8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #336699;color: white\">GES PM/EM</th>" +
                                     "<th style=\"border: 1px solid #ddd; padding: 8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #336699;color: white\">PROJECT ID </th>" +
                                     "<th style=\"border: 1px solid #ddd; padding: 8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #336699;color: white\">PROJECT NAME </th>" +
                                     "<th style=\"border: 1px solid #ddd; padding: 8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #336699;color: white\">BASE DATE </th>" +
                                     "<th style=\"border: 1px solid #ddd; padding: 8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #336699;color: white\">PLAN DATE </th>" +
                                     "<th style=\"border: 1px solid #ddd; padding: 8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #336699;color: white\">ACTUAL DATE </th>" +
                                     "</tr>";
                            }
                            HtmlBody += "<tr>" +
                                "<td style=\"border: 1px solid #ddd; padding: 8px\">" + ottrlist.OttrId + "</font></td>" +
                                 "<td style=\"border: 1px solid #ddd; padding: 8px\">" + ottrlist.GESPM + "</font></td>" +
                                  "<td style=\"border: 1px solid #ddd; padding: 8px\">" + ottrlist.GESGroupId + "</font></td>" +
                                   "<td style=\"border: 1px solid #ddd; padding: 8px\">" + ottrlist.ProjectName + "</font></td>" +
                                    "<td style=\"border: 1px solid #ddd; padding: 8px\">" + ottrlist.BaseDate.ToString("dd-MMM-yy") + "</font></td>" +
                                     "<td style=\"border: 1px solid #ddd; padding: 8px\">" + ottrlist.PlanDate.ToString("dd-MMM-yy") + "</font></td>" +
                                      "<td style=\"border: 1px solid #ddd; padding: 8px\">" + " " + "</font></td>" +
                                  "</tr>";
                            totalCountForGoanOTTR++;

                        }
                    }
                    if (totalCountForGoanOTTR > 0)
                    {
                        HtmlBody += "</table>";
                    }
                    if (ottr.Value.Item1.Count > 0)
                    {
                        foreach (var ottrlist in ottr.Value.Item1)
                        {
                            var currentDate = DateTime.Now;
                            var planDate = ottrlist.PlanDate;
                            var totalDiffernce = (planDate - currentDate).TotalDays;
                            if (totalDiffernce > Int32.Parse(NumberOfDaysBeforeEmailToSent)) continue;
                            if (totalCountForUpcomingOTTR == 0)
                            {
                               
                                HtmlBody += "<br><br><p style=\"font-family:arial;font-size:10pt\">"+ UpcomingOttrMessage + "</p>" +

                                    "<table style=\"border-collapse: collapse;width: 100%\">" +
                                     "<tr>" +
                                     "<th style=\"border: 1px solid #ddd; padding: 8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #336699;color: white\">OTTR ID</th>" +
                                     "<th style=\"border: 1px solid #ddd; padding: 8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #336699;color: white\">GES PM/EM</th>" +
                                     "<th style=\"border: 1px solid #ddd; padding: 8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #336699;color: white\">PROJECT ID </th>" +
                                     "<th style=\"border: 1px solid #ddd; padding: 8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #336699;color: white\">PROJECT NAME </th>" +
                                     "<th style=\"border: 1px solid #ddd; padding: 8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #336699;color: white\">BASE DATE </th>" +
                                     "<th style=\"border: 1px solid #ddd; padding: 8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #336699;color: white\">PLAN DATE </th>" +
                                     "<th style=\"border: 1px solid #ddd; padding: 8px;padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #336699;color: white\">ACTUAL DATE </th>" +
                                     "</tr>";
                            }
                            
                            HtmlBody += "<tr>" +
                                "<td style=\"border: 1px solid #ddd; padding: 8px\">" + ottrlist.OttrId + "</font></td>" +
                                 "<td style=\"border: 1px solid #ddd; padding: 8px\">" + ottrlist.GESPM + "</font></td>" +
                                  "<td style=\"border: 1px solid #ddd; padding: 8px\">" + ottrlist.GESGroupId + "</font></td>" +
                                   "<td style=\"border: 1px solid #ddd; padding: 8px\">" + ottrlist.ProjectName + "</font></td>" +
                                    "<td style=\"border: 1px solid #ddd; padding: 8px\">" + ottrlist.BaseDate.ToString("dd-MMM-yy") + "</font></td>" +
                                     "<td style=\"border: 1px solid #ddd; padding: 8px\">" + ottrlist.PlanDate.ToString("dd-MMM-yy") + "</font></td>" +
                                      "<td style=\"border: 1px solid #ddd; padding: 8px\">" + " " + "</font></td>" +
                                  "</tr>";
                            totalCountForUpcomingOTTR++;

                        }
                    }
                    HtmlBody += "</table></body></html> ";
                    if (totalCountForUpcomingOTTR > 0 || totalCountForGoanOTTR>0)
                    {
                        //logger.LogInformation("email-ID", LoggedByGESPersonId.EmailAddressOffice);
                            await sendMail(LoggedByGESPersonId.EmailAddressOffice, LoggedByGESPersonId.EmailAddressOffice, "",Subject, HtmlBody);
                        //logger.LogInformation("Mail Send Called");
                    }
                }
            */
            }
            catch (Exception e)
            {
                //logger.LogInformation("In Email Service error occcured"+e);
                logger.LogError("Error occured", e);
            }
        }

        public async Task<object> sendMail(string from, string to, string cc, string subject, string body)
        {
            Email email = new Email();
            email.from = from;
            email.to = to;
            email.cc = cc;
            email.bcc = "shital.sakhare@honeywell.com";
            email.subject = subject;
            email.body = body;
            email.ApplicationName = "Deliverable Log + OTTR";

            RequestProcessor rp = new RequestProcessor();
            string app = "";
            string action = "";
            requestProcessorConfig.TryGetValue("EmailNotificationName", out app);
            requestProcessorConfig.TryGetValue("EmailNotificationEndPoint", out action);
            rp.app = app;
            rp.action = action;

            // rp.data = "{\"subject\":\"Service Request updated - SR ID - 53\",\"from\":null,\"to\":\"sagarpawar9912@gmail.com;abhijeet.adsod@mindnerves.com\",\"body\":\"< h2 > Dear 100,</ h2 >< br > < h2 > The service request assigned to you, with SR ID -53 has been updated by 101 on 01/01/0001 00:00:00 </ h2 >< br >\",\"cc\":\"shipramindnerves022@gmail.com;abhikale100@gmail.com\"}";
            rp.data = JsonConvert.SerializeObject(email);
            rp.requestType = "POST";
            string secretKey = "";
            requestProcessorConfig.TryGetValue("SecretKey", out secretKey);
            rp.secretKey = secretKey;
            object response = await PostAsync(rp);
            //logger.LogInformation("response from eamil" + response.ToString());
            return response;

        }

        public async Task<UserInfo> getUserInfo(string coordinator)
        { 
            try
            {
               // logger.LogInformation("In user Function");
                RequestProcessor rp = new RequestProcessor();
                string app = "";
                string action = "";
                requestProcessorConfig.TryGetValue("AccessControlName", out app);
                requestProcessorConfig.TryGetValue("GetUserInfoEndPoint", out action);
                rp.app = app;
                rp.action = action + coordinator;
                // rp.data = "";
                rp.requestType = "GET";
                string secretKey = "";
                requestProcessorConfig.TryGetValue("SecretKey", out secretKey);
                rp.secretKey = secretKey;
                object response = await PostAsync(rp);
                Dictionary<string, object> userobj = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(response));

                UserInfo user = JsonConvert.DeserializeObject<UserInfo>(Convert.ToString(userobj.GetValueOrDefault("employee")));
                if (user != null)
                {
                    //logger.LogInformation("user Response",user.ToString());
                    return user;
                }
                return null;

            }
            catch { return null; }
        }

        public async Task<Object> PostAsync(RequestProcessor rp)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var json_str = JsonConvert.SerializeObject(rp);
                    StringContent Jsonbody = null;
                    if (json_str != null)
                    {
                        Jsonbody = new StringContent(json_str.ToString(), Encoding.UTF8, "application/json");
                    }
                    string redirectURL = "";
                    requestProcessorConfig.TryGetValue("EndPointURL", out redirectURL);

                    var result = client.PostAsync(redirectURL, Jsonbody).Result;
                    string content = await result.Content.ReadAsStringAsync();
                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        return content;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private string getFLFC(int? flFcID)
        {
            if (flFcID != null && flFcID == 1)
            {
                return "Frontlog";
            }
            else if (flFcID != null && flFcID == 2)
            {
                return "Forecast";
            }
            else {
               return "FL/FC";
            }
        }

        //Calculate Only Business Working Days
        private int BusinessDaysUntil(DateTime firstDay, DateTime lastDay, params DateTime[] holidays)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            bool negativeNumber = true;
            if (firstDay > lastDay)
            {
                negativeNumber = false;
                DateTime temp = firstDay.Date;
                firstDay = lastDay.Date;
                lastDay = temp;
            }
            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days;
            int fullWeekCount = businessDays / 7;
            // find out if there are weekends during the time exceedng the full weeks
            if (businessDays > fullWeekCount * 7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                int firstDayOfWeek = firstDay.DayOfWeek == DayOfWeek.Sunday
                    ? 7 : (int)firstDay.DayOfWeek;
                int lastDayOfWeek = lastDay.DayOfWeek == DayOfWeek.Sunday
                    ? 7 : (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval
                    businessDays -= 1;
            }

            // subtract the weekends during the full weeks in the interval
            businessDays -= fullWeekCount + fullWeekCount;

            // subtract the number of holidays during the time interval
            foreach (DateTime holiday in holidays)
            {
                DateTime bh = holiday.Date;
                if (firstDay <= bh && bh <= lastDay)
                    --businessDays;
            }

            return negativeNumber ? businessDays : businessDays * -1;
        }

    }
}
