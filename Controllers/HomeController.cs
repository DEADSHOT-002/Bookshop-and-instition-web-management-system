using Microsoft.AspNetCore.Mvc;

namespace BookshopTuitionSystem.Controllers
{
    public class HomeController : Controller
    {
        private bool NotLoggedIn()
        {
            return HttpContext.Session.GetInt32("UserId") == null;
        }

        public IActionResult Index()
        {
           
            if (!NotLoggedIn())
                return RedirectToAction("SelectModule");


            return View();
        }

        public IActionResult SelectModule()
        {
            if (NotLoggedIn())
                return RedirectToAction("Login", "Account");

            return View();
        }
       
    }
}