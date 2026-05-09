namespace ClickAndCollect.Models
{
    public class Cashier : User
    {
        private int storeid;
        public int StoreId
        {
            get { return storeid; }
            set { storeid = value; }
        }
    }
}
