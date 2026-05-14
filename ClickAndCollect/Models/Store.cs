using ClickAndCollect.Models.DAL;
using ClickAndCollect.Models.DALInterfaces;

namespace ClickAndCollect.Models
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
        private List<Order> orders;
        private List<Timeslot> timeslots;
        private List<OrderMaker> ordermakers;
        private List<Cashier> cashiers;

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

        public Store() { }

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

        public Store(int storeid, string phonenumber, string name, int postalcode, string cityname, string streetname, int housenumber)
        {
            StoreId = storeid;
            PhoneNumber = phonenumber;
            Name = name;
            PostalCode = postalcode;
            CityName = cityname;
            StreetName = streetname;
            HouseNumber = housenumber;
        }

        public Store(int storeid, string phonenumber, string name, int postalcode, string cityname, string streetname, int housenumber, TimeOnly start, TimeOnly end)
        : this(phonenumber, name, postalcode, cityname, streetname, housenumber,start,end)
        {
            StoreId = storeid;
        }

        //Méthodes

        public static async Task<bool> AddStoreAsync(Store s, IStoreDAL dal) 
        {
            bool ok = await dal.AddStoreAsync(s);

            return ok;
        }

    }
}
