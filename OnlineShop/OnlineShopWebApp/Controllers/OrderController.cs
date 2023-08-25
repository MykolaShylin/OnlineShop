using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.DB.Interfaces;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Interfaces;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineShopWebApp.Services;
using AutoMapper;
using System;
using Org.BouncyCastle.Bcpg;
using System.Linq;

namespace OnlineShopWebApp.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IBasketStorage _baskets;
        private readonly IProductsStorage _products;
        private readonly IPurchases _closedPurchases;
        private readonly UserManager<User> _userManager;
        private readonly EmailService _emailService;
        private readonly IMapper _mapping;

        public OrderController(IBasketStorage baskets, IPurchases closedPurchases, UserManager<User> userManager, EmailService emailService, IMapper mapping, IProductsStorage products, TelegramService telegramService)
        {
            _baskets = baskets;
            _closedPurchases = closedPurchases;
            _userManager = userManager;
            _emailService = emailService;
            _mapping = mapping;
            _products = products;
        }

        public IActionResult Confirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> OrderingAsync(OrderViewModel order)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var basketItems = (await _baskets.TryGetExistingByUserIdAsync(user.Id)).Items;

            var orderDb = _mapping.Map<OrderViewModel, Order>(order, opt =>
            {
                opt.AfterMap((src, dest) => dest.Items = basketItems);
                opt.BeforeMap((src, dest) => src.Id = Guid.NewGuid());
                opt.BeforeMap((src, dest) => src.OrderDateTime = DateTime.UtcNow.ToString());
            });

            var basketItemsView = _mapping.Map<List<BasketItemViewModel>>(basketItems);
            foreach(var item in basketItemsView)
            {
                item.Product.Flavor = item.Product.Flavors.First(x => x.Id == item.ProductInfo.FlavorId);
            }

            await _emailService.SendOrderConfirmEmailAsync(user.Email, basketItemsView);

            await _products.ReduceAmountInStock(orderDb.Items);
            await _closedPurchases.SaveAsync(orderDb);
            await _baskets.CloseAsync(user.Id);

            return Redirect(nameof(Confirmation));
        }
    }
}
