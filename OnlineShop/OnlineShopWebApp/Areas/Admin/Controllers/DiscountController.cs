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
using OnlineShop.DB.Patterns;

namespace OnlineShopWebApp.Areas.Admin.Controllers
{
    [Area(Constants.AdminRoleName)]
    [Authorize(Roles = $"{Constants.AdminRoleName}, {Constants.ModeratorRoleName}")]
    public class DiscountController : Controller
    {        
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapping;
        private readonly IUnitOfWork _unitOfWork;
        public DiscountController(UserManager<User> userManager, IMapper mapping, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _mapping = mapping;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> NoDiscountProducts()
        {
            var products = _mapping.Map<List<ProductViewModel>>((await _unitOfWork.DiscountsDbStorage.GetProductsWithOutDiscountAsync()));
            return View(products);
        }

        public async Task<IActionResult> ProductDiscountInfo(int productId, int discountId)
        {
            var product = _mapping.Map<ProductViewModel>((await _unitOfWork.ProductsDbStorage.TryGetByIdAsync(productId)));
            ViewBag.Product = product;
            var discount = _mapping.Map<DiscountViewModel>((await _unitOfWork.DiscountsDbStorage.TryGetByIdAsync(discountId)));
            return View(discount);
        }

        public async Task<IActionResult> SortedDiscounts(int discountId)
        {
            ViewBag.Discount = _mapping.Map<DiscountViewModel>((await _unitOfWork.DiscountsDbStorage.TryGetByIdAsync(discountId)));
            var discounts = _mapping.Map<List<DiscountViewModel>>((await _unitOfWork.DiscountsDbStorage.GetAllAsync()));
            return View(discounts);
        }
        public async Task<IActionResult> DiscountProducts()
        {
            var discounts = await _unitOfWork.DiscountsDbStorage.GetAllAsync();
            var discountsView = _mapping.Map<List<DiscountViewModel>>(discounts);
            return View(discountsView);
        }

        public async Task<IActionResult> RemoveProductDiscount(int prodId, int discountId)
        {
            var product = await _unitOfWork.ProductsDbStorage.TryGetByIdAsync(prodId);
            await _unitOfWork.DiscountsDbStorage.RemoveDiscountAsync(product, discountId);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction("DiscountProducts");
        }

        public async Task<IActionResult> MakeDiscount(int prodId)
        {
            var product = _mapping.Map<ProductViewModel>((await _unitOfWork.ProductsDbStorage.TryGetByIdAsync(prodId)));
            ViewBag.Product = product;
            var discounts = _mapping.Map<List<DiscountViewModel>>((await _unitOfWork.DiscountsDbStorage.GetAllAsync()));
            return View(discounts);
        }

        [HttpPost]
        public async Task<ActionResult> MakeDiscountAsync(int productId, string discountDescription, int discountId)
        {
            if (ModelState.IsValid)
            {
                var discount = await _unitOfWork.DiscountsDbStorage.TryGetByIdAsync(discountId);
                var product = await _unitOfWork.ProductsDbStorage.TryGetByIdAsync(productId);
                await _unitOfWork.DiscountsDbStorage.AddAsync(product, discount, discountDescription);
                await _unitOfWork.SaveChangesAsync();
            }
            return Redirect($"ProductDiscountInfo?productId={productId}&discountId={discountId}");
        }

        public async Task<IActionResult> EditProductDiscount(int productId, int discountId)
        {
            var product = await _unitOfWork.ProductsDbStorage.TryGetByIdAsync(productId);
            var discount = await _unitOfWork.DiscountsDbStorage.TryGetByIdAsync(discountId);
            var discounts = await _unitOfWork.DiscountsDbStorage.GetAllAsync();
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
                var product = await _unitOfWork.ProductsDbStorage.TryGetByIdAsync(productId);
                await _unitOfWork.DiscountsDbStorage.ChangeDiscountAsync(product, oldDiscountId, newDiscountId, discountDescription);
                await _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ProductDiscountInfo), new {productId=productId, discountId=newDiscountId});
        }
    }
}
