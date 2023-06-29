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

namespace OnlineShopWebApp.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IBasketStorage _baskets;
        private readonly IPurchases _closedPurchases;
        private readonly UserManager<User> _userManager;

        public OrderController(IBasketStorage baskets, IPurchases closedPurchases, UserManager<User> userManager)
        {
            _baskets = baskets;
            _closedPurchases = closedPurchases;
            _userManager = userManager;
        }

        public IActionResult Confirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> OrderingAsync(OrderViewModel order)
        {
            var userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
            var basketItems = (await _baskets.TryGetExistingByUserIdAsync(userId)).Items;
            var orderDb = Mapping.ConvertToOrderDb(order, basketItems);
            await _closedPurchases.SaveAsync(orderDb);
            await _baskets.CloseAsync(userId);
            return Redirect("Confirmation");
        }
    }
}
