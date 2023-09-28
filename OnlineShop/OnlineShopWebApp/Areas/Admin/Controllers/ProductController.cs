using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Enumerations;
using OnlineShop.DB.Models.Interfaces;
using OnlineShopWebApp.Controllers;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis;
using AutoMapper;
using OnlineShopWebApp.FeedbackApi.Models;
using System.Net.Http.Headers;
using OnlineShopWebApp.FeedbackApi;
using OnlineShop.DB.Patterns;

namespace OnlineShopWebApp.Areas.Admin.Controllers
{
    [Area(Constants.AdminRoleName)]
    [Authorize(Roles = Constants.AdminRoleName)]
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly FeedbackApiClient _feedbackApiClient;
        private readonly IMapper _mapping;
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IWebHostEnvironment appEnvironment, FeedbackApiClient feedbackApiClient, IMapper mapping, IUnitOfWork unitOfWork)
        {
            _appEnvironment = appEnvironment;
            _feedbackApiClient = feedbackApiClient;
            _mapping = mapping;
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> ProductsInStock(ProductCategories category = ProductCategories.None)
        {
            var existingProducts = _mapping.Map<List<ProductViewModel>>((await _unitOfWork.ProxyProductsDbStorage.GetAllAsync()));
            var existingFlavors = _mapping.Map<List<FlavorViewModel>>(await _unitOfWork.FlavorsDbStorage.GetAllAsync());
            if (category != ProductCategories.None)
            {
                existingProducts = _mapping.Map<List<ProductViewModel>>(await _unitOfWork.ProxyProductsDbStorage.TryGetByCategoryAsync(category));
            }
            ViewBag.Flavors = existingFlavors;
            ViewBag.Category = category;
            return View(existingProducts);
        }

        [HttpPost]
        public async Task<ActionResult> AddAsync(ProductViewModel product, List<string> flavors)
        {
            if ((await _unitOfWork.ProxyProductsDbStorage.GetAllAsync()).Any(prod => prod.Name.ToLower() == product.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Продукт с таким названием уже существует");
            }
            if (product.Name == @EnumHelper.GetDisplayName(product.Brand))
            {
                ModelState.AddModelError("Name", "Название и бренд продукта не должны совпадать");
            }
            if (ModelState.IsValid)
            {
                var productDb = await CreateNewProductWithFilesAsync(product, flavors);

                await _unitOfWork.ProxyProductsDbStorage.SaveAsync(productDb);

                await _unitOfWork.DiscountsDbStorage.AddAsync(await _unitOfWork.ProxyProductsDbStorage.TryGetByNameAsync(productDb.Name), await _unitOfWork.DiscountsDbStorage.GetZeroDiscountAsync(), "Без скидки");

                await _unitOfWork.SaveChangesAsync();

                return Redirect("ProductsInStock");
            }
            return Redirect("ProductsInStock");
        }

        public async Task<ActionResult> EditProduct(int productId)
        {
            var productView = _mapping.Map<ProductViewModel>(await _unitOfWork.ProxyProductsDbStorage.TryGetByIdAsync(productId));
            var existingFlavors = _mapping.Map<List<FlavorViewModel>>(await _unitOfWork.FlavorsDbStorage.GetAllAsync());
            ViewBag.Flavors = existingFlavors;
            return View(productView);
        }

        [HttpPost]
        public async Task<IActionResult> EditProductAsync(ProductViewModel product, List<string> flavors, List<string> pictures)
        {
            var productView = await GetProductForViewAsync(product, flavors, pictures);
            ViewBag.Flavors = _mapping.Map<List<FlavorViewModel>>(await _unitOfWork.FlavorsDbStorage.GetAllAsync());

            if (product.Name == @EnumHelper.GetDisplayName(product.Brand))
            {
                ModelState.AddModelError("Name", "Название и бренд продукта не должны совпадать");
            }
            if (ModelState.IsValid)
            {
                var productToUpdate = await _unitOfWork.ProxyProductsDbStorage.TryGetByIdAsync(product.Id);

                if (productToUpdate == null)
                {
                    ModelState.AddModelError(string.Empty, "Продукт, который вы пытаетесь отредактирвать, был удален другим пользователем! Вернитесь на страницу продуктов!");
                    return View(productView);
                }
                if (product.UploadedFile != null)
                {
                    productToUpdate = await CreateNewProductWithFilesAsync(product, flavors);
                }
                else
                {
                    productToUpdate = await EditProductWithExistingFilesAsync(product, flavors, pictures);
                }

                try
                {
                    await _unitOfWork.ProxyProductsDbStorage.EditAsync(productToUpdate);
                    await _unitOfWork.SaveChangesAsync();
                    return RedirectToAction("ProductsInStock");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    var clientValues = (Product)entry.Entity;
                    var databaseEntry = entry.GetDatabaseValues();

                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty, "Продукт, который вы пытаетесь отредактирвать, был удален другим пользователем! Вернитесь на страницу продуктов!");
                        return View(productView);
                    }
                    else
                    {
                        var databaseValues = (Product)entry.GetDatabaseValues().ToObject();
                        if (databaseValues.Category != clientValues.Category)
                        {
                            ModelState.AddModelError("Сategory", "Текущее значение: " + EnumHelper.GetDisplayName(databaseValues.Category));
                        }
                        if (databaseValues.Brand != clientValues.Brand)
                        {
                            ModelState.AddModelError("Brand", "Текущее значение: " + databaseValues.Brand);
                        }
                        if (databaseValues.Name != clientValues.Name)
                        {
                            ModelState.AddModelError("Name", "Текущее значение: " + databaseValues.Name);
                        }
                        if (databaseValues.Cost != clientValues.Cost)
                        {
                            ModelState.AddModelError("Cost", "Текущее значение: " + databaseValues.Cost);
                        }
                        if (databaseValues.Description != clientValues.Description)
                        {
                            ModelState.AddModelError("Description", "Текущее значение: " + databaseValues.Description);
                        }
                        if (databaseValues.AmountInStock != clientValues.AmountInStock)
                        {
                            ModelState.AddModelError("AmountInStock", "Текущее значение: " + databaseValues.AmountInStock);
                        }
                        if (databaseValues.Flavors.Count != flavors.Count || databaseValues.Flavors.Select(x => x.Name).Union(flavors).ToList().Count != flavors.Count)
                        {
                            var errorMesage = string.Empty;
                            foreach (var flavor in databaseValues.Flavors)
                            {
                                errorMesage += flavor.Name + " | ";
                            }
                            ModelState.AddModelError("Flavors", "Текущее значение: " + errorMesage);
                        }

                        ModelState.AddModelError(string.Empty, "Запись, которую вы пытались отредактировать, была изменена другим пользователем после того, как вы получили исходное значение. Операция редактирования была отменена, и были отображены текущие значения в базе данных. Если вы все еще хотите отредактировать эту запись, нажмите кнопку Изменить еще раз или вернитесь на исходную страницу");
                        productView.Concurrency = (byte[])databaseValues.Concurrency;
                        ModelState.Remove("Timestamp");
                    }
                }
            }
            return View(productView);
        }

        private async Task<ProductViewModel> GetProductForViewAsync(ProductViewModel product, List<string> flavors, List<string> pictures)
        {
            product.Pictures = new List<ProductPicture>();
            foreach (var path in pictures)
            {
                var pictureView = await _unitOfWork.PicturesDbStorage.TryGetByPathAsync(path);
                product.Pictures.Add(pictureView);
            }

            product.Flavors = new List<FlavorViewModel>();
            foreach (var flavor in flavors)
            {
                var flavorView = _mapping.Map<FlavorViewModel>(await _unitOfWork.FlavorsDbStorage.TryGetByNameAsync(flavor));
                product.Flavors.Add(flavorView);
            }
            return product;
        }
        private async Task<Product> EditProductWithExistingFilesAsync(ProductViewModel productView, List<string> flavors, List<string> pictures)
        {
            var flavorsDb = new List<Flavor>();
            foreach (var flavor in flavors)
            {
                var flavorDb = await _unitOfWork.FlavorsDbStorage.TryGetByNameAsync(flavor);
                flavorsDb.Add(flavorDb);
            }
            var picturesDb = new List<ProductPicture>();
            foreach (var picture in pictures)
            {
                var pictureDb = await _unitOfWork.PicturesDbStorage.TryGetByPathAsync(picture);
                picturesDb.Add(pictureDb);
            }

            var productDb = _mapping.Map<Product>(productView);

            var discount = await _unitOfWork.DiscountsDbStorage.GetByProductIdAsync(productDb.Id);

            productDb.Flavors = flavorsDb;
            productDb.Pictures = picturesDb;
            productDb.DiscountCost = _unitOfWork.DiscountsDbStorage.CalculateDiscount(productDb.Cost, discount.DiscountPercent);
            return productDb;
        }

        private async Task<Product> CreateNewProductWithFilesAsync(ProductViewModel product, List<string> flavors)
        {
            string productImagePath = Path.Combine(_appEnvironment.WebRootPath + GetProductImagePath(product.Category));
            var picturesNames = new List<string>();
            foreach (var file in product.UploadedFile)
            {
                var fileName = Guid.NewGuid() + "_" + file.FileName;
                picturesNames.Add(fileName);

                using (var fileStream = new FileStream(productImagePath + fileName, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            return await CreateNewProductAsync(product, flavors, picturesNames);
        }

        public async Task<IActionResult> Delete(int productId)
        {
            var product = await _unitOfWork.ProxyProductsDbStorage.TryGetByIdAsync(productId);
            _unitOfWork.ProxyProductsDbStorage.Delete(product);
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(ProductsInStock));
        }

        public async Task<IActionResult> ClearOutStock()
        {
            await _unitOfWork.ProxyProductsDbStorage.ClearAllAsync();
            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(ProductsInStock));
        }

        public async Task<IActionResult> GetDefaultProducts()
        {
            if ((await _unitOfWork.ProxyProductsDbStorage.GetAllAsync()).Count == 0)
            {
                await _unitOfWork.ProxyProductsDbStorage.InitializeDefaultProductsAsync();
                await _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ProductsInStock));
        }

        private async Task<Product> CreateNewProductAsync(ProductViewModel product, List<string> flavors, List<string> fileName)
        {
            var flavorsDb = new List<Flavor>();
            foreach (var flavor in flavors)
            {
                var flavorDb = await _unitOfWork.FlavorsDbStorage.TryGetByNameAsync(flavor);
                flavorsDb.Add(flavorDb);
            }

            if (product.Id != 0)
            {
                var existingPictures = (await _unitOfWork.ProxyProductsDbStorage.TryGetByIdAsync(product.Id)).Pictures.Select(x => x.Path).ToList();
                foreach (var exist in existingPictures)
                {
                    System.IO.File.Delete(Path.Combine(_appEnvironment.WebRootPath + exist));
                    var picture = await _unitOfWork.PicturesDbStorage.TryGetByPathAsync(exist);
                    await _unitOfWork.PicturesDbStorage.DeleteAsync(picture);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            var imagesDb = new List<ProductPicture>();
            foreach (var name in fileName)
            {
                var pictureDb = new ProductPicture { Path = GetProductImagePath(product.Category) + name, NutritionPath = string.Empty };
                await _unitOfWork.PicturesDbStorage.SaveAsync(pictureDb);
                await _unitOfWork.SaveChangesAsync();
                var newPicture = await _unitOfWork.PicturesDbStorage.TryGetByPathAsync(pictureDb.Path);
                imagesDb.Add(newPicture);
            }

            return new Product
            {
                Category = product.Category,
                Name = product.Name,
                Brand = product.Brand,
                Cost = product.Cost,
                Flavors = flavorsDb,
                Description = product.Description,
                AmountInStock = product.AmountInStock,
                Pictures = imagesDb,
            };

        }
        private string GetProductImagePath(ProductCategories category)
        {
            return $"/prod_pictures/{category}/";
        }
    }
}
