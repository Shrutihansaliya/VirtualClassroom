using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Mvc;
using VirtualClassroom.Core;
using VirtualClassroom.Infrastructure;
using System.IO;


namespace VirtualClassroom.Web.Controllers.Faculty;

public class ClassroomController : Controller
{
    private readonly ApplicationDbContext _context;
    //private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;

    public ClassroomController(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "Create Classroom";
        return View();
    }

    //[HttpPost]
    //public IActionResult Create(string className, string description)
    //{
    //    // 🔥 GET USER ID FROM SESSION
    //    var facultyId = HttpContext.Session.GetInt32("UserId");

    //    if (facultyId == null)
    //    {
    //        return RedirectToAction("Login", "Account");
    //    }

    //    var classroom = new TblClassroom
    //    {
    //        ClassName = className,
    //        Description = description,
    //        CreatedBy = facultyId.Value, // ✅ FIXED
    //        CreatedAt = DateTime.Now
    //    };

    //    _context.TblClassrooms.Add(classroom);
    //    _context.SaveChanges();

    //    //return RedirectToAction("Index");
    //    return RedirectToAction("Dashboard", "Faculty");
    //}
    [HttpPost]
    public IActionResult Create(string className, string description)
    {
        var facultyId = HttpContext.Session.GetInt32("UserId");

        if (facultyId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // 🔥 CHECK DUPLICATE FOR SAME FACULTY
        bool exists = _context.TblClassrooms
            .Any(c => c.CreatedBy == facultyId.Value && c.ClassName == className);

        if (exists)
        {
            //TempData["Error"] = "You already created this classroom.";
            //return RedirectToAction("Create");
            ModelState.AddModelError("", "You already created this classroom.");
            return View();
        }

        var classroom = new TblClassroom
        {
            ClassName = className,
            Description = description,
            CreatedBy = facultyId.Value,
            CreatedAt = DateTime.Now
        };

        _context.TblClassrooms.Add(classroom);
        _context.SaveChanges();

        TempData["Success"] = "Classroom created successfully!";
        return RedirectToAction("Dashboard", "Faculty");
    }
    [HttpGet]
    public IActionResult InviteMembers(int classroomId)
    {

        ViewBag.ClassroomId = classroomId;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> InviteMembers(int classroomId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is empty");

        var emails = new List<string>();

        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (!string.IsNullOrWhiteSpace(line))
                    emails.Add(line.Trim());
            }
        }

        //foreach (var email in emails)
        //{
        //    // 1. Save to DB
        //    var invite = new TblClassroomInvites
        //    {
        //        ClassroomId = classroomId,
        //        Email = email,
        //        IsAccepted = false,
        //        SentAt = DateTime.Now
        //    };

        //    _context.TblClassroomInvites.Add(invite);

        //    // 2. Send Email
        //    await SendInviteEmail(email, classroomId);
        //}
        var invites = new List<TblClassroomInvites>();

        foreach (var email in emails)
        {
            invites.Add(new TblClassroomInvites
            {
                ClassroomId = classroomId,
                Email = email,
                IsAccepted = false,
                SentAt = DateTime.Now
            });
        }

        _context.TblClassroomInvites.AddRange(invites);
        //await _context.SaveChangesAsync();

        await _context.SaveChangesAsync();

        foreach (var email in emails)
        {
            await SendInviteEmail(email, classroomId);
        }

        TempData["Success"] = "Invitations sent successfully!";
        //return RedirectToAction("Index");
        return RedirectToAction("Dashboard", "Faculty");
    }
    //private async Task SendInviteEmail(string email, int classroomId)
    //{
    //    var apiKey = _config["SendGrid:ApiKey"];
    //    var client = new SendGridClient(apiKey);

    //    var from = new EmailAddress("jaillymaniya07@gmail.com", "Virtual Classroom");
    //    var subject = "Classroom Invitation";

    //    var classroomLink = $"https://yourdomain.com/Student/Classroom/Join/{classroomId}";

    //    var to = new EmailAddress(email);

    //    var plainTextContent = $"You are invited to join a classroom. Click: {classroomLink}";
    //    var htmlContent = $"<strong>You are invited!</strong><br/><a href='{classroomLink}'>Join Classroom</a>";

    //    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

    //    await client.SendEmailAsync(msg);
    //}
    private async Task SendInviteEmail(string email, int classroomId)
    {
        var apiKey = _config["SendGrid:ApiKey"];

        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("❌ SendGrid API Key is missing!");
            return;
        }

        var client = new SendGridClient(apiKey);

        var from = new EmailAddress("jaillymaniya07@gmail.com", "ScheduleX");

        //var classroomLink = $"https://localhost:7038/Student/Classroom/Join/{classroomId}";
        var request = HttpContext.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";
        var classroomLink = $"{baseUrl}/Student/Classroom/Join/{classroomId}";

        var msg = MailHelper.CreateSingleEmail(
            from,
            new EmailAddress(email),
            "Classroom Invitation",
            $"Join: {classroomLink}",
            $"<strong>Join here:</strong> <a href='{classroomLink}'>Click</a>"
        );

        var response = await client.SendEmailAsync(msg);

        Console.WriteLine($"📩 SendGrid Status: {response.StatusCode}");

        var body = await response.Body.ReadAsStringAsync();
        Console.WriteLine($"📩 SendGrid Response Body: {body}");
    }

    [HttpGet]
    public IActionResult Members(int classroomId)
    {
        var members = _context.TblClassroomInvites
            .Where(x => x.ClassroomId == classroomId)
            .ToList();

        ViewBag.ClassroomId = classroomId;

        return View(members);
    }

}
