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

        public BasketController(IProductsStorage products, IBasketStorage baskets, IPurchases closedPurchases, IFlavor flavors, UserManager<User> userManager, IMapper mapping)
        {
            _products = products;
            _baskets = baskets;
            _closedPurchases = closedPurchases;
            _flavors = flavors;
            _userManager = userManager;
            _mapping = mapping;
        }
        public async Task<IActionResult> CheckOut()
        {
            var userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            var basket = await _baskets.TryGetExistingByUserIdAsync(userId);
            var basketView = _mapping.Map<BasketViewModel>(basket);
            return View(basketView);
        }
        public async Task<IActionResult> Buying(int prodId, int flavorId, int amount)
        {
            var userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            var product = await _products.TryGetByIdAsync(prodId);
            var productInfo = new ChoosingProductInfo { ProductId = prodId, FlavorId = flavorId };
            await _baskets.AddAsync(userId, product, productInfo, amount);
            return RedirectToAction("CheckOut");
        }            

        public async Task<IActionResult> Deleting(int prodId)
        {
            var userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            await _baskets.DeleteAsync(userId, prodId);
            return RedirectToAction("CheckOut");
        }
        public async Task<IActionResult> Purchase()
        {
            var userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            var customer = await _userManager.FindByNameAsync(User.Identity.Name);
            var basket = await _baskets.TryGetExistingByUserIdAsync(userId);
            var basketView = _mapping.Map<BasketViewModel>(basket);
            ViewBag.Customer = _mapping.Map<UserViewModel>(customer);
            return View(basketView);
        }

    }
}
