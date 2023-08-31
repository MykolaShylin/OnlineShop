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
using AutoMapper;
using Microsoft.CodeAnalysis;
using OnlineShopWebApp.FeedbackApi.Models;
using OnlineShopWebApp.FeedbackApi;

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
        private readonly IFavorite _favorites;
        private const int pageProductsCount = 20;
        public ProductController(IProductsStorage products, IProductComparer comparingProducts, IFlavor flavors, UserManager<User> userManager, FeedbackApiClient feedbackApiClient, IMapper mapping, IDiscount discounts, IFavorite favorites)
        {
            this._products = products;
            _comparingProducts = comparingProducts;
            _flavors = flavors;
            _userManager = userManager;
            _feedbackApiClient = feedbackApiClient;
            _mapping = mapping;
            _discounts = discounts;
            _favorites = favorites;
        }
        public async Task<IActionResult> Index(int prodId)
        {
            var feedbacks = await _feedbackApiClient.GetFeedbacksAsync(prodId);
            var product = await _products.TryGetByIdAsync(prodId);

            var user = User.Identity.IsAuthenticated ? await _userManager.FindByNameAsync(User.Identity.Name) : null;

            var favoriteProducts = user != null ? await _favorites.GetByUserIdAsync(user.Id) : null;

            if (product != null)
            {
                var productView = _mapping.Map<ProductViewModel>(product);
                productView.Feedbacks = _mapping.Map<List<FeedbackViewModel>>(feedbacks);

                if (favoriteProducts != null)
                {
                    if (favoriteProducts.Products.Any(x => x.Id == product.Id))
                    {
                        productView.isInFavorites = true;
                    }
                }

                return View(productView);
            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> AddFavorite(int productId)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var product = await _products.TryGetByIdAsync(productId);
            await _favorites.AddAsync(product, user.Id);
            return RedirectToAction(nameof(Favorites));
        }

        [Authorize]
        public async Task<IActionResult> RemoveFavorite(int productId)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var product = await _products.TryGetByIdAsync(productId);
            await _favorites.DeleteAsync(product, user.Id);
            return RedirectToAction(nameof(Favorites));
        }

        [Authorize]
        public async Task<IActionResult> Favorites()
        {
            var userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            var favorites = _mapping.Map<FavoriteProductViewModel>(await _favorites.GetByUserIdAsync(userId));
            return View(favorites);
        }

        public async Task<IActionResult> DeleteFeedbackAsync(int feedbackId, int productId)
        {
            await _feedbackApiClient.DeleteAsync(feedbackId);
            return RedirectToAction(nameof(Index), new { prodId = productId });
        }

        [HttpPost]
        public async Task<IActionResult> AddFeedbackAsync(AddFeedbackModel feedbackModel)
        {
            feedbackModel.UserId = (await _userManager.FindByNameAsync(feedbackModel.Login)).Id;

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
            return RedirectToAction(nameof(CheckComparer));
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
            return View(comparingView);
        }
        public async Task<IActionResult> CategoryProducts(int pageNumber = 1, ProductCategories category = ProductCategories.None)
        {
            var searchingProductsIds = ((int[])TempData["searchingProductsId"])?.First() == -1? new int[0] : ((int[])TempData["searchingProductsId"]);

            var searchingProducts = searchingProductsIds?.Select(x => _products.TryGetByIdAsync(x).Result).ToList();

            var products = searchingProducts ?? await _products.TryGetByCategoryAsync(category);

            if(products.Count== 0)
            {
                return View(new List<MainPageProductsViewModel>());
            }

            var productsView = _mapping.Map<List<Product>, List<MainPageProductsViewModel>>(products, opts =>
            {
                opts.AfterMap((src, dest) =>
                {
                    foreach (var product in dest)
                    {
                        product.PageNumber = (dest.IndexOf(product) + pageProductsCount) / pageProductsCount;
                    }
                });
            }).Where(x => x.PageNumber == pageNumber).ToList();

            var user = User.Identity.IsAuthenticated ? await _userManager.FindByNameAsync(User.Identity.Name) : null;

            var favoriteProducts = user != null ? await _favorites.GetByUserIdAsync(user.Id) : null;

            foreach (var product in productsView)
            {
                product.Rating = await _feedbackApiClient.GetProductRetingAsync(product.Id);

                if (favoriteProducts != null)
                {
                    if (favoriteProducts.Products.Any(x => x.Id == product.Id))
                    {
                        product.IsInFavorites = true;
                    }
                }
            }

            ViewBag.PagesCount = products.Count() / pageProductsCount + 1;

            return View(productsView);
        }
        public async Task<IActionResult> BrandProducts(ProductBrands brand)
        {
            var products = await _products.TryGetByBrandAsync(brand);

            TempData["searchingProductsId"] = products.Count() == 0 ? new int[1] { -1 } : products.Select(x => x.Id).ToArray();

            return RedirectToAction(nameof(CategoryProducts));
        }

        public async Task<IActionResult> SaleProducts()
        {
            var products = await _discounts.GetProductsWithDiscountsAsync();

            TempData["searchingProductsId"] = products.Count() == 0 ? new int[1] { -1} : products.Select(x => x.Id).ToArray();

            return RedirectToAction(nameof(CategoryProducts));
        }

        [HttpPost]
        public async Task<IActionResult> SearchProducts(string searchingText)
        {
            if (searchingText.ToLower() == "акции")
            {
                return RedirectToAction(nameof(SaleProducts));
            }

            var products = await _products.GetAllAsync();

            var nameSortingProducts = products.Where(x => x.Name.ToLower().Contains(searchingText.ToLower())).ToList();
            var brandSortingProducts = products.Where(x => @EnumHelper.GetDisplayName(x.Brand).ToLower().Contains(searchingText.ToLower())).ToList();
            var categorySortingProducts = products.Where(x => @EnumHelper.GetDisplayName(x.Category).ToLower().Contains(searchingText.ToLower())).ToList();

            var sortingProducts = new List<Product>();
            sortingProducts.AddRange(brandSortingProducts);
            sortingProducts.AddRange(nameSortingProducts);
            sortingProducts.AddRange(categorySortingProducts);

            TempData["searchingProductsId"] = sortingProducts.Count() == 0 ? new int[1] { -1 } : sortingProducts.Distinct().Select(x => x.Id).ToArray();

            return RedirectToAction(nameof(CategoryProducts));
        }

    }
}
