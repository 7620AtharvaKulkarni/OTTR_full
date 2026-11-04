using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace OTTR.Models
{
    public class GESGroupmodel 
    {
        [Key]
        public string GroupName { get; set; }
        [Key]
        public List<GESSubGroupmodel> GesSubGroup { get; set; }
    }

    public class GESSubGroupmodel
    {
        public string SubgroupName { get; set; }
        public int SubgroupId { get; set; }
    }
}