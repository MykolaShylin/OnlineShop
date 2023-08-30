using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using OnlineShopWebApp.Models;
using System;
using OnlineShop.DB.Models;
using OnlineShopWebApp.Helpers;
using OnlineShop.DB.Models.Interfaces;
using OnlineShop.DB.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace OnlineShopWebApp.Controllers
{
    [Authorize]
    public class BasketController : Controller
    {
        private readonly IProductsStorage _products;
        private readonly IBasketStorage _baskets;
        private readonly IPurchases _closedPurchases;
        private readonly IFlavor _flavors;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapping;
        private readonly IDiscount _discounts;

        public BasketController(IProductsStorage products, IBasketStorage baskets, IPurchases closedPurchases, IFlavor flavors, UserManager<User> userManager, IMapper mapping, IDiscount discounts)
        {
            _products = products;
            _baskets = baskets;
            _closedPurchases = closedPurchases;
            _flavors = flavors;
            _userManager = userManager;
            _mapping = mapping;
            _discounts = discounts;
        }
        public async Task<IActionResult> CheckOut()
        {
            var userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            var basket = await _baskets.TryGetExistingByUserIdAsync(userId);
            var basketView = _mapping.Map<BasketViewModel>(basket);
            return View(basketView);
        }
        public async Task<IActionResult> Buying(int productId, int flavorId, int amount)
        {            
            var product = await _products.TryGetByIdAsync(productId);
            var userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            var discount = (await _discounts.GetByProductIdAsync(productId)).DiscountPercent;
            var productInfo = new ChoosingProductInfo { ProductId = productId, FlavorId = flavorId, Cost = product.Cost, DiscountPercent = discount };
            await _baskets.AddAsync(userId, product, productInfo, amount);
            return RedirectToAction(nameof(CheckOut));
        }            

        public async Task<IActionResult> Deleting(int prodId)
        {
            var userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            await _baskets.DeleteAsync(userId, prodId);
            return RedirectToAction(nameof(CheckOut));
        }
        public async Task<IActionResult> Purchase(int flavorId)
        {
            var userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            var customer = await _userManager.FindByNameAsync(User.Identity.Name);
            var basket = await _baskets.TryGetExistingByUserIdAsync(userId);
            var basketView = _mapping.Map<Basket, BasketViewModel>(basket, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    dest.Customer = _mapping.Map<UserViewModel>(customer);
                    foreach (var item in dest.Items)
                    {
                        item.Product.Flavor = item.Product.Flavors.First(x => x.Id == flavorId);                        
                    }
                    foreach(var item in src.Items)
                    {
                        item.ProductInfo.FlavorId = flavorId;
                    }
                });          
            });
            return View(basketView);
        }

    }
}
