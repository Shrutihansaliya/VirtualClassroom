using Microsoft.AspNetCore.Mvc;

namespace VirtualClassroom.Web.Controllers.Student
{
    public class StudentController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}