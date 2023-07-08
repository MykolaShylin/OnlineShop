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
using OnlineShopWebApp.FeedbackApi;
using AutoMapper;

namespace OnlineShopWebApp.Areas.Admin.Controllers
{
    [Area(Constants.AdminRoleName)]
    [Authorize(Roles = Constants.AdminRoleName)]
    public class ProductController : Controller
    {
        private IProductsStorage _productsInStock;
        private IFlavor _flavors;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly IPictures _pictures;
        private readonly FeedbackApiClient _feedbackApiClient;
        public ProductController(IWebHostEnvironment appEnvironment, IFlavor flavors, IProductsStorage productsInStock, IPictures pictures, FeedbackApiClient feedbackApiClient)
        {
            _productsInStock = productsInStock;
            _flavors = flavors;
            _appEnvironment = appEnvironment;
            _pictures = pictures;
            _feedbackApiClient = feedbackApiClient;
        }

        public async Task<IActionResult> ProductsInStock()
        {
            var existingProducts = Mapping.ConvertToProductsView(await _productsInStock.GetAllAsync());
            var existingFlavors = (await _flavors.GetAllAsync()).MappToViewModelParameters<Flavor, FlavorViewModel>();
            ViewBag.Flavors = existingFlavors;
            return View(existingProducts);
        }        

