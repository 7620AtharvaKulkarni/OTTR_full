using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTTR.Models
{
    [Table("tblOTTR_GESGroup", Schema = "dbo")]
    public class GESGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("GES_Group_Id", TypeName = "int")]
        public int GESGroupId { get; set; }

        [Column("GES_Group", TypeName = "varchar(50)")]
        public string GESGroupName { get; set; }

        [Column(TypeName = "tinyint")]
        public bool IsActive { get; set; }

        [Column("GES_Sub_Group", TypeName = "varchar(50)")]
        public string GESSubGroupName { get; set; }

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