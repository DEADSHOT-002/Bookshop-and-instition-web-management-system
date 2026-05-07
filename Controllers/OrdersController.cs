using BookshopTuitionSystem.Data;
using BookshopTuitionSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace BookshopTuitionSystem.Controllers
{
    public class OrdersController : Controller
    {
        private readonly AppDbContext _db;

        public OrdersController(AppDbContext db)
        {
            _db = db;
        }

        private bool NotLoggedIn()
        {
            return HttpContext.Session.GetInt32("UserId") == null;
        }

        private List<CartItem> GetCart()
        {
            var cart = HttpContext.Session.GetString("Cart");
            if (cart == null)
                return new List<CartItem>();

            return JsonSerializer.Deserialize<List<CartItem>>(cart);
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString("Cart",
                JsonSerializer.Serialize(cart));
        }

       
        public IActionResult Create()
        {
            if (NotLoggedIn())
                return RedirectToAction("Login", "Account");

            ViewBag.Items = _db.Items.ToList();
            ViewBag.Cart = GetCart();
            return View();
        }

    
        [HttpPost]
        public IActionResult AddToCart(int itemId, int quantity)
        {
            var item = _db.Items.Find(itemId);
            if (item == null)
                return RedirectToAction("Create");

            if (quantity <= 0)
            {
                TempData["Error"] = "Invalid quantity.";
                return RedirectToAction("Create");
            }

            var cart = GetCart();

            var existing = cart.FirstOrDefault(c => c.ItemId == itemId);

            int totalRequested = quantity;

            if (existing != null)
                totalRequested += existing.Quantity;

            
            if (item.Quantity < totalRequested)
            {
                TempData["Error"] = $"Only {item.Quantity} items available in stock.";
                return RedirectToAction("Create");
            }

            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ItemId = item.Id,
                    Type = item.Type,
                    Brand = item.Brand,
                    Quantity = quantity,
                    Price = item.SellingPrice
                });
            }

            SaveCart(cart);

            return RedirectToAction("Create");
        }

        
        [HttpPost]
        public IActionResult RemoveFromCart(int itemId, int removeQty)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ItemId == itemId);

            if (item != null)
            {
                item.Quantity -= removeQty;

                if (item.Quantity <= 0)
                    cart.Remove(item);
            }

            SaveCart(cart);

            return RedirectToAction("Create");
        }

      
        public IActionResult SalesDashboard()
        {
            var today = DateTime.Today;

            var todayOrders = _db.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.OrderDate.Date == today)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            ViewBag.TodayRevenue = todayOrders.Sum(o => o.GrandTotal);
            ViewBag.TodayOrdersCount = todayOrders.Count;

            ViewBag.TodayItemsSold = todayOrders
                .Where(o => o.OrderItems != null)
                .SelectMany(o => o.OrderItems)
                .Sum(i => i.Quantity);

            return View(todayOrders);
        }
        public IActionResult WeeklyReport(int? year, int? month, int? week)
        {
            var now = DateTime.Now;

            int selectedYear = year ?? now.Year;
            int selectedMonth = month ?? now.Month;
            int selectedWeek = week ?? 1;

            var startOfMonth = new DateTime(selectedYear, selectedMonth, 1);
            var startDate = startOfMonth.AddDays((selectedWeek - 1) * 7);
            var endDate = startDate.AddDays(7);

            var orders = _db.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.OrderDate >= startDate && o.OrderDate < endDate)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            ViewBag.TotalRevenue = orders.Sum(o => o.GrandTotal);
            ViewBag.TotalOrders = orders.Count;

            ViewBag.TotalItemsSold = orders
                .Where(o => o.OrderItems != null)
                .SelectMany(o => o.OrderItems)
                .Sum(i => i.Quantity);

           
            ViewBag.Years = new SelectList(
                Enumerable.Range(2024, 7),
                selectedYear
            );

            ViewBag.Months = new SelectList(
                Enumerable.Range(1, 12)
                .Select(m => new
                {
                    Id = m,
                    Name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m)
                }),
                "Id",
                "Name",
                selectedMonth
            );

            ViewBag.Weeks = new SelectList(
                Enumerable.Range(1, 4),
                selectedWeek
            );

            return View(orders);
        }
        public IActionResult MonthlyReport(int? year, int? month)
        {
            var now = DateTime.Now;

            int selectedYear = year ?? now.Year;
            int selectedMonth = month ?? now.Month;

            var startDate = new DateTime(selectedYear, selectedMonth, 1);
            var endDate = startDate.AddMonths(1);

            var orders = _db.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.OrderDate >= startDate && o.OrderDate < endDate)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            ViewBag.TotalRevenue = orders.Sum(o => o.GrandTotal);
            ViewBag.TotalOrders = orders.Count;

            ViewBag.TotalItemsSold = orders
                .Where(o => o.OrderItems != null)
                .SelectMany(o => o.OrderItems)
                .Sum(i => i.Quantity);

            return View(orders);
        }
        public IActionResult StockMonitor(string search)
        {
            var items = _db.Items.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                items = items.Where(i =>
                    i.Type.Contains(search) ||
                    i.Brand.Contains(search) ||
                    i.Supplier.Contains(search));
            }

            var list = items.ToList();

            ViewBag.LowStock = list.Where(i => i.Quantity > 0 && i.Quantity < 10).ToList();
            ViewBag.OutOfStock = list.Where(i => i.Quantity == 0).ToList();

            ViewBag.TotalInventoryValue = list.Sum(i => i.CostPrice * i.Quantity);

            return View(list);
        }
        public IActionResult BestSelling(string range)
        {
            var now = DateTime.Now;
            DateTime startDate;

            if (range == "weekly")
                startDate = now.AddDays(-7);
            else
                startDate = new DateTime(now.Year, now.Month, 1); 

            var data = _db.OrderItems
                .Include(o => o.Item)
                .Include(o => o.Order)
                .Where(o => o.Order.OrderDate >= startDate)
                .GroupBy(o => new { o.Item.Type, o.Item.Brand })
                .Select(g => new BestSellingViewModel
                {
                    Type = g.Key.Type,
                    Brand = g.Key.Brand,
                    QuantitySold = g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.Quantity * x.Price)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(10)
                .ToList();

            ViewBag.Range = range ?? "monthly";

            return View(data);
        }
        public IActionResult History(DateTime? selectedDate)
        {
            var date = selectedDate ?? DateTime.Today;

            var orders = _db.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.OrderDate.Date == date.Date)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            ViewBag.SelectedDate = date;

            ViewBag.TotalRevenue = orders.Sum(o => o.GrandTotal);
            ViewBag.TotalOrders = orders.Count;

            return View(orders);
        }

        [HttpPost]
        public IActionResult Checkout(decimal discount, decimal amountPaid, string paymentMethod)
        {
            var cart = GetCart();
            if (!cart.Any())
                return RedirectToAction("Create");

            foreach (var c in cart)
            {
                var item = _db.Items.Find(c.ItemId);
                if (item == null || item.Quantity < c.Quantity)
                {
                    TempData["Error"] = $"Not enough stock for {c.Type} - {c.Brand}";
                    return RedirectToAction("Create");
                }
            }

            decimal subTotal = cart.Sum(c => c.Total);
            decimal grandTotal = subTotal - discount;

            if (paymentMethod == "Cash" && amountPaid < grandTotal)
            {
                TempData["Error"] = "Amount paid is less than total!";
                return RedirectToAction("Create");
            }

            decimal change = paymentMethod == "Cash" ? amountPaid - grandTotal : 0;

            var order = new Order
            {
                InvoiceNumber = "INV-" + DateTime.Now.Ticks,
                CashierName = HttpContext.Session.GetString("Username") ?? HttpContext.Session.GetString("Role"),
                OrderDate = DateTime.Now,
                SubTotal = subTotal,
                Discount = discount,
                GrandTotal = grandTotal,
                PaymentMethod = paymentMethod ?? "Cash",
                AmountPaid = paymentMethod == "Cash" ? amountPaid : grandTotal,
                ChangeReturned = change,
                UserId = HttpContext.Session.GetInt32("UserId") ?? 0,
                OrderItems = new List<OrderItem>()
            };

            foreach (var c in cart)
            {
                var item = _db.Items.Find(c.ItemId);
                item.Quantity -= c.Quantity;
                order.OrderItems.Add(new OrderItem
                {
                    ItemId = item.Id,
                    Quantity = c.Quantity,
                    Price = c.Price
                });
            }

            _db.Orders.Add(order);
            _db.SaveChanges();

            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Create");
        }
    }
}