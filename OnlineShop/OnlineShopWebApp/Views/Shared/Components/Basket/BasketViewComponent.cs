using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Interfaces;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Views.Shared.Components.Cart
{
    public class BasketViewComponent : ViewComponent
    {
        private readonly IBasketStorage _baskets;
        private readonly UserManager<User> _userManager;
        public BasketViewComponent(IBasketStorage baskets, UserManager<User> userManager)
        {
            _baskets = baskets;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            BasketViewModel basketView = null;
            if(User.Identity.Name != null)
            {
                var userId = (await _userManager.FindByNameAsync(User.Identity.Name))?.Id;
                var basket = userId == null ? null : await _baskets.TryGetExistingByUserIdAsync(userId);                
                basketView = userId == null ? null : Mapping.ConvertToBasketView(basket);
            }
            decimal productCounts = basketView?.Amount ?? 0;
            return View("Basket", productCounts);
        }
    }
}
