using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTTR.Models
{
    [Table("tblOTTR_InternalExternalAttributeTo", Schema = "dbo")]
    public class InternalExternalAttributeTo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "int")]
        public int InternalExternalAttributeToId { get; set; }

        [Column("InternalExternalAttributeTo",  TypeName = "varchar(100)")]
        public string InternalExternalAttributeToName { get; set; }

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