namespace ClickAndCollect.Models.DALInterfaces
{
    public interface ICategoryDAL
    { 
        Task<bool> AddCategoryAsync(Category c);
        Task<bool> UpdateCategoryAsync(Category c);
        Task<bool> RemoveCategoryAsync(Category c);
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryAsync(int id);
    }
}
