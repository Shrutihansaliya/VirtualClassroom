using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VirtualClassroom.Core;
using VirtualClassroom.Infrastructure;
using VirtualClassroom.Web.Filters;
using VirtualClassroom.Web.Services.Blob;
namespace VirtualClassroom.Web.Controllers.Student;

public class SubmissionController : BaseController
{
    private readonly ApplicationDbContext _context;

    private readonly BlobSubmissionService _blobService;

    public SubmissionController(ApplicationDbContext context, BlobSubmissionService blobService)
    {
        _context = context;
        _blobService = blobService;
    }

    //public SubmissionController(ApplicationDbContext context)
    //{
    //    _context = context;
    //}

    // OPEN SUBMISSION PAGE
    [HttpGet]
    public IActionResult Submit(int assignmentId)
    {
        ViewBag.AssignmentId = assignmentId;
        //return View();
        return View("~/Views/Student/Submit.cshtml");
    }

    private bool IsBeforeDeadline(TblAssignments assignment)
    {
        return DateTime.Now <= assignment.DueDate;
    }


    [HttpPost]
    public async Task<IActionResult> Submit(int assignmentId, IFormFile file)
    {
        var studentId = HttpContext.Session.GetInt32("UserId");

        var assignment = await _context.TblAssignments
            .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

        if (assignment == null)
            return NotFound();

        if (DateTime.Now > assignment.DueDate)
        {
            TempData["Error"] = "⛔ Deadline passed!";
            return RedirectToAction("ByClass", "Assignment", new { classId = assignment.ClassroomId });
        }
        //return Content("Deadline passed. Submission closed ❌");

        var existing = await _context.TblSubmissions
            .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId);

        if (existing != null)
        {
            TempData["Error"] = "⚠ Already submitted!";
            return RedirectToAction("ByClass", "Assignment", new { classId = assignment.ClassroomId });
        }
        //return Content("Already submitted ❌");

        string fileUrl = null;

        if (file != null)
        {
            string folder = "assignment-" + assignmentId + "/student-" + studentId;
            fileUrl = await _blobService.UploadFileAsync(file, folder);
        }

        var submission = new TblSubmissions
        {
            AssignmentId = assignmentId,
            StudentId = studentId.Value,
            FilePath = fileUrl,
            SubmittedAt = DateTime.Now,
            Status = "Submitted"
        };

        _context.TblSubmissions.Add(submission);
        await _context.SaveChangesAsync();

        return RedirectToAction("ByClass", "Assignment", new { classId = assignment.ClassroomId });
    }


    //[HttpPost]
    //public async Task<IActionResult> EditSubmission(int submissionId, IFormFile file)
    //{
    //    var studentId = HttpContext.Session.GetInt32("UserId");

    //    var submission = await _context.TblSubmissions
    //        .Include(s => s.Assignment)
    //        .FirstOrDefaultAsync(s => s.SubmissionId == submissionId && s.StudentId == studentId);

    //    if (submission == null)
    //        return NotFound();

    //    // ⛔ DEADLINE CHECK
    //    if (DateTime.Now > submission.Assignment.DueDate)
    //        return Content("Deadline passed. Cannot edit ❌");

    //    // UPDATE FILE
    //    if (file != null)
    //    {
    //        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
    //        var path = Path.Combine("wwwroot/uploads", fileName);

    //        using (var stream = new FileStream(path, FileMode.Create))
    //        {
    //            await file.CopyToAsync(stream);
    //        }

    //        submission.FilePath = "/uploads/" + fileName;
    //        submission.SubmittedAt = DateTime.Now;
    //    }

    //    await _context.SaveChangesAsync();

    //    return RedirectToAction("MyClassrooms", "Student");
    //}
    [HttpPost]
    public async Task<IActionResult> EditSubmission(int submissionId, IFormFile file)
    {
        var studentId = HttpContext.Session.GetInt32("UserId");

        var submission = await _context.TblSubmissions
            .Include(s => s.Assignment)
            .FirstOrDefaultAsync(s => s.SubmissionId == submissionId && s.StudentId == studentId);

        if (submission == null)
            return NotFound();

        if (DateTime.Now > submission.Assignment.DueDate)
            return Content("Deadline passed ❌");

        if (file != null)
        {
            string folder = "assignment-" + submission.AssignmentId + "/student-" + studentId;

            var fileUrl = await _blobService.UploadFileAsync(file, folder);

            submission.FilePath = fileUrl;
            submission.SubmittedAt = DateTime.Now;
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("MyClassrooms", "Student");
    }

    public async Task<IActionResult> DeleteSubmission(int submissionId)
    {
        var studentId = HttpContext.Session.GetInt32("UserId");

        var submission = await _context.TblSubmissions
            .Include(s => s.Assignment)
            .FirstOrDefaultAsync(s => s.SubmissionId == submissionId && s.StudentId == studentId);

        if (submission == null)
            return NotFound();

        if (DateTime.Now > submission.Assignment.DueDate)
        {
            TempData["Error"] = "⛔ Cannot delete after deadline!";
            return RedirectToAction("ByClass", "Assignment", new { classId = submission.Assignment.ClassroomId });
        }
        //return Content("Deadline passed ❌");

        _context.TblSubmissions.Remove(submission);
        await _context.SaveChangesAsync();

        //return RedirectToAction("MyClassrooms", "Student");

        TempData["Success"] = "🗑 Submission deleted!";
        return RedirectToAction("ByClass", "Assignment", new { classId = submission.Assignment.ClassroomId });
    }
    [HttpGet("submission/view/{submissionId}")]
    public async Task<IActionResult> ViewSubmissionFile(int submissionId)
    {
        var submission = await _context.TblSubmissions
            .FirstOrDefaultAsync(x => x.SubmissionId == submissionId);

        if (submission == null || string.IsNullOrEmpty(submission.FilePath))
            return NotFound();

        using var httpClient = new HttpClient();
        var fileBytes = await httpClient.GetByteArrayAsync(submission.FilePath);

        var ext = Path.GetExtension(submission.FilePath).ToLower();

        var contentType = ext switch
        {
            ".pdf" => "application/pdf",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".txt" => "text/plain",
            _ => "application/octet-stream"
        };

        Response.Headers["Content-Disposition"] = "inline";

        return File(fileBytes, contentType);
    }
}