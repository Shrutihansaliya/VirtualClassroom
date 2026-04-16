////using Microsoft.AspNetCore.Mvc;
////using Microsoft.EntityFrameworkCore;
////using VirtualClassroom.Core;
////using VirtualClassroom.Infrastructure;
////using VirtualClassroom.Web.Models;

////public class MaterialController : Controller
////{
////    private readonly ApplicationDbContext _context;
////    private readonly EmailService _emailService;

////    public MaterialController(ApplicationDbContext context, EmailService emailService)
////    {
////        _context = context;
////        _emailService = emailService;
////    }

////    // GET: Add Page
////    public IActionResult Add()
////    {
////        return View();
////    }

////    // POST: Add Material
////    [HttpPost]
////    public async Task<IActionResult> Add(AddMaterialViewModel model)
////    {
////        if (!ModelState.IsValid)
////            return View(model);

////        // 🔥 STEP 1: Get Logged-in UserId
////        var userIdStr = HttpContext.Session.GetString("UserId");

////        if (string.IsNullOrEmpty(userIdStr))
////        {
////            return RedirectToAction("Login", "Account");
////        }

////        int userId = int.Parse(userIdStr);

////        // 🔥 STEP 2: File Upload
////        string? filePath = null;

////        if (model.File != null)
////        {
////            string fileName = Guid.NewGuid() + Path.GetExtension(model.File.FileName);

////            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/materials");

////            // Ensure folder exists
////            if (!Directory.Exists(folderPath))
////            {
////                Directory.CreateDirectory(folderPath);
////            }

////            string fullPath = Path.Combine(folderPath, fileName);

////            using (var stream = new FileStream(fullPath, FileMode.Create))
////            {
////                await model.File.CopyToAsync(stream);
////            }

////            filePath = "/materials/" + fileName;
////        }

////        // 🔥 STEP 3: Save Material in DB
////        var material = new TblMaterials
////        {
////            ClassroomId = model.ClassroomId,
////            Title = model.Title,
////            Description = model.Description,
////            FilePath = filePath,
////            FileType = model.File?.ContentType,
////            UploadedAt = DateTime.Now,
////            UploadedBy = userId,
////            IsVisible = true
////        };

////        _context.TblMaterials.Add(material);
////        await _context.SaveChangesAsync();

////        // 🔥 STEP 4: Get Student Emails
////        var studentEmails = _context.TblClassroomMembers
////            .Where(x => x.ClassroomId == model.ClassroomId && x.Role == "Student")
////            .Include(x => x.User)
////            .Select(x => x.User.Email)
////            .Where(email => email != null)
////            .ToList();

////        // 🔥 STEP 5: Send Emails (SendGrid / SMTP)
////        var tasks = studentEmails.Select(email =>
////            _emailService.SendEmailAsync(
////                email,
////                "📚 New Material Uploaded",
////                $@"
////New material has been added!

////Title: {model.Title}
////Description: {model.Description}

////Please login to your Virtual Classroom to view it.
////"
////            ));

////        await Task.WhenAll(tasks);

////        // 🔥 STEP 6: Redirect
////        return RedirectToAction("Dashboard", "Faculty");
////    }
////}




//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using VirtualClassroom.Core;
//using VirtualClassroom.Infrastructure;
//using VirtualClassroom.Web.Models;

//public class MaterialController : Controller
//{
//    private readonly ApplicationDbContext _context;
//    private readonly EmailService _emailService;

//    public MaterialController(ApplicationDbContext context, EmailService emailService)
//    {
//        _context = context;
//        _emailService = emailService;
//    }

//    // GET
//    public IActionResult Add(int classroomId)
//    {
//        var model = new AddMaterialViewModel
//        {
//            ClassroomId = classroomId
//        };

//        return View(model);
//    }

//    // POST
//    [HttpPost]
//    public async Task<IActionResult> Add(AddMaterialViewModel model)
//    {
//        if (!ModelState.IsValid)
//            return View(model);

//        // 🔥 CHECK FILE RECEIVED
//        if (model.File == null)
//        {
//            return Content("❌ File NOT received");
//        }

//        // 🔥 SESSION USER
//        var userIdStr = HttpContext.Session.GetString("UserId");

//        if (string.IsNullOrEmpty(userIdStr))
//        {
//            return RedirectToAction("Login", "Account");
//        }

//        int userId = int.Parse(userIdStr);

//        // 🔥 FILE UPLOAD
//        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/materials");

//        if (!Directory.Exists(folderPath))
//        {
//            Directory.CreateDirectory(folderPath);
//        }

//        string fileName = Guid.NewGuid() + Path.GetExtension(model.File.FileName);
//        string fullPath = Path.Combine(folderPath, fileName);

//        using (var stream = new FileStream(fullPath, FileMode.Create))
//        {
//            await model.File.CopyToAsync(stream);
//        }

