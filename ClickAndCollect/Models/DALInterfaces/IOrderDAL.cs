namespace ClickAndCollect.Models.DALInterfaces
{
    public interface IOrderDAL
    {
        public Task<List<Order>> GetTodayOrdersByStoreAsync(int storeId);
        public Task<bool> FinalizeOrderAsync(int orderId);
    }
}