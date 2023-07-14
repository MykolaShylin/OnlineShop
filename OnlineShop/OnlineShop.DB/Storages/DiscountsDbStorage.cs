using Microsoft.EntityFrameworkCore;
using OnlineShop.DB.Contexts;
using OnlineShop.DB.Migrations;
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
            product.DiscountCost = CalculateDiscount(product.Cost, discount.DiscountPercent);

            var discountDb = await TryGetByIdAsync(discount.Id);
            discountDb.Products.Add(product);

            await dataBaseContext.SaveChangesAsync();
        }

        public async Task<Discount> GetByProductIdAsync(int productId)
        {
            return await dataBaseContext.Discounts.Include(x=>x.Products).FirstOrDefaultAsync(x=>x.Products.Any(z=>z.Id == productId));
        }

        public async Task<List<Product>> GetNoDiscountProductsAsync()
        {
            var discount = await GetZeroDiscountAsync();
            return discount.Products;

        }
        public async Task<Discount> GetZeroDiscountAsync()
        {
            return await dataBaseContext.Discounts.Include(x => x.Products).ThenInclude(x => x.Flavors).Include(x => x.Products).ThenInclude(x => x.Pictures).FirstOrDefaultAsync(x => x.DiscountPercent == 0);
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
            var discount = await TryGetByIdAsync(discountId);
            discount.Products.Remove(product);

            var zeroDiscount = await GetZeroDiscountAsync();
            zeroDiscount.Products.Add(product);

            product.DiscountDescription = string.Empty;
            product.DiscountCost = product.Cost;


            await dataBaseContext.SaveChangesAsync();
        }

        public decimal CalculateDiscount(decimal cost, int discountPercent)
        {
            return decimal.Ceiling(((cost * 100) * (100 - discountPercent) / 100) / 100);
        }
    }
}
