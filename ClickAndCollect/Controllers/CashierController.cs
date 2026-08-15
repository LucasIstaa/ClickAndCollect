using ClickAndCollect.Filters;
using ClickAndCollect.Models.Classes;
using Microsoft.AspNetCore.Mvc;
using ClickAndCollect.Models.DALInterfaces;

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
            int? storeId = HttpContext.Session.GetInt32("StoreId");
            if (storeId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Order? order = await Order.GetOrderAsync(orderDAL,orderId);
            if (order == null || order.Store.StoreId != storeId.Value || order.Status != OrderStatus.Prepared)
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
            int? storeId = HttpContext.Session.GetInt32("StoreId");
            if (storeId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Order? order = await Order.GetOrderAsync(orderDAL, orderId);
            if (order == null || order.Store.StoreId != storeId.Value || order.Status != OrderStatus.Prepared)
            {
                return RedirectToAction("ConsultTodayClientList");
            }

            if (boxesReturned < 0 || boxesReturned > order.BoxesInvolved)
            {
                ViewBag.Step = "EnterBoxes";
                ViewBag.Error = "Invalid number of boxes returned.";
                return View("FinalizeOrder", order);
            }

            if (!await Order.SetBoxesReturnedAsync(orderDAL, orderId, boxesReturned))
            {
                return RedirectToAction("Logout", "Account");
            }

            order.BoxesInvolved -= boxesReturned;

            ViewBag.Step = "ShowPrice";
            ViewBag.FinalPrice = order.TotalPrice();

            return View("FinalizeOrder", order);
        }

        [RoleFilter("Cashier")]
        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(int orderId)
        {
            int? storeId = HttpContext.Session.GetInt32("StoreId");
            if (storeId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Order? order = await Order.GetOrderAsync(orderDAL, orderId);
            if (order == null || order.Store.StoreId != storeId.Value || order.Status != OrderStatus.Prepared)
            {
                return RedirectToAction("ConsultTodayClientList");
            }

            if (!await Order.FinalizeOrderAsync(orderDAL,orderId))
            {
                return RedirectToAction("Logout", "Account");
            }

            order.Status = OrderStatus.Finalized;
            ViewBag.Step = "Finalized";

            return View("FinalizeOrder", order);
        }
    }
}