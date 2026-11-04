using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTTR.Models
{
    [Table("tblSpendPlan_RegionCountryMapping", Schema = "dbo")]
    public class RegionCountryMapping : TransactionModel
    {
        [Key]
        public int RegionID { get; set; }
        [Key]
        public int CountryID { get; set; }
    }
}