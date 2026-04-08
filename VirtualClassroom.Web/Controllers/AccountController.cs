using Microsoft.AspNetCore.Mvc;
using VirtualClassroom.Infrastructure;
using VirtualClassroom.Core;
using BCrypt.Net;

namespace VirtualClassroom.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // 🔹 POST LOGIN (PUT YOUR CODE HERE)
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.TblUsers
                .FirstOrDefault(u => u.Email == email);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                if (user.Role == UserRole.Student)
                    return RedirectToAction("Dashboard", "Student");
                else
                    return RedirectToAction("Dashboard", "Faculty");
            }

            ViewBag.Error = "Invalid Email or Password";
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
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            // 🔐 HASH PASSWORD
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            user.CreatedAt = DateTime.Now;

            _context.TblUsers.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }
    }
}