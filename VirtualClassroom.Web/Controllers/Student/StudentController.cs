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
        public IActionResult AcceptInvite(int classroomId)
        {
            var email = HttpContext.Session.GetString("UserEmail");

            var invite = _context.TblClassroomInvites
                .FirstOrDefault(x => x.ClassroomId == classroomId && x.Email == email);

            if (invite != null)
            {
                invite.IsAccepted = true;
                _context.SaveChanges();
            }

            return RedirectToAction("MyClassrooms");
        }

        // ✅ SHOW PENDING INVITES
        public IActionResult PendingInvites()
        {
            var email = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var invites = _context.TblClassroomInvites
                .Where(x => x.Email.ToLower() == email.ToLower() && x.IsAccepted == false)
                .Include(x => x.Classroom)
                .ToList();

            return View(invites);
        }

      
        public IActionResult Dashboard()
        {
            var email = HttpContext.Session.GetString("UserEmail");

            var classrooms = _context.TblClassroomInvites
                .Where(x => x.Email.ToLower() == email.ToLower() && x.IsAccepted == true)
                .Include(x => x.Classroom)
                .Select(x => x.Classroom)
                .ToList();

            return View(classrooms);
        }
    }
}