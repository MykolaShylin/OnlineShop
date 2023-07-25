using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Enumerations;
using System;
using System.Collections.Generic;

namespace OnlineShop.DB.Interfaces
{
    public interface IPurchases
    {
        Task SaveAsync(Order order);
        Task<Order> TryGetByIdAsync(Guid id);
        Task<List<Order>> GetAllAsync();

        Task<List<Order>> GetAllActiveByUserIdAsync(string id);
        Task UpdateStatusAsync(Guid orderId, OrderStatuses newOrderStatus);
        Task<List<Order>> TryGetByUserIdAsync(string id);

        event EventHandler<OrderStatusUpdatedEventArgs> OrderStatusUpdatedEvent;
        event EventHandler<NewComfirmedOrderEventArgs> NewComfirmedOrderEvent;
    }
}
