using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using OnlineShop.DB.Contexts;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShop.DB.Storages
{
    public class ComparingProductsDbStorage : IProductComparer
    {
        private readonly DataBaseContext dataBaseContext;
        private readonly IMemoryCache _cache;

        public ComparingProductsDbStorage(DataBaseContext dataBaseContext, IMemoryCache cache)
        {
            this.dataBaseContext = dataBaseContext;
            _cache = cache;
        }

        public async Task<ComparingProducts> GetLastAsync(string userId)
        {
            return (await GetAllByUserIdAsync(userId)).Last();
        }
        public async Task<ComparingProducts> TryGetByIdAsync(int comparerId)
        {
            if (_cache.TryGetValue(comparerId, out ComparingProducts? product))
            {
                return product;
            }
            product = await dataBaseContext.ComparingProducts.Include(x => x.Product).ThenInclude(x => x.Flavors).Include(x => x.Product).ThenInclude(x => x.Pictures).Include(x => x.Flavor).FirstOrDefaultAsync(x => x.Id == comparerId);
            if (product != null)
            {
                _cache.Set(comparerId, product, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            return product;
        }
        public async Task<List<ComparingProducts>> GetAllByUserIdAsync(string userId)
        {
            if (_cache.TryGetValue(userId, out List<ComparingProducts>? products))
            {
                return products;
            }
            products = await dataBaseContext.ComparingProducts.Where(x => x.UserId == userId).Include(x => x.Product).ThenInclude(x => x.Flavors).Include(x => x.Product).ThenInclude(x => x.Pictures).Include(x => x.Flavor).ToListAsync();
            products = products.Count == 0 ? null : products;
            if (products != null)
            {
                _cache.Set(userId, products, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            return products;
        }
        public async Task AddAsync(string userId, Product product, Flavor flavor)
        {
            var existingComparer = await GetAllByUserIdAsync(userId);
            if (existingComparer == null || existingComparer.Count < 3 && !existingComparer.Any(x => x.Product.Id == product.Id && x.Flavor.Id == flavor.Id))
            {
                var comparingProduct = new ComparingProducts()
                {
                    UserId = userId,
                    Product = product,
                    Flavor = flavor
                };
                dataBaseContext.ComparingProducts.Add(comparingProduct);
                AddToCache(userId, comparingProduct);
            }
            else if (existingComparer.Count == 3)
            {
                var lastProduct = await GetLastAsync(userId);
                dataBaseContext.ComparingProducts.Remove(lastProduct);
                RemoveFromCache(userId, lastProduct);

                var comparingProduct = new ComparingProducts()
                {
                    UserId = userId,
                    Product = product,
                    Flavor = flavor
                };

                dataBaseContext.ComparingProducts.Add(comparingProduct);
                AddToCache(userId, comparingProduct);
            }            
        }

        public async Task DeleteAsync(int comparerId)
        {
            var existingComparer = await TryGetByIdAsync(comparerId);
            dataBaseContext.ComparingProducts.Remove(existingComparer);
            RemoveFromCache(existingComparer.UserId, existingComparer);
        }

        public void AddToCache(string key, ComparingProducts comparer)
        {
            _cache.TryGetValue(key, out List<ComparingProducts>? products);
            products ??= new List<ComparingProducts>();
            products.Add(comparer);
            _cache.CreateEntry(key);
        }

        public void RemoveFromCache(string key, ComparingProducts comparer)
        {
            _cache.TryGetValue(key, out List<ComparingProducts>? products);
            products.RemoveAll(x=>x.Id == comparer.Id);
            _cache.CreateEntry(key);
        }
    }
}
