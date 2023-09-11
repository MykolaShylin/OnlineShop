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
using OnlineShop.DB.Patterns;

namespace OnlineShopWebApp.Controllers
{
    [Authorize]
    public class BasketController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapping;

        public BasketController(UserManager<User> userManager, IMapper mapping, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _mapping = mapping;
            _unitOfWork = unitOfWork;
        }

        [AllowAnonymous]
        public async Task<IActionResult> CheckOut()
        {
            var userId = await GetUserId();
            var basket = await _unitOfWork.ProxyBasketDbStorage.TryGetExistingByUserIdAsync(userId);
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
            var product = await _unitOfWork.ProxyProductsDbStorage.TryGetByIdAsync(productId);
            var userId = await GetUserId();
            var discount = (await _unitOfWork.DiscountsDbStorage.GetByProductIdAsync(productId)).DiscountPercent;
            var productInfo = new ChoosingProductInfo { ProductId = productId, FlavorId = flavorId, Cost = product.Cost, DiscountPercent = discount };
            await _unitOfWork.ProxyBasketDbStorage.AddAsync(userId, product, productInfo, amount);    
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(CheckOut));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Deleting(int prodId)
        {
            var userId = await GetUserId();
            await _unitOfWork.ProxyBasketDbStorage.DeleteAsync(userId, prodId);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(CheckOut));
        }
        public async Task<IActionResult> Increasing(int amount, Guid itemId)
        {
            var userId = await GetUserId();
            amount++;
            await _unitOfWork.ProxyBasketDbStorage.UpdateItem(userId, itemId, amount: amount);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(EditBasket));
        }

        public async Task<IActionResult> Decreasing(int amount, Guid itemId)
        {
            var userId = await GetUserId();
            amount--;
            await _unitOfWork.ProxyBasketDbStorage.UpdateItem(userId, itemId, amount: amount);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(EditBasket));
        }

        public async Task<IActionResult> ChangeFlavor(int flavorId, Guid itemId)
        {
            var userId = await GetUserId();

            await _unitOfWork.ProxyBasketDbStorage.UpdateItem(userId, itemId, flavorId: flavorId);

            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(EditBasket));
        }

        public async Task<IActionResult> EditBasket()
        {
            var userId = await GetUserId();
            var basket = await _unitOfWork.ProxyBasketDbStorage.TryGetExistingByUserIdAsync(userId);
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

            await _unitOfWork.ProxyBasketDbStorage.UpdateBasket(userId);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Purchase));
        }

        public async Task<IActionResult> Purchase(string anonymousId = null)
        {
            var customer = await _userManager.FindByNameAsync(User.Identity.Name);
            if (anonymousId != null)
            {
                await _unitOfWork.ProxyBasketDbStorage.ChangeTemporaryUserIdAsync(anonymousId, customer.Id);
                await _unitOfWork.SaveChangesAsync();
            }
            var basket = await _unitOfWork.ProxyBasketDbStorage.TryGetExistingByUserIdAsync(customer.Id);
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
