using ClickAndCollect.Models.DALInterfaces;

namespace ClickAndCollect.Models
{
    public class Category
    {
        private int id;
        private String name;

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

        public Category() { }

        public Category(string name)
        {
            this.Name = name;
        }
        public Category(int id, String name) : this(name)
        {
            this.Id = id;
        }

        public async static Task<List<Category>> GetAllCategories(ICategoryDAL dal) 
        {
            return await dal.GetAllCategoriesAsync();
        }
        
    }
}
