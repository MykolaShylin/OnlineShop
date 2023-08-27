using OnlineShop.DB.Contexts;
using OnlineShop.DB.Models.Interfaces;
using OnlineShop.DB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OnlineShop.DB.Storages
{
    public class FavoriteProductsDbStorage : IFavorite
    {
        private readonly DataBaseContext dataBaseContext;
        public FavoriteProductsDbStorage(DataBaseContext dataBaseContext)
        {
            this.dataBaseContext = dataBaseContext;
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
                if(!favorite.Products.Any(x=>x.Id == product.Id))
                {
                    favorite.Products.Add(product);
                }
            }
            await dataBaseContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(Product product, string userId)
        {
            var favorite = await GetByUserIdAsync(userId);
            favorite.Products.Remove(product);
            if(favorite.Products.Count == 0)
            {
                dataBaseContext.FavoriteProduct.Remove(favorite);
            }
            await dataBaseContext.SaveChangesAsync();
        }

        public async Task<FavoriteProduct> GetByUserIdAsync(string userId)
        {
            return await dataBaseContext.FavoriteProduct.Include(x=>x.Products).ThenInclude(x=>x.Flavors).Include(x => x.Products).ThenInclude(x => x.Pictures).Include(x => x.Products).ThenInclude(x => x.FavoriteProducts).FirstOrDefaultAsync(x => x.UserId == userId);
        }
    }
}
