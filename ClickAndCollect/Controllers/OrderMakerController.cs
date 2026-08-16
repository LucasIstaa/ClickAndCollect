using ClickAndCollect.Filters;
using ClickAndCollect.Models.Classes;
using Microsoft.AspNetCore.Mvc;
using ClickAndCollect.Models.DALInterfaces;

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
            int? storeId = HttpContext.Session.GetInt32("StoreId");
            if (storeId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Order? order = await Order.GetOrderAsync(orderDAL, orderId);
            if (order == null || order.Store.StoreId != storeId.Value)
            {
                return RedirectToAction("CheckTomorrowOrders");
            }

            order = await Order.GetOrderlinesAsync(orderDAL,order);

            foreach (OrderLine l in order.Orderlines) 
            {
                l.Order = order;
            }


            return View("ViewOrderDetails", order);
        }

        [RoleFilter("OrderMaker")]
        public async Task<IActionResult> FinalizePreparation(int orderId)
        {
            int? storeId = HttpContext.Session.GetInt32("StoreId");
            if (storeId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Order? order = await Order.GetOrderAsync(orderDAL, orderId);
            if (order == null || order.Store.StoreId != storeId.Value || order.Status != OrderStatus.Placed)
            {
                return RedirectToAction("CheckTomorrowOrders");
            }

            return View("FinalizePreparation", order);
        }

        [RoleFilter("OrderMaker")]
        [HttpPost]
        public async Task<IActionResult> ConfirmPreparation(int orderId, int boxesUsed)
        {
            int? storeId = HttpContext.Session.GetInt32("StoreId");
            if (storeId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Order? order = await Order.GetOrderAsync(orderDAL, orderId);
            if (order == null || order.Store.StoreId != storeId.Value || order.Status != OrderStatus.Placed)
            {
                return RedirectToAction("CheckTomorrowOrders");
            }

            if (boxesUsed < 0)
            {
                ViewBag.Error = "Invalid number of boxes used.";
                return View("FinalizePreparation", order);
            }

            if (await Order.FinalizePreparationAsync(orderDAL, orderId, boxesUsed))
            {
                return RedirectToAction("CheckTomorrowOrders");
            }

            return RedirectToAction("Logout", "Account");
        }
    }
}