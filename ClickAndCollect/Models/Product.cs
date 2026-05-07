using ClickAndCollect.Models.DALInterfaces;

namespace ClickAndCollect.Models
{
    public class Product
    {
        private int productid;
        private string name;
        private decimal price;

        public int ProductId
        {
            get { return productid; }
            set { productid = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public decimal Price
        {
            get { return price; }
            set { price = value; }
        }

        public Product() { }

        public Product(String name, decimal price) 
        {
            this.Name = name;
            this.Price = price; 
        }

        public Product(int id,String name, decimal price) : this(name,price)
        {
            this.ProductId = id;
        }


        public async static Task<List<Product>> GetAllProducts(IProductDAL dal) 
        {
            List<Product> products = await dal.GetAllProductsAsync();
            return products;
        }
    }
}
