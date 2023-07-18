using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Enumerations;
using OnlineShop.DB.Models.Interfaces;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineShopWebApp.FeedbackApi;
using OnlineShopWebApp.FeedbackApi.Models;
using AutoMapper;
using Microsoft.CodeAnalysis;

namespace OnlineShopWebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductsStorage _products;
        private readonly IProductComparer _comparingProducts;
        private readonly IFlavor _flavors;
        private readonly UserManager<User> _userManager;
        private readonly FeedbackApiClient _feedbackApiClient;
        private readonly IMapper _mapping;
        private readonly IDiscount _discounts;
        public ProductController(IProductsStorage products, IProductComparer comparingProducts, IFlavor flavors, UserManager<User> userManager, FeedbackApiClient feedbackApiClient, IMapper mapping, IDiscount discounts)
        {
            this._products = products;
            _comparingProducts = comparingProducts;
            _flavors = flavors;
            _userManager = userManager;
            _feedbackApiClient = feedbackApiClient;
            _mapping = mapping;
            _discounts = discounts;
        }
        public async Task<IActionResult> Index(int prodId)
        {
            var feedbacks = await _feedbackApiClient.GetFeedbacksAsync(prodId);
            var product = await _products.TryGetByIdAsync(prodId);
            if (product != null)
            {
                var productView = _mapping.Map<ProductViewModel>(product);
                productView.Feedbacks = _mapping.Map<List<FeedbackViewModel>>(feedbacks);
                return View(productView);
            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> AddFeedback(int productId)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var newFeedback = new AddFeedbackModel() { ProductId = productId, UserId = user.Id };
            return View(newFeedback);
        }


        public async Task<IActionResult> DeleteFeedbackAsync(int feedbackId, int productId)
        {
            await _feedbackApiClient.DeleteAsync(feedbackId);
            return RedirectToAction(nameof(Index), new { prodId = productId });
        }

        [HttpPost]
        public async Task<IActionResult> AddFeedbackAsync(AddFeedbackModel feedbackModel)
        {

            await _feedbackApiClient.AddAsync(feedbackModel);
            return RedirectToAction(nameof(Index), new { prodId = feedbackModel.ProductId });
        }

        [Authorize]
        public async Task<IActionResult> Comparing(int productId, int flavorId)
        {
            var userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            var product = await _products.TryGetByIdAsync(productId);
            var flavor = await _flavors.TryGetByIdAsync(flavorId);
            await _comparingProducts.AddAsync(userId, product, flavor);
            return RedirectToAction(nameof(Index), new { prodId = productId });
        }

        [Authorize]
        public async Task<IActionResult> Deleting(int prodId)
        {
            await _comparingProducts.DeleteAsync(prodId);
            return RedirectToAction(nameof(CheckComparer));
        }

        [Authorize]
        public async Task<IActionResult> CheckComparer()
        {
            var userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            var comparingProducts = await _comparingProducts.GetAllAsync(userId);
            var comparingView = _mapping.Map<List<ComparingProductsViewModel>>(comparingProducts);
            ViewBag.UserId = userId;
            return View(comparingView);
        }
        public async Task<IActionResult> CategoryProducts(bool isAllListProducts, ProductCategories category)
        {
            var products = await _products.GetAllAsync();
            var productsView = _mapping.Map<List<MainPageProductsViewModel>>(products);

            if (!isAllListProducts)
            {
                products = await _products.TryGetByCategoryAsync(category);
                productsView = _mapping.Map<List<MainPageProductsViewModel>>(products);
            }            
            return View(productsView);
        }

        public async Task<IActionResult> SaleProduct(string prodName)
        {
            var productId = (await _products.TryGetByNameAsync(prodName)).Id;
            return RedirectToAction(nameof(Index), new { prodId = productId});
        }
    }
}
