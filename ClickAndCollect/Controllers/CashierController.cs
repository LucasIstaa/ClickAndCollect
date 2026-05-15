using ClickAndCollect.Filters;
using ClickAndCollect.Models;
using ClickAndCollect.Models.DALInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClickAndCollect.Controllers
{
    [RoleFilter("Cashier")]
    public class CashierController : Controller
    {
        private readonly IOrderDAL orderDAL;

        public CashierController(IOrderDAL dal)
        {
            this.orderDAL = dal;
        }

        public IActionResult Index()
        {
            return RedirectToAction("ConsultTodayClientList");
        }

        public async Task<IActionResult> ConsultTodayClientList()
        {
            string? role = HttpContext.Session.GetString("Role");
            if (role != "Cashier")
            {
                return RedirectToAction("Login", "Account");
            }

            int? storeId = HttpContext.Session.GetInt32("StoreId");
            if (storeId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            List<Order> orders = await orderDAL.GetTodayOrdersByStoreAsync(storeId.Value);

            if (orders.Count == 0)
            {
                ViewBag.Message = "No orders for today";
            }

            return View("ConsultTodayClientList", orders);
        }

        [HttpPost]
        public async Task<IActionResult> FinalizeOrder(int orderId)
        {
            string? role = HttpContext.Session.GetString("Role");
            if (role != "Cashier")
            {
                return RedirectToAction("Login", "Account");
            }

            bool success = await orderDAL.FinalizeOrderAsync(orderId);
            return RedirectToAction("ConsultTodayClientList");
        }
    }
}