namespace ClickAndCollect.Models.DALInterfaces
{
    public interface IStoreDAL
    {
        public Task<bool> AddStoreAsync(Store s);
        public Task<bool> UpdateStoreAsync(Store s);
        public Task<bool> RemoveStoreAsync(Store s);
        public Task<Store> GetStoreAsync(int id);
        public Task<List<Store>> GetAllStoresAsync();
    }
}