//        string filePath = "/materials/" + fileName;

//        // 🔥 SAVE DB
//        var material = new TblMaterials
//        {
//            ClassroomId = model.ClassroomId,
//            Title = model.Title,
//            Description = model.Description,
//            FilePath = filePath,
//            FileType = model.File.ContentType,
//            UploadedAt = DateTime.Now,
//            UploadedBy = userId,
//            IsVisible = true
//        };

//        _context.TblMaterials.Add(material);
//        await _context.SaveChangesAsync();

//        // 🔥 SEND EMAIL
//        var studentEmails = _context.TblClassroomMembers
//            .Where(x => x.ClassroomId == model.ClassroomId && x.Role == "Student")
//            .Include(x => x.User)
//            .Select(x => x.User.Email)
//            .Where(e => e != null)
//            .ToList();

//        var tasks = studentEmails.Select(email =>
//            _emailService.SendEmailAsync(
//                email,
//                "📚 New Material Uploaded",
//                $"Material '{model.Title}' has been uploaded."
//            ));

//        await Task.WhenAll(tasks);

//        return RedirectToAction("Dashboard", "Faculty");
//    }
//}


using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VirtualClassroom.Core;
using VirtualClassroom.Infrastructure;
using VirtualClassroom.Web.Models;

public class MaterialController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly EmailService _emailService;

    public MaterialController(ApplicationDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }
    public IActionResult Classrooms()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
            return RedirectToAction("Login", "Account");

        var classrooms = _context.TblClassrooms
            .Where(x => x.CreatedBy == userId.Value)
            .ToList();

        return View(classrooms);
    }

    //public IActionResult Classrooms()
    //{
    //    var classrooms = _context.TblClassrooms.ToList();
    //    return View(classrooms);
    //}
    // GET
    public IActionResult Add(int classroomId)
    {
        var model = new AddMaterialViewModel
        {
            ClassroomId = classroomId
        };

        return View(model);
    }

    // POST
    [HttpPost]
    public async Task<IActionResult> Add(AddMaterialViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // ✅ GET USER FROM SESSION (FIXED)
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // ❌ DEBUG (REMOVE LATER IF WORKS)
        if (model.File == null)
        {
            return Content("❌ File NOT received");
        }

        // 📁 FILE UPLOAD
        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/materials");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string fileName = Guid.NewGuid() + Path.GetExtension(model.File.FileName);
        string fullPath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await model.File.CopyToAsync(stream);
        }

        string filePath = "/materials/" + fileName;

        // 💾 SAVE TO DB
        var material = new TblMaterials
        {
            ClassroomId = model.ClassroomId,
            Title = model.Title,
            Description = model.Description,
            FilePath = filePath,
            FileType = model.File.ContentType,
            UploadedAt = DateTime.Now,
            UploadedBy = userId.Value,
            IsVisible = true
        };

        _context.TblMaterials.Add(material);
        await _context.SaveChangesAsync();

        // 📧 SEND EMAIL
        var studentEmails = _context.TblClassroomMembers
            .Where(x => x.ClassroomId == model.ClassroomId && x.Role == "Student")
            .Include(x => x.User)
            .Select(x => x.User.Email)
            .Where(e => e != null)
            .ToList();

        var tasks = studentEmails.Select(email =>
            _emailService.SendEmailAsync(
                email,
                "📚 New Material Uploaded",
                $"Material '{model.Title}' has been uploaded."
            ));

        await Task.WhenAll(tasks);

        //return RedirectToAction("Dashboard", "Faculty");
        return RedirectToAction("List", new { classroomId = material.ClassroomId });
    }


    public IActionResult List(int classroomId)
    {
        var materials = _context.TblMaterials
            .Where(x => x.ClassroomId == classroomId)
            .ToList();

        ViewBag.ClassroomId = classroomId;

        return View(materials);
    }


    public async Task<IActionResult> Delete(int id)
    {
        var material = await _context.TblMaterials.FindAsync(id);

        if (material != null)
        {
            // delete file from folder
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", material.FilePath.TrimStart('/'));

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.TblMaterials.Remove(material);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("List", new { classroomId = material.ClassroomId });
    }

    public IActionResult Edit(int id)
    {
        var material = _context.TblMaterials.Find(id);

        if (material == null)
            return NotFound();

        return View(material);

    }
    [HttpPost]
    public async Task<IActionResult> Edit(TblMaterials model, IFormFile? file)
    {
        var material = await _context.TblMaterials.FindAsync(model.MaterialId);

        if (material == null)
            return NotFound();

        material.Title = model.Title;
        material.Description = model.Description;

        // file update
        if (file != null)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/materials");

            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            material.FilePath = "/materials/" + fileName;
            material.FileType = file.ContentType;
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("List", new { classroomId = material.ClassroomId });
    }

}