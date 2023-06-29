using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Interfaces;
using OnlineShopWebApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Views.Shared.Components.Comparer
{
    public class ComparerViewComponent : ViewComponent
    {
        private readonly IProductComparer _comparingProducts;
        private readonly UserManager<User> _userManager;
        public ComparerViewComponent(IProductComparer comparingProducts, UserManager<User> userManager)
        {
            _comparingProducts = comparingProducts;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<ComparingProducts> products = null;
            if (User.Identity.Name != null)
            {
                var userId = (await _userManager.FindByNameAsync(User.Identity.Name))?.Id;
                products = userId == null ? null : await _comparingProducts.GetAllAsync(userId);
            }
            int productCounts = products?.Count ?? 0;
            return View("Comparer", productCounts);

        }
    }
}
