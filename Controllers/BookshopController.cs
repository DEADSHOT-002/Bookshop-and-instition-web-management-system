using Microsoft.AspNetCore.Mvc;
using BookshopTuitionSystem.Data;
using System.Linq;

namespace BookshopTuitionSystem.Controllers
{
    public class BookshopController : Controller
    {
        private readonly AppDbContext _db;

        public BookshopController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner/Admin" && role != "Staff")
                return RedirectToAction("Index", "Home");

            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            ViewBag.TotalItems = _db.Items.Count();
            ViewBag.TotalSales = _db.Orders.Count();
            ViewBag.TotalRevenue = _db.Orders.Sum(o => (decimal?)o.GrandTotal) ?? 0;

            return View();
        }
    }
}