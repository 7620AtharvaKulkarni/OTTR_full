using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTTR.Models
{
    [Table("tblSpendPlan_Region", Schema = "dbo")]
    public class Region:TransactionModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RegionID { get; set; }

        [Column("SPregionName")]
        public string RegionName { get; set; }
        public int PoleID { get; set; }
        
    }
}