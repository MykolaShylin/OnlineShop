
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache _cache;

        public ClosedPurchasesDbStorage(DataBaseContext dataBaseContext, UserManager<User> userManager, IMemoryCache cache)
        {
            this.dataBaseContext = dataBaseContext;
            _userManager = userManager;
            _cache = cache;
        }
        public async Task SaveAsync(Order order)
        {
            dataBaseContext.ClosedOrders.Add(order);
            await dataBaseContext.SaveChangesAsync();

            var user = await FindUserInCache(order.deliveryInfo.CustomerId);

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
            if (_cache.TryGetValue(id, out Order? order))
            {
                return order;
            }
            order = await dataBaseContext.ClosedOrders.Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Flavors).Include(x => x.payInfo).Include(x => x.deliveryInfo).Include(x => x.Items).ThenInclude(x => x.ProductInfo).FirstOrDefaultAsync(order => order.Id == id);
            if (order != null)
            {
                _cache.Set(id, order, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            return order;
        }
        public async Task<List<Order>> TryGetByUserIdAsync(string id)
        {
            if (_cache.TryGetValue(id, out List<Order>? orders))
            {
                return orders;
            }
            orders = await dataBaseContext.ClosedOrders.Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Flavors).Include(x => x.payInfo).Include(x => x.deliveryInfo).Include(x => x.Items).ThenInclude(x => x.ProductInfo).Where(x => x.deliveryInfo.CustomerId == id).ToListAsync();
            orders = orders.Count == 0 ? null : orders;
            if (orders != null)
            {
                _cache.Set(id, orders, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            return orders;
        }
        public async Task<List<Order>> GetAllAsync()
        {
            return await dataBaseContext.ClosedOrders.Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Flavors).Include(x => x.payInfo).Include(x => x.deliveryInfo).Include(x => x.Items).ThenInclude(x => x.ProductInfo).ToListAsync();
        }

        public async Task<List<Order>> GetAllActiveByUserIdAsync(string id)
        {
            var orders = await TryGetByUserIdAsync(id);
            return orders.Where(x => x.orderStatus == OrderStatuses.OnTheWay
                || x.orderStatus == OrderStatuses.Created
                || x.orderStatus == OrderStatuses.Delivered).ToList();
        }
        public async Task UpdateStatusAsync(Guid orderId, OrderStatuses newOrderStatus)
        {
            var order = await TryGetByIdAsync(orderId);
            var oldStatus = order.orderStatus;
            order.orderStatus = newOrderStatus;
            await dataBaseContext.SaveChangesAsync();

            var user = await FindUserInCache(order.deliveryInfo.CustomerId);

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

        private async Task<User> FindUserInCache(string userId)
        {
            _cache.TryGetValue(userId, out User? user);

            if (user == null)
            {
                user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    _cache.Set(user.Id, user, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }
            }
            return user;
        }
    }
}
