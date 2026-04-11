using Microsoft.AspNetCore.Mvc;
using VirtualClassroom.Core;
using VirtualClassroom.Infrastructure;
using VirtualClassroom.Web.Filters;
using VirtualClassroom.Web.Services.Blob;
using Microsoft.EntityFrameworkCore;
namespace VirtualClassroom.Web.Controllers.Faculty
{
    [RoleAuthorize("Faculty")]
    public class AssignmentFacultyController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobService _blob;
        private readonly IConfiguration _config;

        public AssignmentFacultyController(ApplicationDbContext context, BlobService blob, IConfiguration config)
        {
            _context = context;
            _blob = blob;
            _config = config;
        }

        // 📌 CLASSROOM LIST
        public IActionResult Classrooms()
        {
            var facultyId = HttpContext.Session.GetInt32("UserId");

            if (facultyId == null)
                return RedirectToAction("Login", "Account");

            var data = _context.TblClassrooms
                .Where(x => x.CreatedBy == facultyId)
                .ToList();

            return View(data);
        }

        // 📌 ASSIGNMENT LIST
        public IActionResult Index(int classroomId)
        {
            var data = _context.TblAssignments
                .Where(x => x.ClassroomId == classroomId)
                .ToList();

            ViewBag.ClassroomId = classroomId;
            return View(data);
        }

        // 📌 CREATE
        public IActionResult Create(int classroomId)
        {
            ViewBag.ClassroomId = classroomId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TblAssignments model, IFormFile file)
        {
            var facultyId = HttpContext.Session.GetInt32("UserId");

            if (facultyId == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            // ✅ Due date validation
            if (model.DueDate <= DateTime.Now)
            {
                TempData["error"] = "Due date must be in future!";
                return View(model);
            }

            // ✅ Duplicate assignment check (same title in same classroom)
            bool exists = _context.TblAssignments.Any(a =>
                a.ClassroomId == model.ClassroomId &&
                a.Title.ToLower() == model.Title.ToLower()
            );

            if (exists)
            {
                TempData["error"] = "Assignment with same title already exists!";
                return View(model);
            }

            // ✅ File validation
            if (file == null || file.Length == 0)
            {
                TempData["error"] = "File is required!";
                return View(model);
            }

            // ✅ File size (1MB)
            if (file.Length > 1048576)
            {
                TempData["error"] = "File must be less than 1 MB!";
                return View(model);
            }

            // ✅ File type
            var allowed = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg" };
            var ext = Path.GetExtension(file.FileName).ToLower();

            if (!allowed.Contains(ext))
            {
                TempData["error"] = "Only PDF, Word, JPG allowed!";
                return View(model);
            }

            // 🔥 GET DATA
            var faculty = _context.TblUsers.First(x => x.UserId == facultyId.Value);
            var classroom = _context.TblClassrooms.First(x => x.ClassroomId == model.ClassroomId);

            // 🔥 CLEAN FUNCTION
            string Clean(string value)
            {
                return new string(value.Where(char.IsLetterOrDigit).ToArray()).ToLower();
            }

            string facultyName = Clean(faculty.FullName);
            string className = Clean(classroom.ClassName);

            // 🔥 CREATE FOLDER PATH
            string folderPath = $"{facultyName}/{className}";

            // 🔥 UNIQUE FILE NAME
            string uniqueFileName = $"{Guid.NewGuid()}{ext}";

            // 🔥 FINAL PATH
            string fullPath = $"{folderPath}/{uniqueFileName}";

            // 🔥 UPLOAD TO AZURE
            model.FilePath = await _blob.UploadFileAsync(file, fullPath);
            model.FileType = ext;
            model.CreatedBy = facultyId.Value;
            model.CreatedAt = DateTime.Now;

            _context.TblAssignments.Add(model);
            await _context.SaveChangesAsync();

            // 🔥 CREATE DEFAULT SUBMISSIONS
            var students = _context.TblClassroomMembers
                .Where(x => x.ClassroomId == model.ClassroomId)
                .Select(x => x.UserId)
                .ToList();

            foreach (var studentId in students)
            {
                _context.TblSubmissions.Add(new TblSubmissions
                {
                    AssignmentId = model.AssignmentId,
                    StudentId = studentId,
                    Status = "Pending"
                });
            }

            await _context.SaveChangesAsync();

            // 🔥 SEND EMAIL
            var emails = _context.TblUsers
                .Where(u => students.Contains(u.UserId))
                .Select(u => u.Email)
                .ToList();

            foreach (var email in emails)
            {
                await SendAssignmentEmail(email, model.Title, model.DueDate);
            }

            TempData["success"] = "Assignment created successfully!";
            return RedirectToAction("Index", new { classroomId = model.ClassroomId });
        }
        // 📌 DETAILS
        public IActionResult Details(int id)
        {
            var data = _context.TblSubmissions
                .Include(x => x.Student)
                .Where(x => x.AssignmentId == id)
                .ToList();

            return View(data);
        }

        // 📌 DELETE
        public IActionResult Delete(int id)
        {
            var data = _context.TblAssignments.Find(id);

            if (data == null)
                return NotFound();

            _context.TblAssignments.Remove(data);
            _context.SaveChanges();

            TempData["success"] = "Assignment deleted!";
            return RedirectToAction("Index", new { classroomId = data.ClassroomId });
        }

        // 📧 EMAIL METHOD
        private async Task SendAssignmentEmail(string email, string title, DateTime dueDate)
        {
            var apiKey = _config["SendGrid:ApiKey"];

            var client = new SendGrid.SendGridClient(apiKey);

            var from = new SendGrid.Helpers.Mail.EmailAddress("yourmail@gmail.com", "Virtual Classroom");

            var msg = SendGrid.Helpers.Mail.MailHelper.CreateSingleEmail(
                from,
                new SendGrid.Helpers.Mail.EmailAddress(email),
                "New Assignment Added",
                $"Assignment: {title}, Due: {dueDate}",
                $"<strong>New Assignment:</strong> {title}<br/>Due Date: {dueDate}"
            );

            await client.SendEmailAsync(msg);
        }
    }
}
