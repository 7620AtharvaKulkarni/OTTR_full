using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTTR.Models
{
    [Table("tblOTTR_GESGroupProductMapping", Schema = "dbo")]
    public class GESGroupProductMapping
    {
        [Key]
        [Column("GES_Group_Id", TypeName = "int")]
        public int GESGroupId { get; set; }

        [Key]
        [Column(TypeName = "int")]
        public int ProductId { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string CreatedBy {get; set;}

        [Column(TypeName = "datetime)")]
        public DateTime CreatedOn {get; set;}

        [Column(TypeName = "varchar(50)")]
        public string ModifiedBy {get; set;}

        [Column(TypeName = "datetime")]
        public DateTime ModifiedOn {get; set;}
    }
}