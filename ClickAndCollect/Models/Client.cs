namespace ClickAndCollect.Models
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
    }
}
