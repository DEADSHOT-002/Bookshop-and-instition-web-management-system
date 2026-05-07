using BookshopTuitionSystem.Data;
using BookshopTuitionSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BookshopTuitionSystem.Controllers
{
    public class InstituteController : Controller
    {
        private readonly AppDbContext _db;
        private bool isPresent;

        public InstituteController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");

            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            if (role == "Teacher")
            {
                return View("TeacherDashboard");
            }

            ViewBag.TotalStudents = _db.Students.Count();
            ViewBag.ActiveStudents = _db.Students.Count(s => s.Status == "Active");

            return View();
        }
        private int? GetTeacherId()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return null;

            var teacher = _db.Teachers
                .FirstOrDefault(t => t.UserId == userId.Value);

            return teacher?.TeacherId;
        }
        public IActionResult MarkAvailability()
        {
            if (HttpContext.Session.GetString("Role") != "Teacher")
                return RedirectToAction("Index");

            var teacherId = GetTeacherId();

            if (teacherId == null)
                return RedirectToAction("Index");

            var dates = _db.TeacherAvailabilities
                .Where(a => a.TeacherId == teacherId)
                .Select(a => a.AvailableDate)
                .ToList();

            return View(dates);
        }
        [HttpPost]
        public IActionResult MarkAvailability(DateTime selectedDate)
        {
            var teacherId = GetTeacherId();

            if (teacherId == null)
                return RedirectToAction("Index");

            var exists = _db.TeacherAvailabilities.Any(a =>
                a.TeacherId == teacherId &&
                a.AvailableDate.Date == selectedDate.Date);

            if (!exists)
            {
                _db.TeacherAvailabilities.Add(new TeacherAvailability
                {
                    TeacherId = teacherId.Value,
                    AvailableDate = selectedDate.Date
                });

                _db.SaveChanges();
            }

            return RedirectToAction("MarkAvailability");
        }

        public IActionResult MarkAttendance()
        {
            if (HttpContext.Session.GetString("Role") != "Teacher")
                return RedirectToAction("Index");

            return View();
        }

        [HttpPost]
        public IActionResult MarkAttendance(DateTime date, string grade, string subject)
        {
            var students = _db.Students
                .Where(s => s.Grade == grade && s.Subject == subject)
                .ToList();

            ViewBag.Date = date;
            ViewBag.Grade = grade;
            ViewBag.Subject = subject;

            return View("AttendanceList", students);
        }

        [HttpPost]
        public IActionResult SaveAttendance(List<int> presentStudentIds, DateTime date, string grade, string subject)
        {
            var students = _db.Students
                .Where(s => s.Grade == grade && s.Subject == subject)
                .ToList();

            foreach (var student in students)
            {
               
                var isPresent = presentStudentIds != null && presentStudentIds.Contains(student.Id);

                var status = isPresent ? "Present" : "Absent";

                _db.Attendances.Add(new Attendance
                {
                    StudentId = student.Id,  
                    Date = date,
                    Grade = grade,
                    Subject = subject,
                    Status = status
                });
            }

            _db.SaveChanges();

            return RedirectToAction("MarkAttendance");
        }
        public IActionResult ViewAttendanceRecords(string? grade, string? subject, DateTime? date)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            var query = _db.Attendances
                .Include(a => a.Student)
                .AsQueryable();

            if (!string.IsNullOrEmpty(grade))
                query = query.Where(a => a.Grade == grade);

            if (!string.IsNullOrEmpty(subject))
                query = query.Where(a => a.Subject == subject);

            if (date.HasValue)
                query = query.Where(a => a.Date.Date == date.Value.Date);

            var records = query
                .OrderByDescending(a => a.Date)
                .ThenBy(a => a.Grade)
                .ThenBy(a => a.Student.FullName)
                .ToList();

            ViewBag.Grade = grade;
            ViewBag.Subject = subject;
            ViewBag.Date = date;

            ViewBag.Grades = Enumerable.Range(1, 12).Select(g => "Grade " + g).ToList();

            ViewBag.Subjects = new List<string>
    {
        "Sinhala",
        "Tamil",
        "English",
        "Mathematics",
        "Science",
        "History",
        "Geography",
        "Health & Physical Education",
        "ICT",
        "Dancing",
        "English Literature",
        "Sinhala Literature",       
    };

            return View(records);
        }
    }
}