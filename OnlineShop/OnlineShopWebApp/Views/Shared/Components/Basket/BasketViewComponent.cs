using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Interfaces;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Models;
using ReturnTrue.AspNetCore.Identity.Anonymous;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Views.Shared.Components.Cart
{
    public class BasketViewComponent : ViewComponent
    {
        private readonly IBasketStorage _baskets;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapping;
        public BasketViewComponent(IBasketStorage baskets, UserManager<User> userManager, IMapper mapping)
        {
            _baskets = baskets;
            _userManager = userManager;
            _mapping = mapping;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = await GetUserId();

            var basketView = _mapping.Map<BasketViewModel>(await _baskets.TryGetExistingByUserIdAsync(userId));
            
            decimal productCounts = basketView?.Amount ?? 0;

            return View("Basket", productCounts);
        }

        public async Task<string> GetUserId()
        {
            var anonymousId = HttpContext.Features.Get<IAnonymousIdFeature>().AnonymousId;
            return User.Identity.IsAuthenticated ? (await _userManager.FindByNameAsync(User.Identity.Name)).Id : anonymousId;
        }
    }
}
