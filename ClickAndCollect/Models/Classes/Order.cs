

namespace ClickAndCollect.Models.Classes
{
    public class Order
    {
        private int orderid;
        private OrderStatus status;
        private int boxesInvolved;
        private DateOnly fetchdate;
        private Client client;
        private List<OrderLine> orderlines = new List<OrderLine>();
        private Store store;
        private Timeslot timeslot;

        public int OrderId
        {
            get { return orderid; }
            set { orderid = value; }
        }

        public OrderStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        public DateOnly Fetchdate 
        {
            get { return fetchdate; }
            set { fetchdate = value; }
        }

        public Store Store
        {
            get { return store; }
            set { store = value; }
        }

        public int BoxesInvolved
        {
            get { return boxesInvolved; }
            set { boxesInvolved = value; }
        }

        public Client Client 
        {
            get { return client; }
            set {  client = value; }
        }

        public Timeslot Timeslot 
        {
            get { return timeslot; }
            set { timeslot = value; }
        }

        public List<OrderLine> Orderlines 
        {
            get { return orderlines; }
            set { orderlines = value; }
        }

        public void AddOrderLine(OrderLine line) 
        {
            if (!orderlines.Contains(line)) 
            {
                this.orderlines.Add(line);
            }
        }

        public Order(OrderStatus status, int boxesInvolved, Client client, Product product, int quantity, Store store, Timeslot slot,DateOnly fetchdate)
        {
            Status = status;
            BoxesInvolved = boxesInvolved;
            client.AddOrder(this);
            Client = client;
            AddOrderLine(new OrderLine(quantity,product,this));
            Store = store;
            store.AddOrder(this);
            Fetchdate = fetchdate;

            if (!slot.AddOrder(this)) 
            {
                throw new InvalidOperationException("Ce créneau horaire est complet (10 commandes max).");
            }
            
            Timeslot = slot;
        }

        public Order(int orderid,OrderStatus status, int boxesInvolved, Client client, Product product, int quantity, Store store, Timeslot slot, DateOnly fetchdate) : this(status,boxesInvolved,client,product,quantity, store, slot,fetchdate)
        {
            this.OrderId = orderid;
        }

        //pour la db
        public Order(int orderid, OrderStatus status, int boxesInvolved, Client client, Store store, Timeslot slot, List<OrderLine> lines, DateOnly fetchdate)
        {
            OrderId = orderid;
            Status = status;
            BoxesInvolved = boxesInvolved;
            Client = client;
            client.AddOrder(this);
            Store = store;
            store.AddOrder(this);
            Orderlines = lines;
            Fetchdate = fetchdate;

            if (!slot.AddOrder(this))
            {
                throw new InvalidOperationException("Ce créneau horaire est complet (10 commandes max).");
            }

            Timeslot = slot;
        }

        public Order(int id) 
        {
            OrderId = id;
        }

        //Méthodes

        public override bool Equals(object obj)
        {
            if (obj is Order other)
                return this.OrderId == other.OrderId;

            return false;
        }

        public override int GetHashCode()
        {
            return OrderId.GetHashCode();
        }

        public static async Task<List<Order>> GetClientOrdersAsync(IOrderDAL dal, int clientid) 
        {
            return await dal.GetClientOrdersAsync(clientid);
        }

    }
}
