using Microsoft.AspNetCore.Mvc;
using VirtualClassroom.Web.Filters;

namespace VirtualClassroom.Web.Controllers.Faculty
{
    [RoleAuthorize("Faculty")]
    public class FacultyController : Controller
    {
       
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
