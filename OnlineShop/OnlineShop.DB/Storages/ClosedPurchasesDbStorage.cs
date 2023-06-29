using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OnlineShop.DB.Contexts;
using OnlineShop.DB.Interfaces;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Enumerations;

namespace OnlineShop.DB
{
    public class ClosedPurchasesDbStorage : IPurchases
    {
        private readonly DataBaseContext dataBaseContext;

        public ClosedPurchasesDbStorage(DataBaseContext dataBaseContext)
        {
            this.dataBaseContext = dataBaseContext;
        }
        public async Task SaveAsync(Order order)
        {
            dataBaseContext.ClosedOrders.Add(order);
            await dataBaseContext.SaveChangesAsync();
        }
        public async Task<Order> TryGetByIdAsync(Guid id)
        {
            return await dataBaseContext.ClosedOrders.Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Flavors).Include(x => x.payInfo).Include(x => x.deliveryInfo).Include(x => x.Items).ThenInclude(x => x.ProductInfo).FirstOrDefaultAsync(order => order.Id == id);
        }
        public async Task<List<Order>> TryGetByUserIdAsync(string id)
        {
            return await dataBaseContext.ClosedOrders.Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Flavors).Include(x => x.payInfo).Include(x => x.deliveryInfo).Include(x => x.Items).ThenInclude(x => x.ProductInfo).Where(x=>x.deliveryInfo.CustomerId == id).ToListAsync();
        }
        public async Task<List<Order>> GetAllAsync()
        {
            return await dataBaseContext.ClosedOrders.Include(x=>x.Items).ThenInclude(x=>x.Product).ThenInclude(x => x.Flavors).Include(x=>x.payInfo).Include(x=>x.deliveryInfo).Include(x => x.Items).ThenInclude(x => x.ProductInfo).ToListAsync();
        }  

        public async Task UpdateStatusAsync(Guid orderId, OrderStatuses newOrderStatus)
        {
            var order = await TryGetByIdAsync(orderId);
            order.orderStatus = newOrderStatus;
            await dataBaseContext.SaveChangesAsync();
        }
    }
}
