using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTTR.Models
{
    [Table("tblOTTR_Monitoring", Schema = "dbo")]
    public class OttrMonitoring
    {
        [Key]
       
        [Column("OTTR_ID", TypeName = "int")]
        public int OttrId { get; set; }
        [Column("GES_PM")]
        public string GESPM { get; set; }

        [Column("GES_PM_EID")]
        public string gespmEID { get; set; }

        public string ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string Deliverables { get; set; }

        public string Month { get; set; }

        public DateTime BaseDate { get; set; }

        [Column("Survey_Request")]
        public string SurveyRequest { get; set; }
        public DateTime PlanDate { get; set; }

        public DateTime? ActualDate { get; set; }

        public string ActualDateModifiedBy { get; set; }
        public DateTime? ActualDateModifiedOn { get; set; }

        [Column("GES_Group_Id")]
        public int GESGroupId { get; set; }

        public int RegionId { get; set; }

        public int ProductId { get; set; }

        public int? SpendPlan_Pole { get;set; }

        public int? SpendPlan_Region { get; set; }

        public int? SpendPlan_Country { get; set; }

        public int? AttributeToId { get; set; }

        public int? InternalExternalAttributeId { get; set; }

        public string Comments { get; set; }

        public string CustomerSurvey { get; set; }

      
        [Column("FO_PM_Name")]
        public string FOPMName { get; set; }

        [Column("FO_PM_ID")]
        public string FOPMId { get; set; }


        [Column("Survey_Type")]
        public string surveyType { get; set; }

        [Column("GES_Resource_EID")]
        public string gesResourceId { get; set; }

        [Column("GES_Resource_Name")]
        public string gesResourceName { get; set; }



        [Column("FO_PM_EmailID")]
        public string FOPMEmailID { get; set; }

        [Column("survey_For")]
        public string surveyFor { get; set; }

        [Column("exCust_EmailId")]
        public string exCustEmailId { get; set; }


        [Column("exCust_Name")]
        public string exCustName { get; set; }
        public string CustomerSurveyTemplate { get; set; }

        public string TrackingToDesignInput { get; set; }
       
        [Column(TypeName = "tinyint")]
        public bool IsActive { get; set; }

        public string CreatedBy {get; set;}

        public DateTime CreatedOn {get; set;}

        public string ModifiedBy {get; set;}

       
        public DateTime ModifiedOn {get; set;}

        public int? DeliveryHours { get; set; }
        public int? ReworkHours { get; set; }

        public string CreatedByEmail { get; set; }
        public string CreatedByManagerEmailId { get; set; }

        public string GESPMEmail { get; set; }
        public string GESPMManagerEmail { get; set; }
    }

}
