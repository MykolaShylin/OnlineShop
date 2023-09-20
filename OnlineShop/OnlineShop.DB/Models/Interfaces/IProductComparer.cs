using OnlineShop.DB.Models;
using System;
using System.Collections.Generic;

namespace OnlineShop.DB.Models.Interfaces
{
    public interface IProductComparer
    {
        Task AddAsync(string userId, Product product, Flavor flavor);
        Task DeleteAsync(int comparerId);
        Task<List<ComparingProducts>> GetAllByUserIdAsync(string userId);
        Task<ComparingProducts> TryGetByIdAsync(int comparerId);
        Task<ComparingProducts> GetLastAsync(string userId);
    }
}