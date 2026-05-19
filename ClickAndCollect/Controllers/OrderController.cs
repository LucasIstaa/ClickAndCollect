using ClickAndCollect.Filters;
using ClickAndCollect.Models.Classes;
using ClickAndCollect.Models.DALClasses;
using ClickAndCollect.Models.DALInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClickAndCollect.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderDAL orderDAL;
        private readonly IStoreDAL storeDAL;
        private readonly ITimeslotDAL tslotDAL;

        public OrderController(IOrderDAL odal, IStoreDAL sdal, ITimeslotDAL tdal)
        {
            this.orderDAL = odal;
            this.storeDAL = sdal;
            this.tslotDAL = tdal;
        }

        public IActionResult Index()
        {
            return View();
        }

        [RoleFilter("Client")]
        public async Task<IActionResult> ViewOrdersHistory()
        {
            int? cid = HttpContext.Session.GetInt32("UserId");

            List<Order> orders = await Order.GetClientOrdersAsync(orderDAL, cid.Value);

            return View("ViewOrderHistory", orders);
        }

        [RoleFilter("Client")]
        public async Task<IActionResult> OrderDetails(int id) 
        {

            Order o = await Order.GetOrderAsync(orderDAL,id);
            Store st = await Store.GetStoreAsync(storeDAL, o.Store.StoreId);
            o.Store = st;

            return View("Details", o);
        }

        [RoleFilter("Client")]
        public async Task<IActionResult> PlaceOrder(int storeid, int timeslotid) 
        {
            int? cid = HttpContext.Session.GetInt32("UserId");
            Cart c = HttpContext.Session.GetObject<Cart>("cart");
            Client cl = new Client(cid.Value,null,null,null, null, null, 0, null, null, 0);
            Store st = await Store.GetStoreAsync(storeDAL, storeid);

            Timeslot t = await Timeslot.GetTimeslotAsync(tslotDAL,timeslotid);
            DateOnly tomorrow = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

            if (c != null) 
            {
                Order o = new Order(-1,OrderStatus.Placed, 0, cl, st, t, tomorrow);
                List<OrderLine> orderlines = new List<OrderLine>();

                foreach (CartLine line in c.Lines) 
                {
                    OrderLine ol = new OrderLine(line.Quantity, line.Product,o);
                    orderlines.Add(ol);
                }

                o.Orderlines = orderlines;
                if (await Order.AddOrderAsync(orderDAL, o)) 
                {
                    HttpContext.Session.Remove("cart");
                    return View("OrderConfirmation", o);
                }
                
            }

            return View("Home");
        }

        [RoleFilter("Client")]
        public async Task<IActionResult> ChooseStore()
        {

            List<Store> stores = await Store.GetAllStoreAsync(storeDAL);

            return View("ChooseStore", stores);
        }

        [RoleFilter("Client")]
        public async Task<IActionResult> ChooseTimeslot(int id)
        {

            List<Timeslot> tslots = await Timeslot.GetStoreTimeslotsAsync(tslotDAL, id);
            List<Timeslot> availabletslot = new List<Timeslot>();

            foreach (Timeslot t in tslots) 
            {
                if (t.OrderNumber <= 9) 
                {
                    availabletslot.Add(t);
                }
            }

            return View("ChooseTimeslot", availabletslot);
        }
    }
}
