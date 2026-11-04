using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OTTR.Models;
using OTTR.Repositories;

namespace OTTR.Services
{
    public interface IOttrMonitoringService
    {
        Task<IEnumerable<OttrMonitoring>> GetListOfOttrMonitoringInfo(int GESSroupID,string Country, List<string> Roles);
        Task<OttrMonitoring> GetSpecificOttrMonitoringInfoByOttrId(int ottrId);

        Task<List<OttrMonitoring>> DeleteOTTRMonitoring(OTTRList OttrId,string UserId);
        Task<OttrMonitoring> AddOttrMonitoringInfo(OttrMonitoring ottrMonitoringList);
        Task<OttrMonitoring> UpdateOttrMonitoringInfo(OttrMonitoring ottrMonitoring);
        Task<List<OttrMonitoringModel>> CreateListOfOttrMonitoringForExcel(Dictionary<string, Filers> ottrFilters,int GESSroupID,string country, List<string> listOfRoles);
        Task<IEnumerable<OttrMonitoring>> GetAllOTTRInformation();
    }
    public class OttrMonitoringService : IOttrMonitoringService
    {
        private readonly IOttrMonitoringRepository ottrMonitoringRepository;
        private readonly IAttributeToRepository attributeToRepository;
        private readonly IInternalExternalAttributeToRepository internalExternalAttributeToRepository;
        private readonly IGESGroupRepository gesGroupRepository;
        private readonly IGesgroupdeliveryRepository gesgroupdeliveryRepository;
        private readonly IProductRepository productRepository;
        private readonly IPoleRepository poleRepository;
        private readonly IRegionRepository regionRepository;
        private readonly ICountryRepository countryRepository;

        public OttrMonitoringService(IPoleRepository poleRepository, IRegionRepository regionRepository, ICountryRepository countryRepository, IOttrMonitoringRepository ottrMonitoringRepository, IAttributeToRepository attributeToRepository, IGESGroupRepository gesGroupRepository, IGesgroupdeliveryRepository gesgroupdeliveryRepository, IProductRepository productRepository, IInternalExternalAttributeToRepository internalExternalAttributeToRepository)
        {
            this.ottrMonitoringRepository = ottrMonitoringRepository;
            this.attributeToRepository = attributeToRepository;
            this.gesGroupRepository = gesGroupRepository;
            this.gesgroupdeliveryRepository = gesgroupdeliveryRepository;
            this.productRepository = productRepository;
            this.poleRepository = poleRepository;
            this.regionRepository = regionRepository;
            this.countryRepository = countryRepository;
            this.internalExternalAttributeToRepository = internalExternalAttributeToRepository;
        }

