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
        private readonly IOrderlineDAL olDAL;
        private readonly IUserDAL userDAL;

        public OrderController(IOrderDAL odal, IStoreDAL sdal, ITimeslotDAL tdal, IOrderlineDAL oldal, IUserDAL idal)
        {
            this.orderDAL = odal;
            this.storeDAL = sdal;
            this.tslotDAL = tdal;
            this.olDAL = oldal;
            this.userDAL = idal;
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

            Cart c = HttpContext.Session.GetObject<Cart>("cart");
            

            if (c != null && c.Lines != null) 
            {
                int? cid = HttpContext.Session.GetInt32("UserId");
                User? client = await Models.Classes.User.GetUser(cid.Value, userDAL);
                Store st = await Store.GetStoreAsync(storeDAL, storeid);
                st.Timeslots = await Timeslot.GetStoreTimeslotsAsync(tslotDAL,st.StoreId);
                Timeslot? t = st.GetTimeslotById(timeslotid);
                DateOnly tomorrow = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
                Order o = new Order(-1,OrderStatus.Placed, 0, (Client) client, st, t, null,tomorrow);
                List<OrderLine> orderlines = new List<OrderLine>();

                foreach (CartLine line in c.Lines) 
                {
                    OrderLine ol = new OrderLine(line.Quantity, line.Product,o);
                    orderlines.Add(ol);
                }

                o.Orderlines = orderlines;

                
                    if (await Order.AddOrderAsync(orderDAL, o))
                    {
                        if (await OrderLine.AddOrderlinesAsync(olDAL, o.Orderlines))
                        {
                            HttpContext.Session.Remove("cart");
                            return View("OrderConfirmation", o);
                        }
                        
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
