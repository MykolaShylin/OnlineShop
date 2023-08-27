using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Views.Shared.Components.Favorites
{
    public class FavoritesViewComponent : ViewComponent
    {
        private readonly IFavorite _favorites;
        private readonly UserManager<User> _userManager;

        public FavoritesViewComponent(IFavorite favorites, UserManager<User> userManager)
        {
            _favorites = favorites;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {            
            if(User.Identity.IsAuthenticated)
            {
                var userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
                var favoriteCount = (await _favorites.GetByUserIdAsync(userId))?.Products.Count ?? 0;
                return View("Favorites", favoriteCount);
            }
            return View("Favorites", 0);

        }
    }
}
