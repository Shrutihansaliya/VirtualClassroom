using Microsoft.AspNetCore.Mvc;
using VirtualClassroom.Web.Filters;

namespace VirtualClassroom.Web.Controllers.Student
{
    [RoleAuthorize("Student")]
    public class StudentController : BaseController
    {
        
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}