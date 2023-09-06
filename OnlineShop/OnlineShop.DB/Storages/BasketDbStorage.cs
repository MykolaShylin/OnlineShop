using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OnlineShop.DB.Contexts;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Interfaces;

namespace OnlineShop.DB.Storages
{
    public class BasketDbStorage : IBasketStorage
    {
        private readonly DataBaseContext dataBaseContext;

        public BasketDbStorage(DataBaseContext dataBaseContext)
        {
            this.dataBaseContext = dataBaseContext;
        }
        public async Task<Basket> TryGetByUserIdAsync(string userId)
        {
            return await dataBaseContext.Basket.Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x=>x.Flavors)
                .Include(x => x.Items).ThenInclude(x => x.ProductInfo)
                .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Pictures)
                .FirstOrDefaultAsync(x => x.CustomerId == userId);
        }
        public async Task<Basket> TryGetExistingByUserIdAsync(string userId)
        {
            return await dataBaseContext.Basket.Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Flavors)
                .Include(x => x.Items).ThenInclude(x => x.ProductInfo)
                .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Pictures)
                .FirstOrDefaultAsync(x => x.CustomerId == userId && !x.IsClosed);
        }

        public async Task ChangeTemporaryUserIdAsync(string temporaryUserId, string newUserId)
        {
            var existingBasket = await TryGetExistingByUserIdAsync(temporaryUserId);
            existingBasket.CustomerId = newUserId;
            dataBaseContext.Basket.Update(existingBasket);
            await dataBaseContext.SaveChangesAsync();
        }

        public async Task CloseAsync(string userId)
        {
            var existingBasket = await TryGetExistingByUserIdAsync(userId);
            existingBasket.IsClosed= true;
            dataBaseContext.Basket.Update(existingBasket);
        }
        public async Task DeleteAsync(string userId, int prodId)
        {
            var existingBasket = await TryGetExistingByUserIdAsync(userId);
            var existingBasketItem = existingBasket.Items.FirstOrDefault(x => x.Product.Id == prodId);
            if (existingBasketItem.Amount > 1)
            {
                existingBasketItem.Amount -= 1;
            }
            else if (existingBasket.Items.Count > 1)
            {
                dataBaseContext.BasketItems.Remove(existingBasketItem);
            }
            else
            {
                dataBaseContext.Basket.Remove(existingBasket);
            }
        }

        public async Task UpdateBasket(string userId)
        {
            var existingBasket = await TryGetExistingByUserIdAsync(userId);
            if(existingBasket.Items.Sum(x=>x.Amount) == 0 )
            {
                dataBaseContext.Basket.Remove(existingBasket);
            }
            else
            {
                foreach (var item in existingBasket.Items)
                {
                    if (item.Amount == 0)
                    {
                        dataBaseContext.BasketItems.Remove(item);
                    }
                }
            }            
        }

        public async Task UpdateItem(string userId, Guid itemId, int flavorId = 0, int amount = 0)
        {
            var existingBasket = await TryGetExistingByUserIdAsync(userId);
            var existingBasketItem = existingBasket.Items.FirstOrDefault(x => x.Id == itemId);
            
            if(flavorId == 0)
            {
                existingBasketItem.Amount = amount;
            }
            else
            {
                existingBasketItem.ProductInfo.FlavorId = flavorId;
            }

            dataBaseContext.BasketItems.Update(existingBasketItem);
        }

        public async Task AddAsync(string userId, Product product, ChoosingProductInfo productInfo, int amount)
        {
            var existingBasket = await TryGetExistingByUserIdAsync(userId);
            if (existingBasket == null)
            {
                var newBasket = new Basket
                {
                    CustomerId = userId
                };
                newBasket.Items = new List<BasketItem>
                    {
                        new BasketItem
                        {
                            Amount = amount,
                            Product = product,
                            ProductInfo= productInfo                            
                        }
                    };
                dataBaseContext.Basket.Add(newBasket);
            }
            else
            {
                var existingBasketItem = existingBasket.Items.FirstOrDefault(x => x.Product.Id == productInfo.ProductId && x.ProductInfo.FlavorId == productInfo.FlavorId);
                if (existingBasketItem != null)
                {
                    existingBasketItem.Amount += 1;
                }
                else
                {
                    existingBasket.Items.Add(new BasketItem
                    {
                        Amount= amount,
                        Product = product,
                        ProductInfo = productInfo,
                        Basket = existingBasket
                    });
                }
            }
        }
    }
}
