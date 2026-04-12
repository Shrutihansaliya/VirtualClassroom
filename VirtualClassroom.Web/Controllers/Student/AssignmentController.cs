//using Microsoft.AspNetCore.Mvc;
//using VirtualClassroom.Infrastructure;

namespace VirtualClassroom.Web.Controllers.Student;
//{
//    public class AssignmentController
//    {
//    }
//}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VirtualClassroom.Infrastructure;
using VirtualClassroom.Web.Controllers;
using VirtualClassroom.Web.Filters;


public class AssignmentController : BaseController
{
    private readonly ApplicationDbContext _context;

    public AssignmentController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        // 🔥 Get student's enrolled classes
        var assignments = _context.TblAssignments
            .Include(a => a.Classroom)
            .Where(a => a.Classroom.Members.Any(m => m.UserId == userId))
            .ToList();

        //return View(assignments);
        return View("~/Views/Student/Assignments.cshtml", assignments);
    }

    public IActionResult ByClass(int classId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        var assignments = _context.TblAssignments
            .Include(a => a.Classroom)
            .Where(a => a.ClassroomId == classId &&
                        a.Classroom.Members.Any(m => m.UserId == userId))
            .ToList();

        return View("~/Views/Student/Assignments.cshtml", assignments);
    }
}