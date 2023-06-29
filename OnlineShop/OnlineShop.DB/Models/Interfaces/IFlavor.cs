using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Enumerations;
using System;
using System.Collections.Generic;

namespace OnlineShop.DB.Models.Interfaces
{
    public interface IFlavor
    {
        Task SaveAsync(Flavor flavor);
        Task<Flavor> TryGetByIdAsync(int id);
        Task<List<Flavor>> GetAllAsync();
        Task DeleteAsync(Flavor flavor);
        Task<Flavor> TryGetByNameAsync(string name);
    }
}
