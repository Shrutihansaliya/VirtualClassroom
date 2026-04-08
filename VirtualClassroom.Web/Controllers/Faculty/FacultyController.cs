using Microsoft.AspNetCore.Mvc;

namespace VirtualClassroom.Web.Controllers.Faculty
{
    public class FacultyController : Controller
    {
       
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
