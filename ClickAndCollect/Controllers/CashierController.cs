using ClickAndCollect.Filters;
using ClickAndCollect.Models;
using ClickAndCollect.Models.Classes;
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

        [RoleFilter("Cashier")]
        public async Task<IActionResult> ConsultTodayClientList()
        {

            int? storeId = HttpContext.Session.GetInt32("StoreId");
            if (storeId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            List<Order> orders = await Order.GetTodayOrdersByStoreAsync(orderDAL,storeId.Value);

            if (orders.Count == 0)
            {
                ViewBag.Message = "No orders for today";
            }

            return View("ConsultTodayClientList", orders);
        }

        [RoleFilter("Cashier")]
        public async Task<IActionResult> FinalizeOrder(int orderId)
        {

            Order? order = await Order.GetOrderAsync(orderDAL,orderId);
            if (order == null)
            {
                return RedirectToAction("ConsultTodayClientList");
            }

            ViewBag.Step = "EnterBoxes";
            return View("FinalizeOrder", order);
        }

        [RoleFilter("Cashier")]
        [HttpPost]
        public async Task<IActionResult> SubmitBoxes(int orderId, int boxesReturned)
        {

            await Order.SetBoxesReturnedAsync(orderDAL,orderId, boxesReturned);

            decimal finalPrice = await Order.CalculateFinalPriceAsync(orderDAL,orderId, boxesReturned);

            Order? order = await Order.GetOrderAsync(orderDAL, orderId);
            ViewBag.Step = "ShowPrice";
            ViewBag.FinalPrice = finalPrice;

            return View("FinalizeOrder", order);
        }

        [RoleFilter("Cashier")]
        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(int orderId)
        {
            await Order.FinalizeOrderAsync(orderDAL,orderId);

            Order? order = await Order.GetOrderAsync(orderDAL, orderId);
            ViewBag.Step = "Finalized";

            return View("FinalizeOrder", order);
        }
    }
}