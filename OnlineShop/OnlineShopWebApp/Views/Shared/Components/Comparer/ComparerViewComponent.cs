using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Interfaces;
using OnlineShop.DB.Patterns;
using OnlineShopWebApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Views.Shared.Components.Comparer
{
    public class ComparerViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        public ComparerViewComponent(UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<ComparingProducts> products = null;
            if (User.Identity.Name != null)
            {
                var userId = (await _userManager.FindByNameAsync(User.Identity.Name))?.Id;
                products = userId == null ? null : await _unitOfWork.ComparingProductsDbStorage.GetAllAsync(userId);
            }
            int productCounts = products?.Count ?? 0;
            return View("Comparer", productCounts);

        }
    }
}
