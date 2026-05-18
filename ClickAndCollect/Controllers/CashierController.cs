using ClickAndCollect.Models;
using ClickAndCollect.Models.DALInterfaces;
using ClickAndCollect.Models.Classes;
using Microsoft.AspNetCore.Mvc;

namespace ClickAndCollect.Controllers
{
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

        public async Task<IActionResult> FinalizeOrder(int orderId)
        {
            string? role = HttpContext.Session.GetString("Role");
            if (role != "Cashier")
            {
                return RedirectToAction("Login", "Account");
            }

            Order? order = await orderDAL.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return RedirectToAction("ConsultTodayClientList");
            }

            ViewBag.Step = "EnterBoxes";
            return View("FinalizeOrder", order);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitBoxes(int orderId, int boxesReturned)
        {
            string? role = HttpContext.Session.GetString("Role");
            if (role != "Cashier")
            {
                return RedirectToAction("Login", "Account");
            }

            await orderDAL.SetBoxesReturnedAsync(orderId, boxesReturned);

            decimal finalPrice = await orderDAL.CalculateFinalPriceAsync(orderId, boxesReturned);

            Order? order = await orderDAL.GetOrderByIdAsync(orderId);
            ViewBag.Step = "ShowPrice";
            ViewBag.FinalPrice = finalPrice;

            return View("FinalizeOrder", order);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(int orderId)
        {
            string? role = HttpContext.Session.GetString("Role");
            if (role != "Cashier")
            {
                return RedirectToAction("Login", "Account");
            }

            await orderDAL.FinalizeOrderAsync(orderId);

            Order? order = await orderDAL.GetOrderByIdAsync(orderId);
            ViewBag.Step = "Finalized";

            return View("FinalizeOrder", order);
        }
    }
}