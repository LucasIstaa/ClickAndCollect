using ClickAndCollect.Models.Classes;

namespace ClickAndCollect.Models.DALInterfaces
{
    public interface IOrderDAL
    {
        Task<bool> AddOrderAsync(Order o);
        Task<bool> UpdateOrderAsync(Order o, OrderStatus expectedStatus);
        Task<bool> RemoveOrderAsync(Order o);
        Task<List<Order>> GetAllOrdersAsync();
        Task<List<Order>> GetClientOrdersAsync(int id);
        Task<Order> GetOrderlinesAsync(Order o);
        Task<Order?> GetOrderAsync(int id);
        Task<List<Order>> GetTodayOrdersByStoreAsync(int storeId);
        Task<bool> FinalizeOrderAsync(int orderId);
        Task<List<Order>> GetTomorrowOrdersByStoreAsync(int storeId);
    }
}