using Microsoft.AspNetCore.Mvc;
using BookshopTuitionSystem.Data;
using BookshopTuitionSystem.Models;
using System.Linq;

namespace BookshopTuitionSystem.Controllers
{
    public class ItemsController : Controller
    {
        private readonly AppDbContext _db;

        public ItemsController(AppDbContext db)
        {
            _db = db;
        }


        private bool NotLoggedIn()
        {
            return HttpContext.Session.GetInt32("UserId") == null;
        }

        private bool IsOwner()
        {
            return HttpContext.Session.GetString("Role") == "Owner/Admin";
        }

        private bool IsStaffOrOwner()
        {
            var role = HttpContext.Session.GetString("Role");
            return role == "Owner/Admin" || role == "Staff";
        }


        public IActionResult Index(string search)
        {
            if (NotLoggedIn())
                return RedirectToAction("Login", "Account");

            var items = _db.Items.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                items = items.Where(i =>
                    i.Type.Contains(search) ||
                    i.Brand.Contains(search) ||
                    i.Supplier.Contains(search));
            }

            return View(items.ToList());
        }


        public IActionResult Create()
        {
            if (NotLoggedIn())
                return RedirectToAction("Login", "Account");

            if (!IsStaffOrOwner())
                return RedirectToAction("Index");

            return View();
        }

        [HttpPost]
        public IActionResult Create(Item i)
        {
            if (!IsStaffOrOwner())
                return RedirectToAction("Index");

            _db.Items.Add(i);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }


        public IActionResult Edit(int id)
        {
            if (!IsOwner())
                return RedirectToAction("Index");

            var item = _db.Items.Find(id);
            if (item == null)
                return RedirectToAction("Index");

            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(Item i)
        {
            if (!IsOwner())
                return RedirectToAction("Index");

            _db.Items.Update(i);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }



        public IActionResult Delete(int id)
        {
            if (!IsOwner())
                return RedirectToAction("Index");

            var item = _db.Items.Find(id);

            if (item != null)
            {
                _db.Items.Remove(item);
                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}