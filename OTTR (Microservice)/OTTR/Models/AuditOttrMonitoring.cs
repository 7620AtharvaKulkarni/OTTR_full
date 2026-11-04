using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTTR.Models
{
    [Table("tblOTTR_AuditOTTRMonitoring", Schema = "dbo")]
    public class AuditOttrMonitoring
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Audit_Ottr_Id", TypeName = "int")]
        public int AuditOttrId { get; set; }

        [Column("OTTR_ID")]
        public int OttrId { get; set; }

        [Column("GES_PM")]
        public string GESPM { get; set; }

        [Column("GES_PM_EID")]
        public string gespmEID { get; set; }



        public string ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string Deliverables { get; set; }

        public string Month { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime BaseDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime PlanDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ActualDate { get; set; }

        [Column("GES_Group_Id", TypeName = "int")]
        public int GESGroupId { get; set; }

        [Column(TypeName = "int")]
        public int RegionId { get; set; }

        [Column(TypeName = "int")]
        public int ProductId { get; set; }

        [Column(TypeName = "int")]
        public int? AttributeToId { get; set; }

        public string Comments { get; set; }

        public string CustomerSurvey { get; set; }

        [Column("FO_PM_Name")]
        public string FOPMName { get; set; }

        [Column("FO_PM_ID")]
        public string FOPMID { get; set; }

        [Column("FO_PM_EmailID")]
        public string FOPMEmailID { get; set; }
        public string CustomerSurveyTemplate { get; set; }

        [Column(TypeName = "tinyint")]
        [Display(Name = "IsActive")]
        public bool IsActive { get; set; }

        [Display(Name = "CreatedBy")]
        public string CreatedBy { get; set; }

        [Column(TypeName = "datetime)")]
        [Display(Name = "CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "ModifiedBy")]
        public string ModifiedBy { get; set; }

        [Column(TypeName = "datetime")]
        [Display(Name = "ModifiedOn")]
        public DateTime ModifiedOn { get; set; }

        public string TrackingToDesignInput { get; set; }


        [Column(TypeName = "int")]
        public int? InternalExternalAttributeId { get; set; }

        public int? DeliveryHours { get; set; }
        public int? ReworkHours { get; set; }

        [Column("Survey_Request")]
        public string SurveyRequest { get; set; }

        public int? SpendPlan_Pole { get; set; }

        public int? SpendPlan_Region { get; set; }

        public int? SpendPlan_Country { get; set; }

        [Column("Survey_Type")]
        public string SurveyType { get; set; }

        [Column("GES_Resource_EID")]
        public string GESResourceEID { get; set; }

        [Column("GES_Resource_Name")]
        public string GESResourceName { get; set; }
        public AuditOttrMonitoring()
        {
            OttrId = 0;
            GESPM = null;
            gespmEID = null;
            ProjectId = null;
            ProjectName = null;
            Deliverables = null;
            Month = null;
            BaseDate = new DateTime();
            PlanDate = new DateTime();
            ActualDate = new DateTime();
            GESGroupId = 0;
            RegionId = 0;
            ProductId = 0;
            AttributeToId = 0;
            Comments = null;
            CustomerSurvey = null;
            FOPMName = null;
            FOPMID = null;
            FOPMEmailID = null;
            CustomerSurveyTemplate = null;
            IsActive = true;
            CreatedBy = null;
            CreatedOn = new DateTime();
            ModifiedBy = null;
            ModifiedOn = new DateTime();
            TrackingToDesignInput = null;
            InternalExternalAttributeId = 0;
            GESResourceEID = null;
            SurveyType = null;
            GESResourceName = null;
            DeliveryHours = null;
            ReworkHours = null;
            //CreatedByEmail = null;
            //CreatedByManagerEmailId = null;
        }

        public AuditOttrMonitoring(OttrMonitoring ottrMonitoring)
        {
            OttrId = ottrMonitoring.OttrId;
            GESPM = ottrMonitoring.GESPM;
            gespmEID = ottrMonitoring.gespmEID;
            ProjectId = ottrMonitoring.ProjectId;
            ProjectName = ottrMonitoring.ProjectName;
            Deliverables = ottrMonitoring.Deliverables;
            Month = ottrMonitoring.Month;
            BaseDate = ottrMonitoring.BaseDate;
            PlanDate = ottrMonitoring.PlanDate;
            ActualDate = ottrMonitoring.ActualDate;
            GESGroupId = ottrMonitoring.GESGroupId;
            RegionId = ottrMonitoring.RegionId;
            ProductId = ottrMonitoring.ProductId;
            AttributeToId = ottrMonitoring.AttributeToId;
            Comments = ottrMonitoring.Comments;
            CustomerSurvey = ottrMonitoring.CustomerSurvey;
            FOPMName = ottrMonitoring.FOPMName;
            FOPMID = ottrMonitoring.FOPMId;
            FOPMEmailID = ottrMonitoring.FOPMEmailID;
            CustomerSurveyTemplate = ottrMonitoring.CustomerSurveyTemplate;
            IsActive = ottrMonitoring.IsActive;
            CreatedBy = ottrMonitoring.CreatedBy;
            CreatedOn = ottrMonitoring.CreatedOn;
            ModifiedBy = ottrMonitoring.ModifiedBy;
            ModifiedOn = ottrMonitoring.ModifiedOn;
            TrackingToDesignInput = ottrMonitoring.TrackingToDesignInput;
            InternalExternalAttributeId = ottrMonitoring.InternalExternalAttributeId;
            SurveyRequest = ottrMonitoring.SurveyRequest;
            SpendPlan_Country = ottrMonitoring.SpendPlan_Country;
            SpendPlan_Pole = ottrMonitoring.SpendPlan_Pole;
            SpendPlan_Region = ottrMonitoring.SpendPlan_Region;
            SurveyType = ottrMonitoring.surveyType;
            GESResourceEID = ottrMonitoring.gesResourceId;
            GESResourceName = ottrMonitoring.gesResourceName;
            DeliveryHours = ottrMonitoring.DeliveryHours;
            ReworkHours = ottrMonitoring.ReworkHours;
            //CreatedByEmail = ottrMonitoring.CreatedByEmail;
            //CreatedByManagerEmailId = ottrMonitoring.CreatedByManagerEmailId;
        }
    }
}
