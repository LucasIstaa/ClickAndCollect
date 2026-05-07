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

        public Store() { }

        public Store(string phonenumber, string name, int postalcode, string cityname, string streetname, int housenumber)
        {
            PhoneNumber = phonenumber;
            Name = name;
            PostalCode = postalcode;
            CityName = cityname;
            StreetName = streetname;
            HouseNumber = housenumber;
        }

        public Store(int storeid, string phonenumber, string name, int postalcode, string cityname, string streetname, int housenumber)
        : this(phonenumber, name, postalcode, cityname, streetname, housenumber)
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
