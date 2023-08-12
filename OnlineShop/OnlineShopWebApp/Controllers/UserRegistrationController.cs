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
using OnlineShopWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace OnlineShopWebApp.Controllers
{
    public class UserRegistrationController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly EmailService _emailService;
        private readonly IMapper _mapping;
        public UserRegistrationController(UserManager<User> userManager, SignInManager<User> signInManager, EmailService emailService, IMapper mapping)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _mapping = mapping;
        }
        public IActionResult Register(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> RegistrationConfirmAsync(UserViewModel user, string returnUrl)
        {
            var userDb = _mapping.Map<User>(user);
            if (user.Name == user.Password)
            {
                ModelState.AddModelError("", "Имя и пароль не должны совпадать");
            }
            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(userDb, user.Password);
                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(userDb);
                    var callbackUrl = Url.Action(
                        nameof(ConfirmEmail),
                        "UserRegistration",
                        values: returnUrl == null ? new { userId = userDb.Id, code = code } : new { userId = userDb.Id, code = code, returnUrl = returnUrl },
                        protocol: HttpContext.Request.Scheme);

                    await _emailService.SendEmailConfirmAsync(userDb.Email, callbackUrl);

                    await _userManager.AddToRoleAsync(userDb, Constants.UserRoleName);

                    await _signInManager.SignInAsync(userDb, false);
                    await _signInManager.PasswordSignInAsync(userDb.UserName, userDb.PasswordHash, true, false);

                    return View("EmailConfirm",returnUrl);
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code, string returnUrl)
        {
            if (userId == null || code == null)
            {
                return View("EmailConfirmError");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("EmailConfirmError");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return returnUrl == null ? RedirectToAction("Index", "Home") : Redirect(returnUrl);
            }  
            else
            {
                return View("EmailConfirmError");
            }
        }
    }
}
