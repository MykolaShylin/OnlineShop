using Microsoft.EntityFrameworkCore;
using OnlineShop.DB.Contexts;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Enumerations;
using OnlineShop.DB.Models.Interfaces;

namespace OnlineShop.DB.Storages
{
    public class DiscountsDbStorage : IDiscount
    {
        private readonly DataBaseContext dataBaseContext;
        public DiscountsDbStorage(DataBaseContext dataBaseContext)
        {
            this.dataBaseContext = dataBaseContext;
        }

        public async Task AddAsync(Product product, Discount discount, string discountDescription)
        {
            product.DiscountDescription = discountDescription;
            product.DiscountCost = decimal.Ceiling(((product.Cost * 100) * (100 - discount.DiscountPercent) / 100) / 100);
            var discounts = await GetAllAsync();

            foreach (var disc in discounts)
            {
                if (disc.Id == discount.Id)
                {
                    disc.Products.Add(product);
                    break;
                }
            }
            await dataBaseContext.SaveChangesAsync();
        }

        public async Task<Discount> TryGetByIdAsync(int discountId)
        {
            return await dataBaseContext.Discounts.Include(x => x.Products).ThenInclude(x => x.Flavors).Include(x => x.Products).ThenInclude(x => x.Pictures).FirstOrDefaultAsync(x=>x.Id == discountId);
        }

        public async Task<List<Discount>> GetAllAsync()
        {
            return await dataBaseContext.Discounts.Include(x => x.Products.Where(t => t.DiscountCost != 0)).ThenInclude(x => x.Flavors).Include(x => x.Products.Where(t=>t.DiscountCost != 0)).ThenInclude(x => x.Pictures).ToListAsync();
        }

        public async Task<List<Discount>> GetAllByDiscountAsync(int discount)
        {
            return await dataBaseContext.Discounts.Include(x => x.Products).ThenInclude(x => x.Flavors).Include(x => x.Products).ThenInclude(x => x.Pictures).Where(x=>x.DiscountPercent == discount).ToListAsync();
        }

        public async Task ChangeDiscountAsync(Product product, int oldDiscountId, int newDiscountId, string description)
        {
            var discounts = await GetAllAsync();
            if (oldDiscountId != newDiscountId)
            {
                foreach (var discount in discounts)
                {
                    if (discount.Id == oldDiscountId)
                    {
                        discount.Products.Remove(product);
                    }
                }

                foreach (var discount in discounts)
                {
                    if (discount.Id == newDiscountId)
                    {
                        product.DiscountDescription = description;
                        product.DiscountCost = decimal.Ceiling(((product.Cost * 100) * (100 - discount.DiscountPercent) / 100) / 100);
                        discount.Products.Add(product);
                        break;
                    }
                }
            }
            else
            {
                product.DiscountDescription = description;
            }
            
            await dataBaseContext.SaveChangesAsync();
        }

        public async Task RemoveDiscountAsync(Product product, int discountId)
        {
            var discounts = await GetAllAsync();

            foreach (var discount in discounts)
            {
                if (discount.Id == discountId)
                {
                    discount.Products.Remove(product);
                    product.DiscountDescription = string.Empty;
                    break;
                }
            }
            await dataBaseContext.SaveChangesAsync();
        }
    }
}
