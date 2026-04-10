
using Microsoft.AspNetCore.Mvc;
using VirtualClassroom.Infrastructure;
using VirtualClassroom.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using BCrypt.Net;
using System.Text.RegularExpressions;

namespace VirtualClassroom.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= LOGIN =================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.TblUsers.FirstOrDefault(u => u.Email == email);

            if (user != null)
            {
                // 🚫 Google users restriction
                if (user.AuthProvider == "Google")
                {
                    ViewBag.Error = "Please login using Google";
                    return View();
                }

                // 🔐 Verify password
                if (!string.IsNullOrEmpty(user.PasswordHash) &&
                    BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    // ✅ SESSION
                     HttpContext.Session.SetInt32("UserId", user.UserId);
                    HttpContext.Session.SetString("UserName", user.FullName);
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetString("UserRole", user.Role.ToString());

                    return RedirectToRoleDashboard(user.Role);
                }
            }

            ViewBag.Error = "Invalid email or password";
            return View();
        }



        // ================= GOOGLE LOGIN =================
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return RedirectToAction("Login");

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;
            var googleId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = _context.TblUsers.FirstOrDefault(x => x.Email == email);

            if (user != null)
            {
                // ✅ ADD SESSION
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserName", user.FullName);
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Role.ToString());

                return RedirectToRoleDashboard(user.Role);
            }

            TempData["Email"] = email;
            TempData["Name"] = name;
            TempData["GoogleId"] = googleId;

            return RedirectToAction("SelectRole");
        }

        // ================= ROLE SELECTION =================
        [HttpGet]
        public IActionResult SelectRole()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SelectRole(int role)
        {
            var email = TempData["Email"]?.ToString();
            var name = TempData["Name"]?.ToString();
            var googleId = TempData["GoogleId"]?.ToString();

            if (email == null)
                return RedirectToAction("Login");

            // Prevent duplicate user
            if (_context.TblUsers.Any(x => x.Email == email))
            {
                var existingUser = _context.TblUsers.First(x => x.Email == email);
                return RedirectToRoleDashboard(existingUser.Role);
            }

            var user = new TblUsers
            {
                Email = email,
                FullName = name ?? "User",
                AuthProvider = "Google",
                ProviderUserId = googleId,
                Role = (UserRole)role,
                CreatedAt = DateTime.Now,
                PasswordHash = null
            };

            _context.TblUsers.Add(user);
            _context.SaveChanges();
            // ✅ ADD SESSION
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role.ToString());
            return RedirectToRoleDashboard(user.Role);
        }

        private IActionResult RedirectToRoleDashboard(UserRole role)
        {
            if (role == UserRole.Student)
                return RedirectToAction("Dashboard", "Student");

            return RedirectToAction("Dashboard", "Faculty");
        }

        // ================= REGISTER =================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }



        [HttpPost]
        public IActionResult Register(TblUsers user)
        {
            if (!ModelState.IsValid)
                return View(user);

            var passwordPattern = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$");

            if (!passwordPattern.IsMatch(user.PasswordHash))
            {
                ViewBag.Error = "Password must be at least 6 characters and include uppercase, lowercase, number, and special character.";
                return View(user);
            }

            // 🚫 Duplicate email check
            if (_context.TblUsers.Any(x => x.Email == user.Email))
            {
                ViewBag.Error = "Email already exists";
                return View(user);
            }

            if (_context.TblUsers.Any(x => x.FullName == user.FullName))
            {
                ViewBag.Error = " Username already exists";
                return View(user);
            }


            user.CreatedAt = DateTime.Now;
            user.AuthProvider = "Local";

            // 🔐 Hash password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            _context.TblUsers.Add(user);
            _context.SaveChanges();

            // ✅ AUTO LOGIN (SESSION SET)
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role.ToString());

            // ✅ REDIRECT BASED ON ROLE
            return RedirectToRoleDashboard(user.Role);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }




}