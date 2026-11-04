using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTTR.Models
{

    public class Condition
    {
        public string filterType { get; set; }
        public string type { get; set; }
        public string filter { get; set; }
    }


    public class Filers
    {
        public string type { get; set; }
        public string filter { get; set; }
        public string filterType { get; set; }
        public string @operator { get; set; }
        public Condition condition1 { get; set; }
        public Condition condition2 { get; set; }
    }
    //public class OttrFilterModel
    //{
    //    public int gesGroup { get; set; }
    //    public Filers ottrId { get; set; }
    //    public Filers gespm { get; set; }
    //    public Filers projectId { get; set; }
    //    public Filers ProjectName { get; set; }
    //    public Filers deliverables { get; set; }
    //    public Filers month { get; set; }
    //    public Filers baseDate { get; set; }
    //    public Filers planDate { get; set; }
    //    public Filers actualDate { get; set; }
    //    public Filers gesGroupId { get; set; }
    //    public Filers regionId { get; set; }
    //    public Filers surveyRequest { get; set; }
    //    public Filers spendPlan_Pole { get; set; }
    //    public Filers spendPlan_Region { get; set; }
    //    public Filers spendPlan_Country { get; set; }
    //    public Filers productId { get; set; }
    //    public Filers difference { get; set; }
    //    public Filers attributeToId { get; set; }
    //    public Filers trackingToDesignInput { get; set; }
    //    public Filers internalExternalAttributeId { get; set; }
    //    public Filers comments { get; set; }
    //    public Filers fopmName { get; set; }
    //    public Filers fopmEmailId { get; set; }



    //}
}