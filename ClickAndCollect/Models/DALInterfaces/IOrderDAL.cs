namespace ClickAndCollect.Models.DALInterfaces
{
    public interface IOrderDAL
    {
        public Task<List<Order>> GetTodayOrdersByStoreAsync(int storeId);
        public Task<bool> FinalizeOrderAsync(int orderId);
        public Task<Order?> GetOrderByIdAsync(int orderId);
        public Task<bool> SetBoxesReturnedAsync(int orderId, int nbBoxes);
        public Task<decimal> CalculateFinalPriceAsync(int orderId, int boxesReturned);
    }
}