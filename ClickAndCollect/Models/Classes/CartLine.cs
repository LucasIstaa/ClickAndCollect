namespace ClickAndCollect.Models.Classes
{
    public class CartLine
    {
        private int quantity;
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

        public CartLine() { }

        public CartLine(int quantity, Product product) 
        {
            this.Quantity = quantity;
            this.Product = product;
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
    }
}
