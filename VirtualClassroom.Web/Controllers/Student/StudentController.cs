using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VirtualClassroom.Infrastructure;
using VirtualClassroom.Web.Filters;

namespace VirtualClassroom.Web.Controllers.Student
{
    [RoleAuthorize("Student")]
    public class StudentController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> MyClassrooms()
        {
            string email = User.Identity.Name; // logged-in user email

            var classrooms = await _context.TblClassroomInvites
                .Where(x => x.Email == email && x.IsAccepted == true)
                .Select(x => x.Classroom)
                .ToListAsync();

            return View(classrooms);
        }

        public IActionResult Dashboard()
        {
            return View();
        }
    }
}