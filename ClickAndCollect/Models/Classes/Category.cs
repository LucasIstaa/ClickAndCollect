using ClickAndCollect.Models.DALInterfaces;
using System.Text.Json.Serialization;

namespace ClickAndCollect.Models.Classes
{
    public class Category
    {
        private int id;
        private String name;
        [JsonIgnore]
        private List<Product> products;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public String Name 
        {
            get { return name; }
            set { name = value; }
        }

        public void AddProduct(Product pro) 
        {
            if (!products.Contains(pro)) 
            {
                products.Add(pro);
            }
        }

        public void RemoveProduct(Product pro)
        {
            if (products.Contains(pro))
            {
                products.Remove(pro);
            }
            else
            {
                throw new ArgumentException("Product not found in category");
            }
        }

        public Category(string name)
        {
            this.Name = name;
            products = new List<Product>();
        }
        public Category(int id, String name) : this(name)
        {
            this.Id = id;
        }

        //Méthodes

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
