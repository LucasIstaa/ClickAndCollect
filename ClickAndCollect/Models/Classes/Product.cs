using ClickAndCollect.Models.DALInterfaces;

namespace ClickAndCollect.Models.Classes
{
    public class Product
    {
        private int productid;
        private string name;
        private decimal price;
        private Category category;

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

        public Category Category
        {
            get { return category; }
            set { category = value; }
        }

        public Product() { }

        public Product(String name, decimal price, Category cat) 
        {
            this.Name = name;
            this.Price = price;
            this.Category = cat;
        }

        public Product(int id,String name, decimal price,Category cat) : this(name,price,cat)
        {
            this.ProductId = id;
        }

        //Méthodes


        public async static Task<List<Product>> GetAllProducts(IProductDAL dal) 
        {
            return await dal.GetAllProductsAsync();
            
        }

        public async static Task<List<Product>> GetProductsByCategory(IProductDAL dal, int? categoryid)
        {
            return await dal.GetProductsByCategoryAsync(categoryid);
        }

        public async static Task<Product> GetProductAsync(IProductDAL dal, int id) 
        {
            return await dal.GetProductAsync(id);
        }

        public override bool Equals(object? obj)
        {
            return obj is Product product &&
                   ProductId == product.ProductId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ProductId);
        }

        public override string ToString()
        {
            return $"{Name} (#{ProductId}) - {Price}€";
        }

    }
}
