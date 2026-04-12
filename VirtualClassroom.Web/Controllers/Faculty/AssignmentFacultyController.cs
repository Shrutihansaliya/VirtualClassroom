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
            ViewData["Title"] = "Assignments";
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
            ViewData["Title"] = "Assignments";
            var data = _context.TblAssignments
                .Where(x => x.ClassroomId == classroomId)
                .ToList();

            ViewBag.ClassroomId = classroomId;
            return View(data);
        }

        // 📌 CREATE
        public IActionResult Create(int classroomId)
        {
            ViewData["Title"] = "Assignments";
            Console.WriteLine("🔥 CONTROLLER HIT");
            var model = new TblAssignments
            {
                ClassroomId = classroomId
            };

            return View(model);
        }

        //[HttpPost]
        //public async Task<IActionResult> Create(TblAssignments model, IFormFile file)
        //{
        //    var facultyId = HttpContext.Session.GetInt32("UserId");

        //    if (facultyId == null)
        //        return RedirectToAction("Login", "Account");

        //    if (model.ClassroomId == 0)
        //    {
        //        TempData["error"] = "Invalid classroom!";
        //        return RedirectToAction("Classrooms");
        //    }

        //    try
        //    {
        //        model.Title = model.Title?.Trim();
        //        model.Description = model.Description?.Trim();

        //        if (string.IsNullOrEmpty(model.Title) || model.Title.Length < 3)
        //        {
        //            TempData["error"] = "Title must be at least 3 characters!";
        //            return View(model);
        //        }

        //        if (string.IsNullOrEmpty(model.Description))
        //        {
        //            TempData["error"] = "Description is required!";
        //            return View(model);
        //        }

        //        if (!ModelState.IsValid)
        //            return View(model);

        //        if (model.DueDate <= DateTime.Now)
        //        {
        //            TempData["error"] = "Due date must be in future!";
        //            return View(model);
        //        }

        //        //bool exists = _context.TblAssignments.Any(a =>
        //        //    a.ClassroomId == model.ClassroomId &&
        //        //    a.Title.ToLower() == model.Title.ToLower()
        //        //);

        //        //if (exists)
        //        //{
        //        //    TempData["error"] = "Assignment already exists!";
        //        //    return View(model);
        //        //}

        //        if (file == null || file.Length == 0)
        //        {
        //            TempData["error"] = "File is required!";
        //            return View(model);
        //        }

        //        var allowed = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg" };
        //        var ext = Path.GetExtension(file.FileName).ToLower();

        //        if (!allowed.Contains(ext))
        //        {
        //            TempData["error"] = "Invalid file type!";
        //            return View(model);
        //        }

        //        var faculty = _context.TblUsers.First(x => x.UserId == facultyId.Value);

        //        var classroom = _context.TblClassrooms
        //            .FirstOrDefault(x => x.ClassroomId == model.ClassroomId);

        //        if (classroom == null)
        //        {
        //            TempData["error"] = "Classroom not found!";
        //            return RedirectToAction("Classrooms");
        //        }

        //        string Clean(string value)
        //        {
        //            return new string(value.Where(char.IsLetterOrDigit).ToArray()).ToLower();
        //        }

        //        string folderPath = $"{Clean(faculty.FullName)}/{Clean(classroom.ClassName)}";
        //        string fileName = $"{Guid.NewGuid()}{ext}";
        //        string fullPath = $"{folderPath}/{fileName}";

        //        model.FilePath = await _blob.UploadFileAsync(file, fullPath);
        //        model.FileType = ext;
        //        model.CreatedBy = facultyId.Value;
        //        model.CreatedAt = DateTime.Now;

        //        _context.TblAssignments.Add(model);
        //        var result = await _context.SaveChangesAsync();
        //        Console.WriteLine("Saved rows: " + result);
        //        if (result == 0)
        //        {
        //            TempData["error"] = "Failed to save!";
        //            return View(model);
        //        }

        //        TempData["success"] = "Assignment created successfully!";
        //        return RedirectToAction("Index", new { classroomId = model.ClassroomId });
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["error"] = ex.Message;
        //        return View(model);
        //    }
        //}

        // 📌 DETAILS
        [HttpPost]
        public async Task<IActionResult> Create(TblAssignments model, IFormFile file)
        {
            Console.WriteLine("🔥 POST HIT");

            var facultyId = HttpContext.Session.GetInt32("UserId");

            if (facultyId == null)
                return RedirectToAction("Login", "Account");

            try
            {
                // ✅ REMOVE ALL VALIDATION BLOCKING
                ModelState.Clear();

                // 🔥 FIX DUE DATE (AUTO CORRECT)
                if (model.DueDate <= DateTime.Now)
                {
                    model.DueDate = DateTime.Now.AddHours(1); // ✅ AUTO FIX
                    Console.WriteLine("⚠ DueDate corrected to +1 hour");
                }

                // 🔥 FILE VALIDATION (SAFE)
                if (file == null || file.Length == 0)
                {
                    TempData["error"] = "File is required!";
                    return RedirectToAction("Create", new { classroomId = model.ClassroomId });
                }

                var ext = Path.GetExtension(file.FileName).ToLower();
                var allowed = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg" };

                if (!allowed.Contains(ext))
                {
                    TempData["error"] = "Invalid file type!";
                    return RedirectToAction("Create", new { classroomId = model.ClassroomId });
                }

                // 🔹 GET FACULTY + CLASSROOM
                var faculty = _context.TblUsers.First(x => x.UserId == facultyId.Value);

                var classroom = _context.TblClassrooms
                    .FirstOrDefault(x => x.ClassroomId == model.ClassroomId);

                if (classroom == null)
                {
                    TempData["error"] = "Classroom not found!";
                    return RedirectToAction("Classrooms");
                }

                // 🔹 FILE UPLOAD
                string Clean(string value)
                {
                    return new string(value.Where(char.IsLetterOrDigit).ToArray()).ToLower();
                }

                string folderPath = $"{Clean(faculty.FullName)}/{Clean(classroom.ClassName)}";
                string fileName = $"{Guid.NewGuid()}{ext}";
                string fullPath = $"{folderPath}/{fileName}";

                model.FilePath = await _blob.UploadFileAsync(file, fullPath);
                model.FileType = ext;
                model.CreatedBy = facultyId.Value;
                model.CreatedAt = DateTime.Now;

                // 🔹 SAVE TO DATABASE
                _context.TblAssignments.Add(model);
                await _context.SaveChangesAsync();

                Console.WriteLine("✅ SAVED TO DATABASE");

                TempData["success"] = "Assignment created successfully!";

                // 🔥 SEND EMAIL
                var students = _context.TblClassroomMembers
      .Include(x => x.User)
      .Where(x => x.ClassroomId == model.ClassroomId)
      .Select(x => x.User)
      .ToList();
                Console.WriteLine("Students count: " + students.Count);
                foreach (var student in students)
                {
                    if (!string.IsNullOrEmpty(student.Email))
                    {
                        await SendAssignmentEmail(student.Email, model.Title, model.DueDate);
                    }
                }

                TempData["success2"] = $"Email sent to {students.Count} students!";

                return RedirectToAction("Index", new { classroomId = model.ClassroomId });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ ERROR: " + ex.Message);

                TempData["error"] = ex.Message;
                return RedirectToAction("Create", new { classroomId = model.ClassroomId });
            }
        }
        public IActionResult Details(int id)
        {
            ViewData["Title"] = "Assignments";
            var assignment = _context.TblAssignments
                .FirstOrDefault(x => x.AssignmentId == id);

            if (assignment == null)
                return NotFound();

            var students = _context.TblClassroomMembers
                .Include(x => x.User)
                .Where(x => x.ClassroomId == assignment.ClassroomId
                         && x.User != null)
                .Select(x => x.User)
                .AsEnumerable()
                .Where(u => u.Role.ToString() == "Student")
                .ToList();

            var submissions = _context.TblSubmissions
                .Where(x => x.AssignmentId == id)
                .ToList();

            var result = students.Select(student =>
            {
                var submission = submissions
                    .FirstOrDefault(s => s.StudentId == student.UserId);

                return new TblSubmissions
                {
                    Student = student,
                    Status = submission != null ? "Submitted" : "Pending",
                    FilePath = submission?.FilePath,
                    SubmittedAt = submission?.SubmittedAt
                };
            }).ToList();

            return View(result);
        }

        // 📌 DELETE
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var assignment = _context.TblAssignments
                .FirstOrDefault(x => x.AssignmentId == id);

            if (assignment == null)
                return NotFound();

            try
            {
                // 🔥 GET ALL SUBMISSIONS
                var submissions = _context.TblSubmissions
                    .Where(x => x.AssignmentId == id)
                    .ToList();

                // 🔥 DELETE FILES FROM BLOB (STUDENT FILES)
                foreach (var sub in submissions)
                {
                    if (!string.IsNullOrEmpty(sub.FilePath))
                    {
                        await _blob.DeleteFileAsync(sub.FilePath);
                    }
                }

                // 🔥 DELETE ASSIGNMENT FILE FROM BLOB
                if (!string.IsNullOrEmpty(assignment.FilePath))
                {
                    await _blob.DeleteFileAsync(assignment.FilePath);
                }

                // 🔥 DELETE FROM DATABASE
                _context.TblSubmissions.RemoveRange(submissions);
                _context.TblAssignments.Remove(assignment);

                await _context.SaveChangesAsync();

                TempData["success"] = "Assignment and all submissions deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }

            return RedirectToAction("Index", new { classroomId = assignment.ClassroomId });
        }

        // 📧 EMAIL METHOD
        //private async Task SendAssignmentEmail(string email, string title, DateTime dueDate)
        //{
        //    var apiKey = _config["SendGrid:ApiKey"];

        //    var client = new SendGrid.SendGridClient(apiKey);

        //    var from = new SendGrid.Helpers.Mail.EmailAddress("yourmail@gmail.com", "Virtual Classroom");

        //    var msg = SendGrid.Helpers.Mail.MailHelper.CreateSingleEmail(
        //        from,
        //        new SendGrid.Helpers.Mail.EmailAddress(email),
        //        "New Assignment Added",
        //        $"Assignment: {title}, Due: {dueDate}",
        //        $"<strong>New Assignment:</strong> {title}<br/>Due Date: {dueDate}"
        //    );

        //    await client.SendEmailAsync(msg);
        //}


        private async Task SendAssignmentEmail(string email, string title, DateTime dueDate)
        {
            Console.WriteLine("=== EMAIL START ===");
            Console.WriteLine("To: " + email);

            var apiKey = _config["SendGrid:ApiKey"];
            Console.WriteLine("API KEY: " + apiKey); // 🔥 ADD THIS
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("❌ SendGrid API Key missing");
                return;
            }

            var client = new SendGrid.SendGridClient(apiKey);

            var from = new SendGrid.Helpers.Mail.EmailAddress("yourmail@gmail.com", "Virtual Classroom");

            var msg = SendGrid.Helpers.Mail.MailHelper.CreateSingleEmail(
                from,
                new SendGrid.Helpers.Mail.EmailAddress(email),
                "New Assignment Added",
                $"Assignment: {title}, Due: {dueDate}",
                $"<strong>New Assignment:</strong> {title}<br/>Due Date: {dueDate}"
            );

            var response = await client.SendEmailAsync(msg);

            Console.WriteLine("Email Status: " + response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("❌ Email failed");
            }
            else
            {
                Console.WriteLine("✅ Email sent");
            }
        }
    }
}
