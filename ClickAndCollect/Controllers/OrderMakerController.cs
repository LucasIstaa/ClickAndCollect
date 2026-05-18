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

        public async Task<IActionResult> CheckTomorrowOrders()
        {
            string? role = HttpContext.Session.GetString("Role");
            if (role != "OrderMaker")
            {
                return RedirectToAction("Login", "Account");
            }

            int? storeId = HttpContext.Session.GetInt32("StoreId");
            if (storeId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            List<Order> orders = await orderDAL.GetTomorrowOrdersByStoreAsync(storeId.Value);

            if (orders.Count == 0)
            {
                ViewBag.Message = "No orders for tomorrow";
            }

            return View("CheckTomorrowOrders", orders);
        }

        public async Task<IActionResult> ViewOrderDetails(int orderId)
        {
            string? role = HttpContext.Session.GetString("Role");
            if (role != "OrderMaker")
            {
                return RedirectToAction("Login", "Account");
            }

            Order? order = await orderDAL.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return RedirectToAction("CheckTomorrowOrders");
            }

            order.Orderlines = await orderDAL.GetOrderlinesAsync(orderId);

            return View("ViewOrderDetails", order);
        }

        public async Task<IActionResult> FinalizePreparation(int orderId)
        {
            string? role = HttpContext.Session.GetString("Role");
            if (role != "OrderMaker")
            {
                return RedirectToAction("Login", "Account");
            }

            Order? order = await orderDAL.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return RedirectToAction("CheckTomorrowOrders");
            }

            return View("FinalizePreparation", order);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmPreparation(int orderId, int boxesUsed)
        {
            string? role = HttpContext.Session.GetString("Role");
            if (role != "OrderMaker")
            {
                return RedirectToAction("Login", "Account");
            }

            await orderDAL.FinalizePreparationAsync(orderId, boxesUsed);

            return RedirectToAction("CheckTomorrowOrders");
        }
    }
}