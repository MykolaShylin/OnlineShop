using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.DB.Interfaces;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Enumerations;
using OnlineShop.DB.Models.Interfaces;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace OnlineShopWebApp.Areas.Admin.Controllers
{
    [Area(Constants.AdminRoleName)]
    [Authorize(Roles = $"{Constants.AdminRoleName}, {Constants.ModeratorRoleName}")]
    public class DiscountController : Controller
    {
        private readonly IProductsStorage _products;
        private readonly IDiscount _discounts;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapping;
        public DiscountController(IDiscount discounts, IProductsStorage products, UserManager<User> userManager, IMapper mapping)
        {
            _products = products;
            _userManager = userManager;
            _discounts = discounts;
            _mapping = mapping;
        }

        public async Task<IActionResult> NoDiscountProducts()
        {
            var products = _mapping.Map<List<ProductViewModel>>((await _discounts.GetProductsWithOutDiscountAsync()));
            return View(products);
        }

        public async Task<IActionResult> ProductDiscountInfo(int productId, int discountId)
        {
            var product = _mapping.Map<ProductViewModel>((await _products.TryGetByIdAsync(productId)));
            ViewBag.Product = product;
            var discount = _mapping.Map<DiscountViewModel>((await _discounts.TryGetByIdAsync(discountId)));
            return View(discount);
        }

        public async Task<IActionResult> SortedDiscounts(int discountId)
        {
            ViewBag.Discount = _mapping.Map<DiscountViewModel>((await _discounts.TryGetByIdAsync(discountId)));
            var discounts = _mapping.Map<List<DiscountViewModel>>((await _discounts.GetAllAsync()));
            return View(discounts);
        }
        public async Task<IActionResult> DiscountProducts()
        {
            var discounts = await _discounts.GetAllAsync();
            var discountsView = _mapping.Map<List<DiscountViewModel>>(discounts);
            return View(discountsView);
        }

        public async Task<IActionResult> RemoveProductDiscount(int prodId, int discountId)
        {
            var product = await _products.TryGetByIdAsync(prodId);
            await _discounts.RemoveDiscountAsync(product, discountId);
            return RedirectToAction("DiscountProducts");
        }

        public async Task<IActionResult> MakeDiscount(int prodId)
        {
            var product = _mapping.Map<ProductViewModel>((await _products.TryGetByIdAsync(prodId)));
            ViewBag.Product = product;
            var discounts = _mapping.Map<List<DiscountViewModel>>((await _discounts.GetAllAsync()));
            return View(discounts);
        }

        [HttpPost]
        public async Task<ActionResult> MakeDiscountAsync(int productId, string discountDescription, int discountId)
        {
            if (ModelState.IsValid)
            {
                var discount = await _discounts.TryGetByIdAsync(discountId);
                var product = await _products.TryGetByIdAsync(productId);
                await _discounts.AddAsync(product, discount, discountDescription);
            }
            return Redirect($"ProductDiscountInfo?productId={productId}&discountId={discountId}");
        }

        public async Task<IActionResult> EditProductDiscount(int productId, int discountId)
        {
            var product = await _products.TryGetByIdAsync(productId);
            var discount = await _discounts.TryGetByIdAsync(discountId);
            var discounts = await _discounts.GetAllAsync();
            var editView = new EditProductDiscountViewModel
            {
                Product = _mapping.Map<ProductViewModel>(product),
                Discount = _mapping.Map<DiscountViewModel>(discount),
                Discounts = _mapping.Map<List<DiscountViewModel>>(discounts)
            };
            return View(editView);
        }

        [HttpPost]
        public async Task<ActionResult> EditProductDiscountAsync(int productId, string discountDescription, int oldDiscountId, int newDiscountId)
        {
            if (ModelState.IsValid)
            {
                var product = await _products.TryGetByIdAsync(productId);
                await _discounts.ChangeDiscountAsync(product, oldDiscountId, newDiscountId, discountDescription);
            }
            return RedirectToAction(nameof(ProductDiscountInfo), new {productId=productId, discountId=newDiscountId});
        }
    }
}
