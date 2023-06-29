using OnlineShop.DB.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShop.DB.Models.Interfaces
{
    public interface IBasketStorage
    {
        Task<Basket> TryGetByUserIdAsync(string id);
        Task AddAsync(string userId, Product product, ChoosingProductInfo productInfo, int amount);
        Task DeleteAsync(string userId, int prodId);
        Task CloseAsync(string userId);
        Task<Basket> TryGetExistingByUserIdAsync(string userId);
    }
}
