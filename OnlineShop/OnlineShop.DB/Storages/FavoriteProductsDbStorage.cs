using OnlineShop.DB.Contexts;
using OnlineShop.DB.Models.Interfaces;
using OnlineShop.DB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineShop.DB.Helpers;
using Microsoft.Extensions.Caching.Memory;

namespace OnlineShop.DB.Storages
{
    public class FavoriteProductsDbStorage : IFavorite
    {
        private readonly DataBaseContext dataBaseContext;
        private readonly ProductEqualityComparer _comparer;
        private readonly IMemoryCache _cache;
        public FavoriteProductsDbStorage(DataBaseContext dataBaseContext, IMemoryCache cache)
        {
            this.dataBaseContext = dataBaseContext;
            _comparer = new ProductEqualityComparer();
            _cache = cache;
        }

        public async Task AddAsync(Product product, string userId)
        {
            var favorite = await GetByUserIdAsync(userId);
            if(favorite==null)
            {
                var newFavorite = new FavoriteProduct
                {
                    Products = new List<Product> { product },
                    UserId = userId
                };
                await dataBaseContext.FavoriteProduct.AddAsync(newFavorite);
            }
            else 
            {                
                if(!favorite.Products.Any(x=> _comparer.Equals(x, product)))
                {
                    favorite.Products.Add(product);
                }
            }
        }
        public async Task DeleteAsync(Product product, string userId)
        {
            var favorite = await GetByUserIdAsync(userId);
            favorite.Products.Remove(product);

            if(favorite.Products.Count == 0)
            {
                dataBaseContext.FavoriteProduct.Remove(favorite);
            }
        }

        public async Task<FavoriteProduct> GetByUserIdAsync(string userId)
        {
            if(_cache.TryGetValue(userId, out FavoriteProduct? favorites))
            {
                return favorites;
            }
            favorites = await dataBaseContext.FavoriteProduct.Include(x => x.Products).ThenInclude(x => x.Flavors).Include(x => x.Products).ThenInclude(x => x.Pictures).Include(x => x.Products).ThenInclude(x => x.FavoriteProducts).FirstOrDefaultAsync(x => x.UserId == userId);
            if(favorites != null)
            {
                _cache.Set(userId, favorites, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            return favorites;
        }

    }
}
