using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTTR.Models
{
    [Table("tblOTTR_Region", Schema = "dbo")]
    public class Gesgroupdelivery
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "int")]
        public int RegionId { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string RegionName { get; set; }
        
        [Column(TypeName = "tinyint")]
        public bool IsActive { get; set; }

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