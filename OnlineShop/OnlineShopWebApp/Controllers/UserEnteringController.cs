using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Interfaces;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Constants = OnlineShop.DB.Models.Constants;
using System;

namespace OnlineShopWebApp.Controllers
{
    public class UserEnteringController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public UserEnteringController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<ActionResult> LoginAsync(string returnUrl)
        {
            if(returnUrl == "/Basket/Buying" || returnUrl == "/Product/Comparing")
            {
                returnUrl = $"/Product/CategoryProducts?isAllListProducts={true}";
            }
            var logInViewModel = new LogInViewModel()
            { 
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(logInViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> LoginAsync(LogInViewModel login)
        {

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(login.NameLogin, login.Password, login.IsRemember, false);
                if (result.Succeeded)
                {
                    return Redirect(login.ReturnUrl == null ? "/Home/Index" : login.ReturnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Пароль или логин не верный");
                }
            }
            login.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return View(login);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }


        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "UserEntering", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            var loginViewModel = new LogInViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login", loginViewModel);
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information.");
                return View("Login", loginViewModel);
            }
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, false);

            if (signInResult.Succeeded)
            {                
                return LocalRedirect(returnUrl);
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                if (email != null)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    var defaultPassword = string.Empty;
                    var isNewUser = false;
                    if (user == null)
                    {
                        user = new User
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                            RealName = info.Principal.FindFirstValue(ClaimTypes.Name),
                            PhoneNumber = info.Principal.FindFirstValue(ClaimTypes.MobilePhone)
                        };
                        defaultPassword = Guid.NewGuid().ToString().Substring(0,10);
                        isNewUser = true;
                        await _userManager.CreateAsync(user, defaultPassword);
                        await _userManager.AddToRoleAsync(user, Constants.UserRoleName);
                    }
                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, false);

                    if(isNewUser)
                    {
                        return Redirect($"/Home/Index?defaultPassword={defaultPassword}");
                    }
                    return LocalRedirect(returnUrl);
                }

                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support on bullbody.support@gmail.com";

                return View("Error");
            }
        }        
    }

}
