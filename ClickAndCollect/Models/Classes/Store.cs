using ClickAndCollect.Models.DALInterfaces;

namespace ClickAndCollect.Models.Classes
{
    public class Store
    {
        private int storeid;
        private string phonenumber;
        private string name;
        private int postalcode;
        private string cityname;
        private string streetname;
        private int housenumber;
        private List<Order> orders = new List<Order>();
        private List<Timeslot> timeslots = new List<Timeslot>();
        private List<OrderMaker> ordermakers = new List<OrderMaker>();
        private List<Cashier> cashiers = new List<Cashier>();

        public int StoreId
        {
            get { return storeid; }
            set { storeid = value; }
        }

        public string PhoneNumber
        {
            get { return phonenumber; }
            set { phonenumber = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int PostalCode
        {
            get { return postalcode; }
            set { postalcode = value; }
        }

        public string CityName
        {
            get { return cityname; }
            set { cityname = value; }
        }

        public string StreetName
        {
            get { return streetname; }
            set { streetname = value; }
        }

        public int HouseNumber
        {
            get { return housenumber; }
            set { housenumber = value; }
        }

        public void AddOrder(Order order) 
        {
            if (!orders.Contains(order))
            {
                orders.Add(order);
            }
            else 
            {
                throw new ArgumentException("Order already in list of store");
            }
        }

        public void AddTimeslot(Timeslot slot)
        {
            if (!timeslots.Contains(slot))
            {
                timeslots.Add(slot);
            }
            else
            {
                throw new ArgumentException("Timeslot already in list of store");
            }
        }

        public void AddOrdermaker(OrderMaker maker)
        {
            if (!ordermakers.Contains(maker))
            {
                ordermakers.Add(maker);
            }
            else 
            {
                throw new ArgumentException("Ordermaker already in store");
            }
        }

        public void AddCashier(Cashier cashier) 
        {
            if (!cashiers.Contains(cashier))
            {
                cashiers.Add(cashier);
            }
            else 
            {
                throw new ArgumentException("Cashier already in store");
            }
        }

        public void RemoveOrder(Order order)
        {
            if (orders.Contains(order))
                orders.Remove(order);
            else
                throw new ArgumentException("Order not found in store");
        }

        public void RemoveTimeslot(Timeslot slot)
        {
            if (timeslots.Contains(slot))
                timeslots.Remove(slot);
            else
                throw new ArgumentException("Timeslot not found in store");
        }

        public void RemoveOrdermaker(OrderMaker maker)
        {
            if (ordermakers.Contains(maker))
                ordermakers.Remove(maker);
            else
                throw new ArgumentException("Ordermaker not found in store");
        }

        public void RemoveCashier(Cashier cashier)
        {
            if (cashiers.Contains(cashier))
                cashiers.Remove(cashier);
            else
                throw new ArgumentException("Cashier not found in store");
        }

        //Constructeurs

        public Store(string phonenumber, string name, int postalcode, string cityname, string streetname, int housenumber, TimeOnly start, TimeOnly end)
        {
            PhoneNumber = phonenumber;
            Name = name;
            PostalCode = postalcode;
            CityName = cityname;
            StreetName = streetname;
            HouseNumber = housenumber;
            AddTimeslot(new Timeslot(start, end, this));
        }

        public Store(int storeid, string phonenumber, string name, int postalcode, string cityname, string streetname, int housenumber, TimeOnly start, TimeOnly end) : this(phonenumber, name, postalcode, cityname, streetname, housenumber, start, end)
        {
            this.storeid = storeid;
        }

        //Méthodes

        public static async Task<bool> AddStoreAsync(Store s, IStoreDAL dal) 
        {
            bool ok = await dal.AddStoreAsync(s);

            return ok;
        }

        public static async Task<List<Store>> GetAllStoreAsync(IStoreDAL dal)
        {
            return await dal.GetAllStoresAsync();
        }

        public static async Task<Store> GetStoreAsync(IStoreDAL dal, int id) 
        {
            return await dal.GetStoreAsync(id);
        }

        public override bool Equals(object? obj)
        {
            return obj is Store store &&
                   storeid == store.storeid;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(storeid);
        }
    }
}
