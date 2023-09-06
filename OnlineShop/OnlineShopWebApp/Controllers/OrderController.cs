using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.DB.Interfaces;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Interfaces;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineShopWebApp.Services;
using AutoMapper;
using System;
using Org.BouncyCastle.Bcpg;
using System.Linq;
using OnlineShop.DB.Patterns;

namespace OnlineShopWebApp.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly EmailService _emailService;
        private readonly IMapper _mapping;

        public OrderController(UserManager<User> userManager, EmailService emailService, IMapper mapping, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _emailService = emailService;
            _mapping = mapping;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Confirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> OrderingAsync(OrderViewModel order)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var basketItems = (await _unitOfWork.BasketDbStorage.TryGetExistingByUserIdAsync(user.Id)).Items;

            var orderDb = _mapping.Map<OrderViewModel, Order>(order, opt =>
            {
                opt.AfterMap((src, dest) => dest.Items = basketItems);
                opt.BeforeMap((src, dest) => src.Id = Guid.NewGuid());
                opt.BeforeMap((src, dest) => src.OrderDateTime = DateTime.UtcNow.ToString());
            });

            var basketItemsView = _mapping.Map<List<BasketItemViewModel>>(basketItems);
            foreach(var item in basketItemsView)
            {
                item.Product.Flavor = item.Product.Flavors.First(x => x.Id == item.ProductInfo.FlavorId);
            }

            await _emailService.SendOrderConfirmEmailAsync(user.Email, basketItemsView);

            await _unitOfWork.ProductsDbStorage.ReduceAmountInStock(orderDb.Items);
            await _unitOfWork.ClosedPurchasesDbStorage.SaveAsync(orderDb);
            await _unitOfWork.BasketDbStorage.CloseAsync(user.Id);
            await _unitOfWork.SaveChangesAsync();

            return Redirect(nameof(Confirmation));
        }
    }
}
