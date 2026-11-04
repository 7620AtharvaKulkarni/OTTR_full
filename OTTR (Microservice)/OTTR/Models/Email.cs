using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTTR.Models
{
    public class Email
    {
        public string subject { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string body { get; set; }
        public string cc { get; set; }
        public string bcc { get; set; }
        public string ApplicationName { get; set; }
    }
}
