using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTTR.Models
{
    public class OttrMonitoringModel
    {
        public int OttrId { get; set; }
        public string GESPM { get; set; }
        public string gespmEID { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Deliverables { get; set; }
        public string Month { get; set; }
        public DateTime BaseDate { get; set; }
        public DateTime PlanDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public string ActualDateModifiedBy { get; set; }
        public DateTime? ActualDateModifiedOn { get; set; }
        public string GESGroup { get; set; }
        public string Region { get; set; }
        public string Product { get; set; }
        //public int Difference { get; set; }
        public string AttributeTo { get; set; }

        public string TrackingToDesignInput { get; set; }

        public string InternalExternalAttributeId { get; set; }
        public string Comments { get; set; }
        public string CustomerSurvey { get; set; }
        public string FOPMName { get; set; }
        public string FOPMEmailId { get; set; }

        public string FOPMId { get; set; }
        public string CustomerSurveyTemplate { get; set; }

        
        public string surveyFor { get; set; }

       
        public string exCustEmailId { get; set; }


        
        public string exCustName { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        

        public string SurveyRequest { get; set; }

        public string SpendPlan_Pole { get; set; }

        public string SpendPlan_Region { get; set; }

        public string SpendPlan_Country { get; set; }

        public string surveyType { get; set; }
        public string gesResourceId { get; set; }
        public string gesResourceName { get; set; }

        public string GESCenter { get; set; }
        public int? DeliveryHours { get; set; }
        public int? ReworkHours { get; set; }
        public string CreatedByEmail { get; set; }
        public string CreatedByManagerEmailId { get; set; }

        public string GESPMEmail { get; set; }
        public string GESPMManagerEmail { get; set; }
    }
}
