using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTTR.Models
{
    [Table("tblSpendPlan_Pole", Schema = "dbo")]
    public class Pole : TransactionModel
    {
        [Key]
        public int PoleID { get; set; }
        public string PoleName { get; set; }
    }
}