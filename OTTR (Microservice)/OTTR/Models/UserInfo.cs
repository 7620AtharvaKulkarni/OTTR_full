using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTTR.Models
{
    public class UserInfo 
    {
        public string EID { get; set; }
        public string NamePrefix { get; set; }
        public string NameFirst { get; set; }
        public string NameMiddle { get; set; }
        public string NameLast { get; set; }
        public string Country { get; set; }
        public string BusinessTitle { get; set; }
        public string CostCenterCode { get; set; }
        public string Pes_LocationCode { get; set; }
        public string Grade { get; set; }
        public DateTime? DateOfTermination { get; set; }
        public DateTime? DateOfRehire { get; set; }
        public string SupervisorEID { get; set; }
        public string HrContactEID { get; set; }
        public string SBG { get; set; }
        public string SBU { get; set; }
        public string SBE { get; set; }
        public string SBX { get; set; }
        public string OrgEntity { get; set; }
        public string EmailAddressOffice { get; set; }
        public string Sex { get; set; }
        public bool IsActive { get; set; }
    }
}