        //Adding New OttrMonitoring Information
        public async Task<OttrMonitoring> AddOttrMonitoringInfo(OttrMonitoring ottrMonitoring)
        {
            try
            {
                if (ottrMonitoring.ActualDate != null)
                {
                    ottrMonitoring.ActualDateModifiedBy = ottrMonitoring.CreatedBy;
                    ottrMonitoring.ActualDateModifiedOn = DateTime.Now;
                }
                OttrMonitoring addedOttrMonitoringInfo = await ottrMonitoringRepository.AddOTTRInformation(ottrMonitoring);
                return addedOttrMonitoringInfo;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //Get the List of All OttrMonitoring Information
        public async Task<IEnumerable<OttrMonitoring>> GetListOfOttrMonitoringInfo(int GESSroupID, string Country, List<string> Roles)
        {
            try
            {
                var listOfOttrMonitoringInfo = await ottrMonitoringRepository.GetAllOTTRInformation(GESSroupID, Country, Roles);
                return listOfOttrMonitoringInfo;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //Get Particular OttMonitoring Record by OttrId
        public async Task<OttrMonitoring> GetSpecificOttrMonitoringInfoByOttrId(int ottrId)
        {
            try
            {
                var ottrMonitoringInformation = await ottrMonitoringRepository.GetSpecificOttrInfo(ottrId);
                return ottrMonitoringInformation;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //Update Particular OttrMonitoring Record 
        public async Task<OttrMonitoring> UpdateOttrMonitoringInfo(OttrMonitoring ottrMonitoring)
        {
            try
            {

                var updatedOttrMonitoringInfo = await ottrMonitoringRepository.UpdateOttrInformation(ottrMonitoring);
                return updatedOttrMonitoringInfo;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //Create Ottr Monitoring Model for Excel
        public async Task<List<OttrMonitoringModel>> CreateListOfOttrMonitoringForExcel(Dictionary<string, Filers> ottrFilters, int gesGroupId, string country, List<string> listOfRoles)
        {
            List<OttrMonitoringModel> listOfOttrMonitoringModel = new List<OttrMonitoringModel>();
            try
            {

                return await ottrMonitoringRepository.GetAllOTTRInformationWithFilters(ottrFilters, gesGroupId, country, listOfRoles);


                //foreach (var ottrMonitoring in listOfOttrMonitoringInfo)
                //{
                //    OttrMonitoringModel model = new OttrMonitoringModel();
                //    model.OttrId = ottrMonitoring.OttrId;
                //    model.GESPM = ottrMonitoring.GESPM;
                //    model.gespmEID = ottrMonitoring.gespmEID;
                //    model.ProjectId = ottrMonitoring.ProjectId;
                //    model.ProjectName = ottrMonitoring.ProjectName;
                //    model.Deliverables = ottrMonitoring.Deliverables;
                //    model.Month = ottrMonitoring.Month;
                //    model.BaseDate = ottrMonitoring.BaseDate;
                //    model.PlanDate = ottrMonitoring.PlanDate;
                //    model.ActualDate = ottrMonitoring.ActualDate;
                //    model.GESGroup = await gesGroupRepository.GetGESSubGroupNameByGESGroupId(ottrMonitoring.GESGroupId);
                //    //model.Region = ottrMonitoring.RegionId!=0 ?await gesgroupdeliveryRepository.GetRegionNameByRegionId(ottrMonitoring.RegionId):"0";
                //    //model.Product = await productRepository.GetProductNameByProductId(ottrMonitoring.ProductId);
                //    model.Difference = model.ActualDate == null ? 0 : (int)(BusinessDaysUntil(model.PlanDate, model.ActualDate.Value, new DateTime[0]));
                //    //model.AttributeTo = ottrMonitoring.AttributeToId == null ? "" : await attributeToRepository.GetAttributeToNameByAttributeToId(ottrMonitoring.AttributeToId.Value);
                //    model.InternalExternalAttributeId = ottrMonitoring.InternalExternalAttributeId == null ? "" : await internalExternalAttributeToRepository.GetAttributeToNameByAttributeToId(ottrMonitoring.InternalExternalAttributeId.Value);
                //    model.TrackingToDesignInput = ottrMonitoring.TrackingToDesignInput;
                //    model.Comments = ottrMonitoring.Comments;
                //    model.CustomerSurvey = ottrMonitoring.CustomerSurvey;
                //    model.FOPMName = ottrMonitoring.FOPMName;
                //    model.FOPMId = ottrMonitoring.FOPMId;
                //    // model.FOPMEmailId = ottrMonitoring.FOPMEmailID;
                //    model.CustomerSurveyTemplate = ottrMonitoring.CustomerSurveyTemplate;
                //    model.SurveyRequest = ottrMonitoring.SurveyRequest;
                //    model.SpendPlan_Pole = ottrMonitoring.SpendPlan_Pole == null ? "" : await poleRepository.GetPoleNameByPoleId(ottrMonitoring.SpendPlan_Pole);
                //    model.SpendPlan_Region = ottrMonitoring.SpendPlan_Region == null ? "" : await regionRepository.GetRegionNameByRegionId(ottrMonitoring.SpendPlan_Region);
                //    model.SpendPlan_Country = ottrMonitoring.SpendPlan_Country == null ? "" : await countryRepository.GetCountryNameByCountryId(ottrMonitoring.SpendPlan_Country);
                //    model.GESCenter = await gesGroupRepository.GetGESGroupNameByGESGroupId(ottrMonitoring.GESGroupId);
                //    model.surveyType = ottrMonitoring.surveyType;
                //    model.gesResourceId = ottrMonitoring.gesResourceId;
                //    model.gesResourceName = ottrMonitoring.gesResourceName;
                //    model.CreatedBy = ottrMonitoring.CreatedBy;
                //    model.CreatedOn = ottrMonitoring.CreatedOn;
                //    listOfOttrMonitoringModel.Add(model);
                //}
                //return listOfOttrMonitoringModel;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public async Task<List<OttrMonitoring>> DeleteOTTRMonitoring(OTTRList OttrIdList, string UserEID)
        {
            try
            {
                List<OttrMonitoring> ottrMonitorings = new List<OttrMonitoring>();
                foreach (int OttrId in OttrIdList.OttrIds)
                {
                    var ottrMonitoringInfo = await GetSpecificOttrMonitoringInfoByOttrId(OttrId);

                    if (ottrMonitoringInfo != null)
                    {
                        ottrMonitoringInfo.IsActive = false;
                        ottrMonitoringInfo.ModifiedBy = UserEID;
                        ottrMonitoringInfo.ModifiedOn = DateTime.Now;
                        ottrMonitorings.Add(ottrMonitoringInfo);
                    }
                    else
                    {
                        return null;
                    }
                }
                if (ottrMonitorings.Count > 0)
                {
                    return await ottrMonitoringRepository.Delete(ottrMonitorings);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                return null;
            }

        }
        //Calculate Only Business Working Days
        private int BusinessDaysUntil(DateTime firstDay, DateTime lastDay, params DateTime[] holidays)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            bool negativeNumber = false;
            if (firstDay > lastDay)
            {
                negativeNumber = true;
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

        public async Task<IEnumerable<OttrMonitoring>> GetAllOTTRInformation()
        {
            try
            {

                var updatedOttrMonitoringInfo = await ottrMonitoringRepository.GetAllOTTRInformation();
                return updatedOttrMonitoringInfo;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}