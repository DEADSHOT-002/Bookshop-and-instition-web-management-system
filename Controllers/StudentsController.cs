using Microsoft.AspNetCore.Mvc;
using BookshopTuitionSystem.Data;
using BookshopTuitionSystem.Models;
using System.Linq;

namespace BookshopTuitionSystem.Controllers
{
    public class StudentsController : Controller
    {
        private readonly AppDbContext _db;

        public StudentsController(AppDbContext db)
        {
            _db = db;
        }

        private bool NotLoggedIn()
        {
            return HttpContext.Session.GetInt32("UserId") == null;
        }


        public IActionResult Index(string name, string grade, int? id)
        {
            var students = _db.Students.AsQueryable();

            if (id.HasValue)
            {
                students = students.Where(s => s.Id == id.Value);
            }

          
            if (!string.IsNullOrEmpty(name))
            {
                students = students.Where(s => s.FullName.Contains(name));
            }

          
            if (!string.IsNullOrEmpty(grade))
            {
                students = students.Where(s => s.Grade.Contains(grade));
            }

            return View(students.ToList());
        }


        public IActionResult Create()
        {
            if (NotLoggedIn())
                return RedirectToAction("Login", "Account");

            return View();
        }


        [HttpPost]
        public IActionResult Create(Student s)
        {
            if (!ModelState.IsValid)
            {
                return View(s);
            }

            
            if (string.IsNullOrWhiteSpace(s.Course))
            {
                s.Course = "General";
            }

            _db.Students.Add(s);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }


        public IActionResult Edit(int id)
        {
            var s = _db.Students.Find(id);
            return View(s);
        }

        
        [HttpPost]
        public IActionResult Edit(Student s)
        {
            _db.Students.Update(s);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }


        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Owner/Admin")
                return RedirectToAction("Index");

            var student = _db.Students.Find(id);

            if (student != null)
            {
                _db.Students.Remove(student);
                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}