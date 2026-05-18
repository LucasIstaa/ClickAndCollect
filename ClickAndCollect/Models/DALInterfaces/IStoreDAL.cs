using ClickAndCollect.Models.Classes;

namespace ClickAndCollect.Models.DALInterfaces
{
    public interface IStoreDAL
    {
        Task<bool> AddStoreAsync(Store s);
        Task<bool> UpdateStoreAsync(Store s);
        Task<bool> RemoveStoreAsync(Store s);
        Task<Store> GetStoreAsync(int id);
        Task<List<Store>> GetAllStoresAsync();
        Task<List<Timeslot>> GetStoreTimeslotsAsync(int storeId);
        
    }
}
