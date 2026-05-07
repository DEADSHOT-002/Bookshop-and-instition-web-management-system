using BookshopTuitionSystem.Data;
using BookshopTuitionSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookshopTuitionSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        private readonly PasswordHasher<User> _hasher = new();

        public AccountController(AppDbContext db)
        {
            _db = db;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                ViewBag.Error = "Invalid login";
                return View();
            }

            var result = _hasher.VerifyHashedPassword(
                user, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Invalid login";
                return View();
            }

            
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Role", user.Role);

            if (user.Role == "Owner/Admin")
                return RedirectToAction("SelectModule", "Home");

            if (user.Role == "Staff")
                return RedirectToAction("Index", "Bookshop");

            if (user.Role == "Teacher")
                return RedirectToAction("Index", "Institute");

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult AddUser()
        {
            if (HttpContext.Session.GetString("Role") != "Owner/Admin")
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public IActionResult AddUser(
         string username,
         string password,
         string role,

         string StaffFullName,
         int Age,
         string StaffAddress,
         string NIC,
         string StaffContact,

         string TeacherFullName,
         string TeacherAddress,
         string Subject,
         string Qualifications,
         string Email,
         string TeacherContact)
        {
            if (HttpContext.Session.GetString("Role") != "Owner/Admin")
                return RedirectToAction("Index", "Home");

            
            var user = new User
            {
                Username = username,
                Role = role
            };

            user.PasswordHash = _hasher.HashPassword(user, password);

            _db.Users.Add(user);
            _db.SaveChanges();

           
            if (role == "Staff")
            {
                var staff = new Staff
                {
                    UserId = user.UserId,
                    FullName = StaffFullName,
                    Age = Age,
                    Address = StaffAddress,
                    NIC = NIC,
                    ContactNo = StaffContact
                };

                _db.Staffs.Add(staff);
            }

           
            if (role == "Teacher")
            {
                var teacher = new Teacher
                {
                    UserId = user.UserId,
                    FullName = TeacherFullName,
                    Address = TeacherAddress,
                    Subject = Subject,
                    Qualifications = Qualifications,
                    Email = Email,
                    ContactNo = TeacherContact
                };

                _db.Teachers.Add(teacher);
            }

            _db.SaveChanges();

            return RedirectToAction("SelectModule", "Home");
        }

       
    }
}