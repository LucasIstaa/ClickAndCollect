using ClickAndCollect.Models.Classes;
using ClickAndCollect.Models.DALClasses;
using ClickAndCollect.Models.DALInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClickAndCollect.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderDAL orderDAL;

        public OrderController(IOrderDAL odal)
        {
            this.orderDAL = odal;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ViewOrdersHistory()
        {
            //modif
            int cid = 1;

            List<Order> orders = await Order.GetClientOrdersAsync(orderDAL, cid);

            return View("ViewOrderHistory", orders);
        }
    }
}
