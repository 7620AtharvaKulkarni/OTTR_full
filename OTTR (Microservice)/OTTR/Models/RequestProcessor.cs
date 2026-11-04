using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTTR.Models
{
    public class RequestProcessor
    {
        public string app { get; set; }
        public string action { get; set; }
        public string data { get; set; }
        public string requestType { get; set; }
        public string secretKey { get; set; }
    }
}