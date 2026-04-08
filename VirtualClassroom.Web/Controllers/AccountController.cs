//namespace VirtualClassroom.Web.Controllers
//{
//    public class AccountController
//    {
//    }
//}

using Microsoft.AspNetCore.Mvc;
using VirtualClassroom.Infrastructure;
using VirtualClassroom.Core;
using System.Linq;

namespace VirtualClassroom.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LOGIN PAGE
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // LOGIN POST
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.TblUsers
                .FirstOrDefault(u => u.Email == email && u.PasswordHash == password);

            if (user != null)
            {
                if (user.Role == UserRole.Student)
                    return RedirectToAction("Dashboard", "Student");

                else
                    return RedirectToAction("Dashboard", "Faculty");
            }

            ViewBag.Error = "Invalid Login";
            return View();
        }

        // REGISTER PAGE
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // REGISTER POST
        [HttpPost]
        public IActionResult Register(TblUsers user)
        {
            if (ModelState.IsValid)
            {
                user.CreatedAt = DateTime.Now;

                _context.TblUsers.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(user);
        }
    }
}