namespace ClickAndCollect.Models.Classes
{
    public class OrderMaker : User
    {
        private Store store;

        public Store Store
        {
            get { return store; }
            set { store = value; }
        }

        public OrderMaker( string username, string password, Store store) : base(username, password)
        {
            this.Store = store;
        }

        public OrderMaker(int id, string username, string password,Store store) : base(id, username, password)
        {
            this.Store = store;
        }

        public override bool Equals(object? obj)
        {
            return obj is OrderMaker maker &&
                   UserId == maker.UserId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UserId);
        }
    }
}
