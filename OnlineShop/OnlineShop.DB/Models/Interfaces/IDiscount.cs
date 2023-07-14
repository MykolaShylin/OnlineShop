using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Enumerations;
using System;
using System.Collections.Generic;

namespace OnlineShop.DB.Models.Interfaces
{
    public interface IDiscount
    {
        Task<List<Discount>> GetAllByDiscountAsync(int discount);
        Task ChangeDiscountAsync(Product product, int oldDiscountId, int newDiscountId, string description);
        Task RemoveDiscountAsync(Product product, int discountId);
        Task<List<Discount>> GetAllAsync();
        Task AddAsync(Product product, Discount discount, string discountDescription);
        Task<Discount> TryGetByIdAsync(int discountId);
        Task<Discount> GetZeroDiscountAsync();
        Task<Discount> GetByProductIdAsync(int productId);
        Task<List<Product>> GetNoDiscountProductsAsync();
        decimal CalculateDiscount(decimal cost, int discountPercent);
    }
}
