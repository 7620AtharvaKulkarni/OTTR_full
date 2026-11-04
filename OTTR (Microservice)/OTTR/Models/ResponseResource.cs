namespace OTTR.Models
{
    public class ResponseResource
    {
        public string message { get;set; }
        public int code { get ;set;} //make this statusCode
        public object data { get ; set ;}
    }
}