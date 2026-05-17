namespace ClickAndCollect.Models.Classes
{
    public class Client : User
    {
        private string firstname;
        private string lastname;
        private string phonenumber;
        private int postalcode;
        private string cityname;
        private string streetname;
        private int housenumber;
        private List<Order> orders;
        private Cart cart;


        public string Firstname
        {
            get { return firstname; }
            set { firstname = value; }
        }
        public string Lastname
        {
            get { return lastname; }
            set { lastname = value; }
        }
        public string Phonenumber
        {
            get { return phonenumber; }
            set { phonenumber = value; }
        }
        public int Postalcode
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

        public Cart Cart 
        {
            get { return cart; }
            set { cart = value; }
        }

        public void AddOrder(Order o) 
        {
            if (!this.orders.Contains(o))
            {
                this.orders.Add(o);
            }
        }


        public Client(string username, string password, string firstname,string lastname, string phonenumber,
            int postalcode, string cityname, string streetname, int houseumber) : base(username, password)
        { 
            this.Firstname = firstname;
            this.Lastname = lastname;
            this.Phonenumber = phonenumber;
            this.Postalcode = postalcode;
            this.CityName = cityname;
            this.StreetName = streetname;
            this.HouseNumber = houseumber;
            this.orders = new List<Order>();
        }

        public Client(int userid,string username, string password, string firstname, string lastname, string phonenumber,
           int postalcode, string cityname, string streetname, int houseumber) : base(userid,username, password)
        {
            this.Firstname = firstname;
            this.Lastname = lastname;
            this.Phonenumber = phonenumber;
            this.Postalcode = postalcode;
            this.CityName = cityname;
            this.StreetName = streetname;
            this.HouseNumber = houseumber;
            this.orders = new List<Order>();
        }


    }
}
