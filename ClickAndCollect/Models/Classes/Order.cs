using ClickAndCollect.Models.DALInterfaces;

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
            set
            {
                if (value < 0)
                {
                    throw new InvalidBoxesCountException("Le nombre de caisses impliquées ne peut pas être négatif.");
                }
                boxesInvolved = value;
            }
        }

        public Client Client
        {
            get { return client; }
            set { client = value; }
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

        public Order(OrderStatus status, int boxesInvolved, Client client, Product product, int quantity, Store store, Timeslot slot, DateOnly fetchdate)
        {
            Status = status;
            BoxesInvolved = boxesInvolved;
            client.AddOrder(this);
            Client = client;
            AddOrderLine(new OrderLine(quantity, product, this));
            Store = store;
            store.AddOrder(this);
            Fetchdate = fetchdate;

            if (!slot.AddOrder(this))
            {
                throw new InvalidOperationException("Ce créneau horaire est complet (10 commandes max).");
            }

            Timeslot = slot;
        }

        public Order(int orderid, OrderStatus status, int boxesInvolved, Client client, Product product, int quantity, Store store, Timeslot slot, DateOnly fetchdate) : this(status, boxesInvolved, client, product, quantity, store, slot, fetchdate)
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

        public Order(int orderid, OrderStatus status, int boxesInvolved, Client client, Store store, Timeslot slot, DateOnly fetchdate)
        {
            OrderId = orderid;
            Status = status;
            BoxesInvolved = boxesInvolved;
            Client = client;
            client.AddOrder(this);
            Store = store;
            store.AddOrder(this);
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

        public static async Task<Order?> GetOrderAsync(IOrderDAL dal, int id)
        {
            return await dal.GetOrderAsync(id);
        }

        public Decimal TotalPrice()
        {
            Decimal tot = 0;

            foreach (OrderLine line in Orderlines)
            {
                tot = tot + (line.Quantity * line.Product.Price);
            }

            tot = tot + 5.95m + (boxesInvolved * 5.95m);

            return tot;
        }

        public static async Task<bool> AddOrderAsync(IOrderDAL dal, Order o)
        {
            return await dal.AddOrderAsync(o);
        }

        public static async Task<List<Order>> GetTomorrowOrdersByStoreAsync(IOrderDAL dal, int storeId)
        {
            return await dal.GetTomorrowOrdersByStoreAsync(storeId);
        }

        public static async Task<bool> FinalizeOrderAsync(IOrderDAL dal, int orderId)
        {
            return await dal.FinalizeOrderAsync(orderId);
        }

        public static async Task<List<OrderLine>> GetOrderlinesAsync(IOrderDAL dal, int id)
        {
            return await dal.GetOrderlinesAsync(id);
        }

        public static async Task<List<Order>> GetTodayOrdersByStoreAsync(IOrderDAL dal, int storeId)
        {
            return await dal.GetTodayOrdersByStoreAsync(storeId);
        }

        public static async Task<bool> UpdateOrder(IOrderDAL dal, Order o, OrderStatus expectedStatus)
        {
            return await dal.UpdateOrderAsync(o, expectedStatus);
        }

        public static async Task<bool> FinalizePreparationAsync(IOrderDAL dal, int orderId, int boxesUsed)
        {
            if (boxesUsed < 0)
            {
                throw new InvalidBoxesCountException("Le nombre de caisses utilisées ne peut pas être négatif.");
            }

            Order? order = await dal.GetOrderAsync(orderId);
            if (order == null)
            {
                return false;
            }

            if (order.Status != OrderStatus.Placed)
            {
                return false;
            }

            OrderStatus expectedStatus = order.Status;
            order.BoxesInvolved = order.BoxesInvolved + boxesUsed;
            order.Status = OrderStatus.Prepared;

            return await dal.UpdateOrderAsync(order, expectedStatus);
        }

        public static async Task<bool> SetBoxesReturnedAsync(IOrderDAL dal, int orderId, int nbBoxes)
        {
            if (nbBoxes < 0)
            {
                throw new InvalidBoxesCountException("Le nombre de caisses retournées ne peut pas être négatif.");
            }

            Order? order = await dal.GetOrderAsync(orderId);
            if (order == null)
            {
                return false;
            }

            if (nbBoxes > order.BoxesInvolved)
            {
                throw new ArgumentException("Le nombre de caisses retournées ne peut pas dépasser le nombre de caisses utilisées.");
            }

            OrderStatus expectedStatus = order.Status;
            order.BoxesInvolved = order.BoxesInvolved - nbBoxes;

            return await dal.UpdateOrderAsync(order, expectedStatus);
        }

        public static async Task<Decimal> CalculateFinalPriceAsync(IOrderDAL dal, int orderId)
        {
            Order? order = await dal.GetOrderAsync(orderId);
            if (order == null)
            {
                throw new ArgumentException("Commande introuvable.");
            }

            return order.TotalPrice();
        }
    }
}