using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VirtualClassroom.Core;
using VirtualClassroom.Infrastructure;

namespace VirtualClassroom.Web.Controllers
{
    public class MaterialController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MaterialController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Material/ByClass?classId=6
        public IActionResult ByClass(int classId)
        {
            if (classId <= 0)
            {
                return BadRequest("Invalid Class ID");
            }

            var materials = _context.TblMaterials
                .Include(m => m.Classroom)
                .Include(m => m.Faculty)
                .Where(m => m.ClassroomId == classId && m.IsVisible)
                .OrderByDescending(m => m.UploadedAt)
                .ToList();

            return View(materials);
        }
    }
}