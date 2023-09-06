using Microsoft.EntityFrameworkCore;
using OnlineShop.DB.Contexts;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShop.DB.Storages
{
    public class ComparingProductsDbStorage : IProductComparer
    {
        private List<ComparingProducts> _comparingProducts;
        private readonly DataBaseContext dataBaseContext;

        public ComparingProductsDbStorage(DataBaseContext dataBaseContext)
        {
            _comparingProducts = new List<ComparingProducts>();
            this.dataBaseContext = dataBaseContext;
        }

        public async Task<ComparingProducts> GetLastAsync(string userId)
        {
            return (await dataBaseContext.ComparingProducts.Include(x => x.Product).ThenInclude(x => x.Flavors).Include(x => x.Product).ThenInclude(x => x.Pictures).Include(x => x.Flavor).ToListAsync()).Last(x => x.UserId == userId);
        }
        public async Task<ComparingProducts> TryGetByIdAsync(int comparerId)
        {
            return await dataBaseContext.ComparingProducts.Include(x => x.Product).ThenInclude(x => x.Flavors).Include(x => x.Product).ThenInclude(x => x.Pictures).Include(x => x.Flavor).FirstOrDefaultAsync(x => x.Id == comparerId);
        }
        public async Task<List<ComparingProducts>> GetAllAsync(string userId)
        {
            return await dataBaseContext.ComparingProducts.Where(x => x.UserId == userId).Include(x => x.Product).ThenInclude(x => x.Flavors).Include(x => x.Product).ThenInclude(x => x.Pictures).Include(x => x.Flavor).ToListAsync();
        }
        public async Task AddAsync(string userId, Product product, Flavor flavor)
        {
            var existingComparer = await GetAllAsync(userId);
            if (existingComparer == null || existingComparer.Count < 3 && !existingComparer.Any(x => x.Product.Name == product.Name && x.Flavor.Name == flavor.Name))
            {
                var comparer = new ComparingProducts()
                {
                    UserId = userId,
                    Product = product,
                    Flavor = flavor
                };
                dataBaseContext.ComparingProducts.Add(comparer);
            }
            else if (existingComparer.Count == 3)
            {
                var lastProduct = await GetLastAsync(userId);
                dataBaseContext.ComparingProducts.Remove(lastProduct);
                var comparingProduct = new ComparingProducts()
                {
                    UserId = userId,
                    Product = product,
                    Flavor = flavor
                };
                dataBaseContext.ComparingProducts.Add(comparingProduct);
            }
        }

        public async Task DeleteAsync(int comparerId)
        {
            var existingComparer = await TryGetByIdAsync(comparerId);
            dataBaseContext.ComparingProducts.Remove(existingComparer);
        }
    }
}
