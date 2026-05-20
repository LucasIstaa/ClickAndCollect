using ClickAndCollect.Filters;
using ClickAndCollect.Models.Classes;
using ClickAndCollect.Models.DALInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClickAndCollect.Controllers
{
    public class OrderMakerController : Controller
    {
        private readonly IOrderDAL orderDAL;

        public OrderMakerController(IOrderDAL dal)
        {
            this.orderDAL = dal;
        }

        public IActionResult Index()
        {
            return RedirectToAction("CheckTomorrowOrders");
        }

        [RoleFilter("OrderMaker")]
        public async Task<IActionResult> CheckTomorrowOrders()
        {

            int? storeId = HttpContext.Session.GetInt32("StoreId");
            if (storeId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            List<Order> orders = await Order.GetTomorrowOrdersByStoreAsync(orderDAL, storeId.Value);

            if (orders.Count == 0)
            {
                ViewBag.Message = "No orders for tomorrow";
            }

            return View("CheckTomorrowOrders", orders);
        }

        [RoleFilter("OrderMaker")]
        public async Task<IActionResult> ViewOrderDetails(int orderId)
        {

            Order? order = await Order.GetOrderAsync(orderDAL, orderId);
            if (order == null)
            {
                return RedirectToAction("CheckTomorrowOrders");
            }

            order.Orderlines = await Order.GetOrderlinesAsync(orderDAL,orderId);

            return View("ViewOrderDetails", order);
        }

        [RoleFilter("OrderMaker")]
        public async Task<IActionResult> FinalizePreparation(int orderId)
        {

            Order? order = await Order.GetOrderAsync(orderDAL, orderId);
            if (order == null)
            {
                return RedirectToAction("CheckTomorrowOrders");
            }

            return View("FinalizePreparation", order);
        }

        [RoleFilter("OrderMaker")]
        [HttpPost]
        public async Task<IActionResult> ConfirmPreparation(int orderId, int boxesUsed)
        {

            Order? order = await Order.GetOrderAsync(orderDAL, orderId);
            order.BoxesInvolved = order.BoxesInvolved + boxesUsed;

            if (await Order.UpdateOrder(orderDAL, order))
            {
                return RedirectToAction("CheckTomorrowOrders");
            }

            return RedirectToAction("Logout", "Account");
        }
    }
}