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
using Microsoft.EntityFrameworkCore.Infrastructure;
using ReturnTrue.AspNetCore.Identity.Anonymous;
using System.Net;
using Microsoft.Owin.Infrastructure;
using Nancy.Session;
using Microsoft.AspNetCore.Builder;
using Org.BouncyCastle.Bcpg;

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

        [AllowAnonymous]
        public async Task<IActionResult> CheckOut()
        {
            var userId = await GetUserId();
            var basket = await _baskets.TryGetExistingByUserIdAsync(userId);
            var basketView = _mapping.Map<Basket, BasketViewModel>(basket, opt =>
            {
                if (basket != null)
                {
                    opt.AfterMap((src, dest) =>
                    {
                        dest.Customer = new UserViewModel { Id = userId };
                        dest.Items.ForEach(item => item.Product.Flavor = item.Product.Flavors.First(x => x.Id == item.ProductInfo.FlavorId));
                    });
                }
            });
            return View(basketView);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Buying(int productId, int flavorId, int amount)
        {
            var product = await _products.TryGetByIdAsync(productId);
            var userId = await GetUserId();
            var discount = (await _discounts.GetByProductIdAsync(productId)).DiscountPercent;
            var productInfo = new ChoosingProductInfo { ProductId = productId, FlavorId = flavorId, Cost = product.Cost, DiscountPercent = discount };
            await _baskets.AddAsync(userId, product, productInfo, amount);
            return RedirectToAction(nameof(CheckOut));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Deleting(int prodId)
        {
            var userId = await GetUserId();
            await _baskets.DeleteAsync(userId, prodId);
            return RedirectToAction(nameof(CheckOut));
        }
        public async Task<IActionResult> Increasing(int amount, Guid itemId)
        {
            var userId = await GetUserId();
            amount++;
            await _baskets.UpdateItem(userId, itemId, amount: amount);

            return RedirectToAction(nameof(EditBasket));
        }

        public async Task<IActionResult> Decreasing(int amount, Guid itemId)
        {
            var userId = await GetUserId();
            amount--;
            await _baskets.UpdateItem(userId, itemId, amount: amount);

            return RedirectToAction(nameof(EditBasket));
        }

        public async Task<IActionResult> ChangeFlavor(int flavorId, Guid itemId)
        {
            var userId = await GetUserId();

            await _baskets.UpdateItem(userId, itemId, flavorId: flavorId);

            return RedirectToAction(nameof(EditBasket));
        }

        public async Task<IActionResult> EditBasket()
        {
            var userId = await GetUserId();
            var basket = await _baskets.TryGetExistingByUserIdAsync(userId);
            var basketView = _mapping.Map<Basket, BasketViewModel>(basket, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    dest.Items.ForEach(item => item.Product.Flavor = item.Product.Flavors.First(x => x.Id == item.ProductInfo.FlavorId));
                });
            });
            return View(basketView);
        }
        
        public async Task<IActionResult> UpdateBasket()
        {
            var userId = await GetUserId();            

            await _baskets.UpdateBasket(userId);

            return RedirectToAction(nameof(Purchase));
        }

        public async Task<IActionResult> Purchase(string anonymousId = null)
        {
            var customer = await _userManager.FindByNameAsync(User.Identity.Name);
            if (anonymousId != null)
            {
                await _baskets.ChangeTemporaryUserIdAsync(anonymousId, customer.Id);
            }
            var basket = await _baskets.TryGetExistingByUserIdAsync(customer.Id);
            var basketView = _mapping.Map<Basket, BasketViewModel>(basket, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    dest.Customer = _mapping.Map<UserViewModel>(customer);
                    dest.Items.ForEach(item => item.Product.Flavor = item.Product.Flavors.First(x => x.Id == item.ProductInfo.FlavorId));
                });
            });
            return View(basketView);
        }

        public async Task<string> GetUserId()
        {
            var anonymousId = HttpContext.Features.Get<IAnonymousIdFeature>().AnonymousId;
            return User.Identity.IsAuthenticated ? (await _userManager.FindByNameAsync(User.Identity.Name)).Id : anonymousId;
        }

    }
}
