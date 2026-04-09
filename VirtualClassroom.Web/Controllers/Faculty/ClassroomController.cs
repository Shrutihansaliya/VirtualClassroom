using Microsoft.AspNetCore.Mvc;
using VirtualClassroom.Core;
using VirtualClassroom.Infrastructure;

namespace VirtualClassroom.Web.Controllers.Faculty;
public class ClassroomController : Controller
{
    private readonly ApplicationDbContext _context;

    public ClassroomController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(string className, string description)
    {
        int facultyId = 1; // 🔥 TEMP (later from login)

        var classroom = new TblClassroom
        {
            ClassName = className,
            Description = description,
            CreatedBy = facultyId,
            CreatedAt = DateTime.Now
        };

        _context.TblClassrooms.Add(classroom);
        _context.SaveChanges();

        return RedirectToAction("AddStudents", new { id = classroom.ClassroomId });
    }
}
