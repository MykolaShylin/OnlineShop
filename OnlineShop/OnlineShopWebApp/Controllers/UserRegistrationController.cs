using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Interfaces;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Models;
using Serilog;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OnlineShopWebApp.Controllers
{
    public class UserRegistrationController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserRegistrationController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Register(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> RegistrationConfirmAsync(UserViewModel user, string returnUrl)
        {
            var userDb = Mapping.ConvertToUserDb(user);
            if (user.Name == user.Password)
            {
                ModelState.AddModelError("", "Имя и пароль не должны совпадать");
            }
            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(userDb, user.Password);
                if (result.Succeeded)
                {
                    if(returnUrl == null)
                    {
                        return Redirect("/UserEntering/Login");
                    }
                    await _signInManager.SignInAsync(userDb, false);
                    await _signInManager.PasswordSignInAsync(userDb.UserName, userDb.PasswordHash, true, false);
                    return Redirect(returnUrl);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View("Register");

        }
    }
}
