using ClickAndCollect.Models.DALInterfaces;
using System.Text.Json.Serialization;

namespace ClickAndCollect.Models.Classes
{
    public class Category
    {
        private int id;
        private string name;

        [JsonIgnore]
        private List<Product> products;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [JsonConstructor]
        public Category(int id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.products = new List<Product>();
        }

        public Category(string name)
        {
            this.Name = name;
            this.products = new List<Product>();
        }

        public void AddProduct(Product pro)
        {
            if (!products.Contains(pro))
                products.Add(pro);
        }

        public void RemoveProduct(Product pro)
        {
            if (!products.Contains(pro))
                throw new ArgumentException("Product not found in category");

            products.Remove(pro);
        }

        public async static Task<List<Category>> GetAllCategories(ICategoryDAL dal)
        {
            return await dal.GetAllCategoriesAsync();
        }

        public async static Task<Category> GetCategory(int id, ICategoryDAL dal)
        {
            return await dal.GetCategoryAsync(id);
        }
    }
}
