using Microsoft.AspNetCore.Mvc;

namespace VirtualClassroom.Web.Controllers
{
    public class BaseController : Controller
    {
        protected int? UserId => HttpContext.Session.GetInt32("UserId");
        protected string UserName => HttpContext.Session.GetString("UserName");
        protected string UserEmail => HttpContext.Session.GetString("UserEmail");
        protected string UserRole => HttpContext.Session.GetString("UserRole");
    }
}