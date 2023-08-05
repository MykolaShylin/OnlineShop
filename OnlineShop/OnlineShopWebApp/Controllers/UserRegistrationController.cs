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
using System.Runtime;
using System;
using Twilio.Rest.Verify.V2.Service;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace OnlineShopWebApp.Controllers
{
    public class UserRegistrationController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly EmailService _emailService;
        private readonly IMapper _mapping;
        private readonly TwilioVerifySettings _twilioSettings;
        public UserRegistrationController(UserManager<User> userManager, SignInManager<User> signInManager, EmailService emailService, IMapper mapping, TwilioVerifySettings settings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _mapping = mapping;
            _twilioSettings = settings;
        }
        public IActionResult Register(string returnUrl)
        {

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public IActionResult PhoneNumberVerification(string userId, string phoneNumber, string returnUrl)
        {
            var phoneNumberConfirmModel = new PhoneVerificationViewModel
            {
                UserId = userId,
                PhoneNumber = phoneNumber,
                ReturnUrl = returnUrl
            };
            return View(phoneNumberConfirmModel);
        }

        [HttpPost]
        public async Task<ActionResult> PhoneNumberVerification(PhoneVerificationViewModel phoneVerificationModel)
        {
            if(!ModelState.IsValid)
            {
                return View(new { userId = phoneVerificationModel.UserId, phoneNumber = phoneVerificationModel.PhoneNumber, returnUrl = phoneVerificationModel.ReturnUrl });
            }

            var user = await _userManager.FindByIdAsync(phoneVerificationModel.UserId);

            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneVerificationModel.PhoneNumber);

            var messageToSend = await MessageResource.CreateAsync(
                from: _twilioSettings.TwilioPhoneNumber,
                to: phoneVerificationModel.PhoneNumber,
                body: "Ваш код для верификации номера телефона на сайте BullBody: " + code);

            return RedirectToAction(nameof(PhoneNumberConfirmn), new { userId = phoneVerificationModel.UserId, phoneNumber = phoneVerificationModel.PhoneNumber, returnUrl = phoneVerificationModel.ReturnUrl });
        }

        public IActionResult PhoneNumberConfirmn(string userId, string phoneNumber, string returnUrl)
        {
            var phoneConfirm = new PhoneConfirmnViewModel
            {
                UserId = userId,
                PhoneNumber = phoneNumber,
                ReturnUrl = returnUrl
            };
            return View(phoneConfirm);
        }

        [HttpPost]
        public async Task<ActionResult> PhoneNumberConfirmn(PhoneConfirmnViewModel phoneConfirmModel)
        {
            var user = await _userManager.FindByIdAsync(phoneConfirmModel.UserId);
            var isVerified = await _userManager.VerifyChangePhoneNumberTokenAsync(user, phoneConfirmModel.ConfirmCode, phoneConfirmModel.PhoneNumber);

            if(isVerified)
            {
                var identityUser = await _userManager.FindByIdAsync(phoneConfirmModel.UserId);
                identityUser.PhoneNumberConfirmed = true;
                var updateResult = await _userManager.UpdateAsync(identityUser);

                if (updateResult.Succeeded)
                {
                    return RedirectToAction(nameof(EmailConfirm), new { userId = phoneConfirmModel.UserId, returnUrl = phoneConfirmModel.ReturnUrl });
                }
            }

            ModelState.AddModelError("", "Неверный код подтверждения либо ошибка на сервере, попробуйте позже");

            return View();
        }

        public async Task<IActionResult> EmailConfirm(string userId, string returnUrl)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action(
                    nameof(ConfirmEmail),
                    "UserRegistration",
                    values: returnUrl == null ? new { userId = user.Id, code = code } : new { userId = user.Id, code = code, returnUrl = returnUrl },
                    protocol: HttpContext.Request.Scheme);

                await _emailService.SendEmailConfirmAsync(user.Email, callbackUrl);

                await _signInManager.PasswordSignInAsync(user.UserName, user.PasswordHash, true, false);

                return View("EmailConfirm");
            }
            return View("EmailConfirmError");
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
                    return RedirectToAction(nameof(PhoneNumberVerification), new { userId = userDb.Id, phoneNumber = userDb.PhoneNumber, returnUrl = returnUrl });
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
