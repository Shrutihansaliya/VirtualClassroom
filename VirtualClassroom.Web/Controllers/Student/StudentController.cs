//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using VirtualClassroom.Infrastructure;
//using VirtualClassroom.Web.Filters;

//namespace VirtualClassroom.Web.Controllers.Student
//{
//    [RoleAuthorize("Student")]
//    public class StudentController : BaseController
//    {
//        private readonly ApplicationDbContext _context;

//        public StudentController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<IActionResult> MyClassrooms()
//        {
//            string email = User.Identity.Name; // logged-in user email

//            var classrooms = await _context.TblClassroomInvites
//                .Where(x => x.Email == email && x.IsAccepted == true)
//                .Select(x => x.Classroom)
//                .ToListAsync();

//            return View(classrooms);
//        }






//        public IActionResult Dashboard()
//        {
//            return View();
//        }
//    }
//}






//new one
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore; // ✅ REQUIRED
//using VirtualClassroom.Infrastructure;
//using VirtualClassroom.Web.Filters;

//namespace VirtualClassroom.Web.Controllers.Student
//{
//    [RoleAuthorize("Student")]
//    public class StudentController : BaseController
//    {
//        private readonly ApplicationDbContext _context;

//        public StudentController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // ✅ SHOW ACCEPTED CLASSROOMS
//        public IActionResult MyClassrooms()
//        {
//            var userEmail = User.Identity.Name;

//            var classrooms = _context.TblClassroomInvites
//                .Where(x => x.Email == userEmail && x.IsAccepted == true)
//                .Include(x => x.Classroom) // 🔥 FIX
//                .Select(x => x.Classroom)
//                .ToList();

//            return View(classrooms);
//        }

//        // ✅ ACCEPT INVITE
//        public IActionResult AcceptInvite(int classroomId)
//        {
//            var email = User.Identity.Name;

//            var invite = _context.TblClassroomInvites
//                .FirstOrDefault(x => x.ClassroomId == classroomId && x.Email == email);

//            if (invite != null)
//            {
//                invite.IsAccepted = true;
//                _context.SaveChanges();
//            }

//            return RedirectToAction("MyClassrooms");
//        }

//        // ✅ SHOW PENDING INVITES
//        public IActionResult PendingInvites()
//        {
//            var email = User.Identity.Name;

//            var invites = _context.TblClassroomInvites
//                .Where(x => x.Email == email && x.IsAccepted == false)
//                .Include(x => x.Classroom) // 🔥 FIX
//                .ToList();

//            return View(invites);
//        }

//        public IActionResult Dashboard()
//        {
//            return View();
//        }
//    }
//}


using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VirtualClassroom.Core;
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

        // ✅ SHOW ACCEPTED CLASSROOMS
        public IActionResult MyClassrooms()
        {
            var email = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var classrooms = _context.TblClassroomInvites
                .Where(x => x.Email.ToLower() == email.ToLower() && x.IsAccepted == true)
                .Include(x => x.Classroom)
                .Select(x => x.Classroom)
                .ToList();

            return View(classrooms);
        }

        // ✅ ACCEPT INVITE
        //public IActionResult AcceptInvite(int classroomId)
        //{
        //    var email = HttpContext.Session.GetString("UserEmail");

        //    var invite = _context.TblClassroomInvites
        //        .FirstOrDefault(x => x.ClassroomId == classroomId && x.Email == email);

        //    if (invite != null)
        //    {
        //        invite.IsAccepted = true;
        //        _context.SaveChanges();
        //    }

        //    return RedirectToAction("MyClassrooms");
        //}

        // ✅ SHOW PENDING INVITES
        public IActionResult PendingInvites()
        {
            ViewData["Title"] = "View Invitation";
            var email = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            //var invites = _context.TblClassroomInvites
            //    .Where(x => x.Email.ToLower() == email.ToLower() && x.IsAccepted == false)
            //    .Include(x => x.Classroom)
            //    .ToList();
            var invites = _context.TblClassroomInvites
                .Include(x => x.Classroom)
                .Where(x => x.Email.ToLower() == email.ToLower() && x.IsAccepted == false)
                .ToList();

            return View(invites);
        }

        //public async Task<IActionResult> AcceptInvite(int classroomId)
        //{
        //    var userEmail = User.Identity.Name; // logged-in user's email

        //    // 1. Find invite
        //    var invite = await _context.TblClassroomInvites
        //        .FirstOrDefaultAsync(x => x.ClassroomId == classroomId && x.Email == userEmail);

        //    if (invite == null)
        //    {
        //        return NotFound();
        //    }

        //    // 2. Update invite status
        //    invite.IsAccepted = true;

        //    // 3. Get UserId from TblUsers
        //    var user = await _context.TblUsers
        //        .FirstOrDefaultAsync(u => u.Email == userEmail);

        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    // 4. Check if already member (IMPORTANT 🔥)
        //    var alreadyMember = await _context.TblClassroomMembers
        //        .AnyAsync(m => m.ClassroomId == classroomId && m.UserId == user.UserId);

        //    if (!alreadyMember)
        //    {
        //        // 5. Insert into members table
        //        var member = new TblClassroomMembers
        //        {
        //            ClassroomId = classroomId,
        //            UserId = user.UserId,
        //            Role = "Student", // or dynamic
        //            JoinedAt = DateTime.Now
        //        };

        //        _context.TblClassroomMembers.Add(member);
        //    }

        //    // 6. Save all changes
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("MyClassrooms");
        //}


        public IActionResult Dashboard()
        {
            ViewData["Title"] = "Dashboard";
            var email = HttpContext.Session.GetString("UserEmail");

            var classrooms = _context.TblClassroomInvites
                .Where(x => x.Email.ToLower() == email.ToLower() && x.IsAccepted == true)
                .Include(x => x.Classroom)
                .Select(x => x.Classroom)
                .ToList();

            return View(classrooms);
        }


        public async Task<IActionResult> AcceptInvite(int classroomId)
        {
            var email = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            // 1. Find invite
            var invite = await _context.TblClassroomInvites
                .FirstOrDefaultAsync(x => x.ClassroomId == classroomId && x.Email == email);

            if (invite == null)
            {
                return NotFound();
            }

            // 2. Mark accepted
            invite.IsAccepted = true;

            // 3. Get user
            var user = await _context.TblUsers
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return NotFound();
            }

            // 4. Prevent duplicate entry
            var alreadyMember = await _context.TblClassroomMembers
                .AnyAsync(m => m.ClassroomId == classroomId && m.UserId == user.UserId);

            if (!alreadyMember)
            {
                var member = new TblClassroomMembers
                {
                    ClassroomId = classroomId,
                    UserId = user.UserId,
                    Role = "Student",
                    JoinedAt = DateTime.Now
                };

                _context.TblClassroomMembers.Add(member);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard");
        }
    }
}