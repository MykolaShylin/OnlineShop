using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Interfaces;
using OnlineShop.DB.Patterns;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Views.Shared.Components.Favorites
{
    public class FavoritesViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public FavoritesViewComponent(UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {            
            if(User.Identity.IsAuthenticated)
            {
                var userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
                var favoriteCount = (await _unitOfWork.FavoriteProductsDbStorage.GetByUserIdAsync(userId))?.Products.Count ?? 0;
                return View("Favorites", favoriteCount);
            }
            return View("Favorites", 0);

        }
    }
}
