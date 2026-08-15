namespace ClickAndCollect.Models.Classes
{
    public class Cashier : User
    {
        private Store store;

        public Store Store
        {
            get { return store; }
            set { store = value; }
        }

        public Cashier(string username, string password, Store store) : base(username, password)
        {
            this.Store = store;
        }

        public Cashier(int id, string username, string password, Store store) : base(id, username, password)
        {
            this.Store = store;
        }

        public override string GetRole()
        {
            return "Cashier";
        }

        public override bool Equals(object? obj)
        {
            return obj is Cashier cashier &&
                   UserId == cashier.UserId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UserId);
        }
    }
}
