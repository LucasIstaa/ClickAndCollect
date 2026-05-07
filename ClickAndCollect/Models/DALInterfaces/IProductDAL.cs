namespace ClickAndCollect.Models.DALInterfaces
{
    public interface IProductDAL
    {

           public Task<bool> AddProductAsync(Product p);
           public Task<bool> UpdateProductAsync(Product p);
           public Task<bool> RemoveProductAsync(Product p);
           public Task<Product> GetProductAsync(int id);
           public Task<List<Product>> GetAllProductsAsync();
        
    }
}
