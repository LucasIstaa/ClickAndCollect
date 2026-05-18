
﻿using ClickAndCollect.Models.Classes;

public interface IOrderDAL
{
    Task<bool> AddOrderAsync(Order o);
    Task<bool> UpdateOrderAsync(Order o);
    Task<bool> RemoveOrderAsync(Order o);
    Task<List<Order>> GetAllOrdersAsync();
    Task<List<Order>> GetClientOrdersAsync(int id);
    Task<List<OrderLine>> GetOrderlinesAsync(int id);
    Task<Order> GetOrderAsync(int id);
    Task<List<Order>> GetTodayOrdersByStoreAsync(int storeId);
    Task<bool> FinalizeOrderAsync(int orderId);
    Task<Order?> GetOrderByIdAsync(int orderId);
    Task<bool> SetBoxesReturnedAsync(int orderId, int nbBoxes);
    Task<decimal> CalculateFinalPriceAsync(int orderId, int boxesReturned);
    Task<bool> AddOrderlinesAsync(List<OrderLine> lines);
}