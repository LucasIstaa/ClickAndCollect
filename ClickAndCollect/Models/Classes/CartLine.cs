using System.Text.Json.Serialization;

namespace ClickAndCollect.Models.Classes
{
    public class CartLine: IDisposable
    {
        private int quantity;
        private bool _disposed = false;
        public Cart cart;
        private Product product;

        public int Quantity 
        {
            get { return quantity; }
            set { if (value > 0) { quantity = value; } }
        }

        public Product Product 
        {
            get { return product; }
            set { product = value; }
        }

        [JsonIgnore]
        public Cart Cart 
        {
            get { return cart; }
            set { this.cart = value; }

        }

        public CartLine(int quantity, Product product, Cart cart) 
        {
            this.Quantity = quantity;
            this.Product = product;
            this.Cart = cart;
        }

        public override bool Equals(object? obj)
        {
            return obj is CartLine line &&
                   EqualityComparer<Product>.Default.Equals(Product, line.Product);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Product);
        }

        public void Dispose()
        {
            if (!_disposed) 
            {
                _disposed = true;
                this.cart.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        ~CartLine() 
        {
            Dispose();
        }

        public void IncreaseQuantity(int amount)
        {
            if (amount <= 0)
                throw new ArgumentException("La quantité à ajouter doit être positive.");
            Quantity += amount;
        }
    }
}
