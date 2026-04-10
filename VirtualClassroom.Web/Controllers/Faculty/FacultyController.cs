using Microsoft.AspNetCore.Mvc;
using VirtualClassroom.Web.Filters;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using VirtualClassroom.Core;
using VirtualClassroom.Infrastructure;
namespace VirtualClassroom.Web.Controllers.Faculty
{
    [RoleAuthorize("Faculty")]
    public class FacultyController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public FacultyController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Dashboard()
        {
            return View();
        }

       
    }
}
