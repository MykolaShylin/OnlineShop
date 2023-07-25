using Microsoft.AspNetCore.Identity;
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
        public event EventHandler<OrderStatusUpdatedEventArgs> OrderStatusUpdatedEvent;
        public event EventHandler<NewComfirmedOrderEventArgs> NewComfirmedOrderEvent;
        private readonly UserManager<User> _userManager;

        public ClosedPurchasesDbStorage(DataBaseContext dataBaseContext, UserManager<User> userManager)
        {
            this.dataBaseContext = dataBaseContext;
            _userManager = userManager;
        }
        public async Task SaveAsync(Order order)
        {
            dataBaseContext.ClosedOrders.Add(order);
            await dataBaseContext.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(order.deliveryInfo.CustomerId);
            if (user.TelegramUserId != null)
            {
                NewComfirmedOrderEvent?.Invoke(
                    this,
                    new NewComfirmedOrderEventArgs()
                    {
                        User = user,
                        Order = order
                    });
            }
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

        public async Task<List<Order>> GetAllActiveByUserIdAsync(string id)
        {
            var order = await TryGetByUserIdAsync(id);
            return order.Where(x => x.orderStatus == OrderStatuses.OnTheWay
                || x.orderStatus == OrderStatuses.Created
                || x.orderStatus == OrderStatuses.Delivered).ToList();
        }
        public async Task UpdateStatusAsync(Guid orderId, OrderStatuses newOrderStatus)
        {
            var order = await TryGetByIdAsync(orderId);
            var oldStatus = order.orderStatus;
            order.orderStatus = newOrderStatus;
            await dataBaseContext.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(order.deliveryInfo.CustomerId);

            if (user.TelegramUserId != null)
            {
                OrderStatusUpdatedEvent?.Invoke(
                    this,
                    new OrderStatusUpdatedEventArgs()
                    {
                        User = user,
                        NewStatus = newOrderStatus,
                        OldStatus = oldStatus,
                        Order = order
                    });
            }
        }
    }
}
