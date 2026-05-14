using ClickAndCollect.Models.DALInterfaces;
using Microsoft.Data.SqlClient;

namespace ClickAndCollect.Models.DALClasses
{
    public class CategoryDAL : ICategoryDAL
    {
        private String connectionString;

        public CategoryDAL(String conn)
        {
            this.connectionString = conn;
        }

        public Task<bool> AddCategoryAsync(Category c)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            List<Category> categories = new List<Category>();

            using (SqlConnection conn = new SqlConnection(connectionString)) 
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.Category",conn);
                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int id = reader.GetInt32(reader.GetOrdinal("CategoryId"));
                        String name = reader.GetString(reader.GetOrdinal("name"));
                        Category c = new Category(id,name);
                        categories.Add(c);
                    }
                }
            }
            return categories;
        }

        public async Task<Category> GetCategoryAsync(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.Category WHERE CategoryId=@catid", conn);
                cmd.Parameters.AddWithValue("catid", id);
                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if(await reader.ReadAsync())
                    {
                        String name = reader.GetString(reader.GetOrdinal("name"));
                        return new Category(id, name);
                    }

                    return null;
                }
            }
        }

        public Task<bool> RemoveCategoryAsync(Category c)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateCategoryAsync(Category c)
        {
            throw new NotImplementedException();
        }
    }
}
