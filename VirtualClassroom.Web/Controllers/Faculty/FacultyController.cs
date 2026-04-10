using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VirtualClassroom.Core;
using VirtualClassroom.Infrastructure;
using VirtualClassroom.Web.Filters;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
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
            //return View();
            var facultyId = HttpContext.Session.GetInt32("UserId");

            if (facultyId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var classrooms = _context.TblClassrooms
                .Where(c => c.CreatedBy == facultyId.Value)
                .ToList();

            return View(classrooms);
        }
        public IActionResult CreateClassroom()
        {
            return View();
        }

        // Save Classroom + Emails
        [HttpPost]
        public async Task<IActionResult> CreateClassroom(TblClassroom model, string studentEmails)
        {
            int facultyId = 1; // replace with session later

            model.CreatedBy = facultyId;
            model.CreatedAt = DateTime.Now;

            _context.TblClassrooms.Add(model);
            await _context.SaveChangesAsync();

            // Split emails
            var emails = studentEmails.Split(',');

            foreach (var email in emails)
            {
                _context.TblClassroomInvites.Add(new TblClassroomInvites
                {
                    ClassroomId = model.ClassroomId,
                    Email = email.Trim()
                });

                // EMAIL SENDING (SKIP FOR NOW)
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("CreateClassroom");
        }
    }
}
