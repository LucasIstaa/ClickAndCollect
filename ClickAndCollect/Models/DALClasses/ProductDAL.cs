using ClickAndCollect.Models.DALInterfaces;
using Microsoft.Data.SqlClient;

namespace ClickAndCollect.Models.DALClasses
{
    public class ProductDAL : IProductDAL
    {

        private String connectionString;

        public ProductDAL(String conn) 
        {
            this.connectionString = conn;
        }

        public Task<bool> AddProductAsync(Product p)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            List<Product> products = new List<Product>();

            using (SqlConnection connection = new SqlConnection(connectionString)) 
            {
                SqlCommand cmd = new SqlCommand("SELECT p.ProductId,p.name AS pname,p.price,p.CategoryId,c.name FROM dbo.Product p JOIN dbo.Category c ON p.CategoryId=c.CategoryId", connection);
                await connection.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync()) 
                {
                    while (await reader.ReadAsync()) 
                    {
                        int id = reader.GetInt32(reader.GetOrdinal("ProductId"));
                        String name = reader.GetString(reader.GetOrdinal("pname"));
                        decimal price = reader.GetDecimal(reader.GetOrdinal("price"));
                        int catid = reader.GetInt32(reader.GetOrdinal("CategoryId"));
                        string cname = reader.GetString(reader.GetOrdinal("name")); 
                        Product p = new Product(id,name,price, new Category(catid,cname));
                        products.Add(p);
                    }
                }

            }
            return products;
        }

        public Task<Product> GetProductAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int? categoryid)
        {
            List<Product> products = new List<Product>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT p.ProductId,p.name AS pname,p.price,p.CategoryId,c.name FROM dbo.Product p JOIN dbo.Category c ON p.CategoryId=c.CategoryId WHERE " +
                    "p.CategoryId = @catid", connection);
                cmd.Parameters.AddWithValue("catid", categoryid);

                await connection.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int id = reader.GetInt32(reader.GetOrdinal("ProductId"));
                        String name = reader.GetString(reader.GetOrdinal("pname"));
                        decimal price = reader.GetDecimal(reader.GetOrdinal("price"));
                        int catid = reader.GetInt32(reader.GetOrdinal("CategoryId"));
                        string cname = reader.GetString(reader.GetOrdinal("name"));
                        Product p = new Product(id, name, price, new Category(catid, cname));
                        products.Add(p);
                    }
                }

            }

            return products;
        }

        public Task<bool> RemoveProductAsync(Product p)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateProductAsync(Product p)
        {
            throw new NotImplementedException();
        }
    }
}
