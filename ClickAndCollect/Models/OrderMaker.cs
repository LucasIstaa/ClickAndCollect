namespace ClickAndCollect.Models
{
    public class OrderMaker : User
    {
        private int storeid;
        public int StoreId
        {
            get { return storeid; }
            set { storeid = value; }
        }
    }
}
