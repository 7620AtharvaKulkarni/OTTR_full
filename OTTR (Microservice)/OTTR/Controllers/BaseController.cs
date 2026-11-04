using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace OTTR.Controllers
{
    public class BaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        protected JsonResult OK(object data = null)
        {
            return Json(new { Status = "ok", Data = data });
        }

        public JsonResult NoContent(String message)
        {
            return Json(new { Message = message });
        }

        protected JsonResult Error(string message, int errorCode = 0)
        {
            var json = Json(new { Status = "error", Message = message, ErrorCode = errorCode });
            json.StatusCode = 400;
            return json;
        }

        


        protected JsonResult NotAuthorized()
        {
            //Response.StatusCode = 401;
            return Error("You are not authorized to perform this action", 401);
        }

        public JsonResult NotFound()
        {
            //Response.StatusCode = 404;
            return Error("Resource not found", 404);
        }
    }
}