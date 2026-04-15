
using Microsoft.AspNetCore.Mvc;
using VirtualClassroom.Infrastructure;
using VirtualClassroom.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using BCrypt.Net;
using System.Text.RegularExpressions;
using VirtualClassroom.Web.Services.Blob;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity.Data;

namespace VirtualClassroom.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobService _blobService;
        public AccountController(ApplicationDbContext context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
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
            // 🔥 FORCE GOOGLE ACCOUNT SELECTION
            properties.Items["prompt"] = "select_account";
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


        // ================= CHANGE PASSWORD =================
        [HttpGet]
        public IActionResult ChangePassword()
        {
            ViewData["Title"] = "Change Password";
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var email = HttpContext.Session.GetString("UserEmail");

            var user = _context.TblUsers.FirstOrDefault(x => x.Email == email);

            if (user == null)
                return RedirectToAction("Login");

            // 🚫 Google user restriction
            if (user.AuthProvider == "Google")
            {
                TempData["Error"] = "Google users cannot change password.";
                return View();
            }

            // 🔐 Verify current password
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                TempData["Error"] = "Current password is incorrect!";
                return View();
            }

            // 🚫 Prevent same password reuse
            if (BCrypt.Net.BCrypt.Verify(newPassword, user.PasswordHash))
            {
                TempData["Error"] = "New password cannot be same as old password!";
                return View();
            }

            // ✅ Match check
            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "Passwords do not match!";
                return View();
            }

            // 🔥 STRONG PASSWORD VALIDATION (YOUR PATTERN)
            var passwordPattern = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$");

            if (!passwordPattern.IsMatch(newPassword))
            {
                TempData["Error"] = "Password must be at least 6 characters and include uppercase, lowercase, number, and special character.";
                return View();
            }

            // ✅ Save
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _context.SaveChanges();

            TempData["Success"] = "Password updated successfully!";
            return View();
        }


        // ================= PROFILE =================
        [HttpGet]
        public IActionResult Profile()
        {
            ViewData["Title"] = "My Profile";
            var email = HttpContext.Session.GetString("UserEmail");

            var user = _context.TblUsers.FirstOrDefault(x => x.Email == email);

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(TblUsers model, IFormFile file)
        {
            var email = HttpContext.Session.GetString("UserEmail");

            var user = _context.TblUsers.FirstOrDefault(x => x.Email == email);

            if (user == null)
                return RedirectToAction("Login");

            // 🔥 VALIDATION
            if (string.IsNullOrWhiteSpace(model.FullName))
            {
                TempData["error"] = "Username is required!";
                return View(user);
            }

            // UPDATE NAME
            user.FullName = model.FullName;

            // IMAGE UPLOAD
            // IMAGE UPLOAD
            if (file != null)
            {
                // ✅ TYPE VALIDATION (JPG, PNG)
                var allowedTypes = new[] { "image/jpeg", "image/png" };

                if (!allowedTypes.Contains(file.ContentType))
                {
                    TempData["error"] = "Only JPG and PNG images are allowed!";
                    return View(user);
                }

                // ✅ SIZE VALIDATION (1MB)
                if (file.Length > 1 * 1024 * 1024)
                {
                    TempData["error"] = "Image must be less than 1 MB!";
                    return View(user);
                }

                // ✅ DELETE OLD IMAGE
                if (!string.IsNullOrEmpty(user.ProfilePicture))
                {
                    await _blobService.DeleteImageAsync(user.ProfilePicture);
                }

                // ✅ UPLOAD NEW IMAGE
                var url = await _blobService.UploadProfileImageAsync(file);
                user.ProfilePicture = url;

                HttpContext.Session.SetString("ProfilePic", url);
            }

            _context.SaveChanges();

            HttpContext.Session.SetString("UserName", user.FullName);

            TempData["success"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }


        //ForgotPasswordRequest password 
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = _context.TblUsers.FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                ViewBag.Message = "Email not found";
                return View();
            }

            var otp = new Random().Next(100000, 999999).ToString();

            HttpContext.Session.SetString("ResetEmail", email);

            HttpContext.Session.SetString("OTP", otp);
            HttpContext.Session.SetString("OTPTime", DateTime.Now.ToString());

            await _emailService.SendEmailAsync(
        email,
        "Virtual Classroom - OTP Verification",
        $@"
    <div style='font-family:Arial;padding:20px'>
        <h2 style='color:#2a5298'>Virtual Classroom</h2>
        <p>Your OTP is:</p>
        <h1 style='background:#f2f2f2;padding:10px'>{otp}</h1>
        <p>This OTP is valid for 2 minutes.</p>
    </div>"
    );
            return RedirectToAction("VerifyOtp");
        }


        [HttpGet]
        public IActionResult VerifyOtp()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult VerifyOtp(string otp)
        //{
        //    var sessionOtp = HttpContext.Session.GetString("OTP");

        //    if (otp == sessionOtp)
        //    {
        //        return RedirectToAction("ResetPassword");
        //    }

        //    ViewBag.Error = "Invalid OTP";
        //    return View();
        //}


        [HttpPost]
        public IActionResult VerifyOtp(string otp)
        {
            var sessionOtp = HttpContext.Session.GetString("OTP");
            var otpTimeStr = HttpContext.Session.GetString("OTPTime");

            // 🔥 Check expiry
            if (otpTimeStr != null)
            {
                var otpTime = DateTime.Parse(otpTimeStr);

                if ((DateTime.Now - otpTime).TotalMinutes > 5) // 👉 change here
                {
                    ViewBag.Error = "OTP expired";
                    return View();
                }
            }

            // 🔥 Check match
            if (otp == sessionOtp)
            {
                return RedirectToAction("ResetPassword");
            }

            ViewBag.Error = "Invalid OTP";
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }



        [HttpPost]
        public IActionResult ResetPassword(string newPassword, string confirmPassword)
        {
            // ✅ 1. Match check
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match";
                return View();
            }

            // ✅ 2. Password pattern (same as Register)
            var passwordPattern = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$");

            if (!passwordPattern.IsMatch(newPassword))
            {
                ViewBag.Error = "Password must be at least 6 characters and include uppercase, lowercase, number, and special character.";
                return View();
            }

            // ✅ 3. Session check (important)
            var email = HttpContext.Session.GetString("ResetEmail");

            if (email == null)
            {
                return RedirectToAction("Login");
            }

            // ✅ 4. User check (important)
            var user = _context.TblUsers.FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                ViewBag.Error = "User not found";
                return View();
            }

            // 🔐 5. Hash password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            _context.SaveChanges();

            // ✅ 6. Clear session
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        //public IActionResult Logout()
        //{
        //    // Clear your app session
        //    HttpContext.Session.Clear();

        //    // 🔥 Redirect to Google logout
        //    return Redirect("https://accounts.google.com/Logout?continue=https://appengine.google.com/_ah/logout?continue=https://localhost:5001/Account/Login");
        //}

    }




}