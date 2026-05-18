using ClickAndCollect.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ClickAndCollect.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string? role = HttpContext.Session.GetString("Role");
            if (role == "Cashier")
            {
                return RedirectToAction("ConsultTodayClientList", "Cashier");
            }
            if (role == "OrderMaker")
            {
                return RedirectToAction("CheckTomorrowOrders", "OrderMaker");
            }

            return View();
        }

        public IActionResult Privacy()
        {
            string? role = HttpContext.Session.GetString("Role");
            if (role == "Cashier")
            {
                return RedirectToAction("ConsultTodayClientList", "Cashier");
            }
            if (role == "OrderMaker")
            {
                return RedirectToAction("CheckTomorrowOrders", "OrderMaker");
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ClickAndCollect.Models.Classes.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
