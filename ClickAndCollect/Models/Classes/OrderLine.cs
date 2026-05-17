namespace ClickAndCollect.Models.Classes
{
    public class OrderLine
    {
        private int orderlineid;
        private int quantity;
        private Product product;
        private Order order;
        public int OrderLineId
        {
            get { return orderlineid; }
            set { orderlineid = value; }
        }

        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        public Product Product
        { 
            get { return product; }
            set { this.product = value; }
        }

        public Order Order 
        {
            get { return order; }
            set { order = value; }
        }

        public OrderLine(int quantity, Product product, Order order) 
        {
            this.quantity = quantity;
            this.Product = product;
            this.Order = order;
            order.AddOrderLine(this);
        }

        public OrderLine(int id,int quantity, Product product, Order order) :this(quantity,product,order)
        {
            this.OrderLineId = id;
        }

        public override bool Equals(object? obj)
        {
            return obj is OrderLine line &&
                   EqualityComparer<Product>.Default.Equals(Product, line.Product) &&
                   EqualityComparer<Order>.Default.Equals(Order, line.Order);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Product, Order);
        }
    }
}