        [HttpPost]
        public async Task<ActionResult> AddAsync(ProductViewModel product, List<string> flavors)
        {
            if ((await _productsInStock.GetAllAsync()).Any(prod => prod.Name.ToLower() == product.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Продукт с таким названием уже существует");
            }
            if (product.Name == product.Brand)
            {
                ModelState.AddModelError("Name", "Название и бренд продукта не должны совпадать");
            }
            if (ModelState.IsValid)
            {
                var productDb = await CreateNewProductWithFilesAsync(product, flavors);
                await _productsInStock.SaveAsync(productDb);
                return Redirect("ProductsInStock");
            }
            return Redirect("ProductsInStock");
        }

        public async Task<ActionResult> EditProduct(int productId)
        {
            //var feedbacks = await _feedbackApiClient.GetFeedbacksAsync(productId);
            //productView.Feedbacks = Mapping.ConvertToFeedbacksView(feedbacks);
            var productView = Mapping.ConvertToProductView(await _productsInStock.TryGetByIdAsync(productId));
            var existingFlavors = (await _flavors.GetAllAsync()).MappToViewModelParameters<Flavor, FlavorViewModel>();
            ViewBag.Flavors = existingFlavors;
            return View(productView);
        }

        [HttpPost]
        public async Task<ActionResult> EditProductAsync(ProductViewModel product, List<string> flavors, List<string> pictures)
        {
            var productView = await GetProductForViewAsync(product, flavors, pictures);
            if (product.Name == product.Brand)
            {
                ModelState.AddModelError("Name", "Название и бренд продукта не должны совпадать");
            }
            if (ModelState.IsValid)
            {
                var productToUpdate = await _productsInStock.TryGetByIdAsync(product.Id);

                if (productToUpdate == null)
                {
                    ModelState.AddModelError(string.Empty, "Продукт, который вы пытаетесь отредактирвать, был удален другим пользователем! Вернитесь на страницу продуктов!");
                    ViewBag.Flavors = (await _flavors.GetAllAsync()).MappToViewModelParameters<Flavor, FlavorViewModel>();                    
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
                    await _productsInStock.EditAsync(productToUpdate);
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
                        ViewBag.Flavors = (await _flavors.GetAllAsync()).MappToViewModelParameters<Flavor, FlavorViewModel>();
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
                        if (databaseValues.Flavors.Count != flavors.Count || databaseValues.Flavors.Select(x=>x.Name).Union(flavors).ToList().Count != flavors.Count)
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
            ViewBag.Flavors = (await _flavors.GetAllAsync()).MappToViewModelParameters<Flavor, FlavorViewModel>();
            return View(productView);
        }

        private async Task<ProductViewModel> GetProductForViewAsync(ProductViewModel product, List<string> flavors, List<string> pictures)
        {
            product.Pictures = new List<ProductPicture>();

            foreach (var path in pictures)
            {
                var newPath = new ProductPicture { Path = pictures.First() };
                product.Pictures.Add(newPath);
            }
            product.Flavors = new List<FlavorViewModel>();
            foreach (var flavor in flavors)
            {
                var flavorView = (await _flavors.TryGetByNameAsync(flavor)).MappToViewModelParameter<Flavor, FlavorViewModel>();
                product.Flavors.Add(flavorView);
            }
            return product;
        }
        private async Task<Product> EditProductWithExistingFilesAsync(ProductViewModel product, List<string> flavors, List<string> pictures)
        {
            var flavorsDb = new List<Flavor>();
            foreach (var flavor in flavors)
            {
                var flavorDb = await _flavors.TryGetByNameAsync(flavor);
                flavorsDb.Add(flavorDb);
            }
            var picturesDb = new List<ProductPicture>();
            foreach (var picture in pictures)
            {
                var pictureDb = await _pictures.TryGetByPathAsync(picture);
                picturesDb.Add(pictureDb);
            }
            var productDb = Mapping.ConvertToProductDb(product);
            productDb.Flavors = flavorsDb;
            productDb.Pictures = picturesDb;
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

        public async Task<IActionResult> Delete(int prodId)
        {
            var product = await _productsInStock.TryGetByIdAsync(prodId);
            await _productsInStock.DeleteAsync(product);
            return Redirect("ProductsInStock");
        }

        public async Task<IActionResult> ClearOutStock()
        {
            await _productsInStock.ClearAllAsync();
            return Redirect("ProductsInStock");
        }

        public async Task<IActionResult> GetDefaultProducts()
        {
            if ((await _productsInStock.GetAllAsync()).Count == 0)
            {
                await _productsInStock.InitializeDefaultProductsAsync();
            }
            return RedirectToAction("ProductsInStock");
        }

        private async Task<Product> CreateNewProductAsync(ProductViewModel product, List<string> flavors, List<string> fileName)
        {
            var flavorsDb = new List<Flavor>();
            foreach (var flavor in flavors)
            {
                var flavorDb = await _flavors.TryGetByNameAsync(flavor);
                flavorsDb.Add(flavorDb);
            }

            if (product.Id != 0)
            {
                var existingPictures = (await _productsInStock.TryGetByIdAsync(product.Id)).Pictures.Select(x => x.Path).ToList();
                foreach (var exist in existingPictures)
                {
                    System.IO.File.Delete(Path.Combine(_appEnvironment.WebRootPath + exist));
                    var picture = await _pictures.TryGetByPathAsync(exist);
                    await _pictures.DeleteAsync(picture);
                }
            }

            var imagesDb = new List<ProductPicture>();
            foreach (var name in fileName)
            {
                var pictureDb = new ProductPicture { Path = GetProductImagePath(product.Category) + name };
                await _pictures.SaveAsync(pictureDb);
                var newPicture = await _pictures.TryGetByPathAsync(pictureDb.Path);
                imagesDb.Add(newPicture);
            }

            return new Product
            {
                Id = product.Id,
                Category = product.Category,
                Name = product.Name,
                Brand = product.Brand,
                Cost = product.Cost,
                Flavors = flavorsDb,
                Description = product.Description,
                AmountInStock = product.AmountInStock,
                Pictures = imagesDb,
                DiscountCost = product.DiscountCost,
                DiscountDescription = product.DiscountDescription,
                Concurrency = product.Concurrency,
            };
        }
        private string GetProductImagePath(ProductCategories categori)
        {
            switch (categori)
            {
                case ProductCategories.Protein:
                    return "/prod_pictures/Protein/";
                case ProductCategories.ProteinBar:
                    return "/prod_pictures/Bars/";
                case ProductCategories.Aminoacid:
                    return "/prod_pictures/Aminoacid/";
                case ProductCategories.Creatine:
                    return "/prod_pictures/creatine/";
                case ProductCategories.BCAA:
                    return "/prod_pictures/BCAA/";
                case ProductCategories.Gainer:
                    return "/prod_pictures/geiner/";
                case ProductCategories.Citruline:
                    return "/prod_pictures/citruline/";
                case ProductCategories.FatBurner:
                    return "/prod_pictures/FatBurner/";
                default:
                    return string.Empty;
            }
        }
    }
}
