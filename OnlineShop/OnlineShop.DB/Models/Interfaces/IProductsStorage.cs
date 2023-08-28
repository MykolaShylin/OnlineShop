using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Enumerations;
using System;
using System.Collections.Generic;

namespace OnlineShop.DB.Models.Interfaces
{
    public interface IProductsStorage
    {
        Task SaveAsync(Product product);
        Task<Product> TryGetByIdAsync(int id);
        Task<List<Product>> GetAllAsync();
        Task DeleteAsync(Product product);
        Task EditAsync(Product product);
        Task<List<Product>> TryGetByCategoryAsync(ProductCategories categories);
        Task<Product> TryGetByNameAsync(string name);
        Task InitializeDefaultProductsAsync();
        Task ClearAllAsync();
        Task ReduceAmountInStock(List<BasketItem> items);
        Task<List<Product>> TryGetByBrandAsync(ProductBrands brand);
        Task<List<Product>> GetByPageNumber(int pageNumber, int productCount);
    }
}
