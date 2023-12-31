﻿
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Interfaces;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Areas.Admin.Controllers
{
    [Area(Constants.AdminRoleName)]
    [Authorize(Roles = $"{Constants.AdminRoleName}, {Constants.ModeratorRoleName}")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapping;
        private readonly EmailService _emailService;
        private readonly SignInManager<User> _signInManager;
        public UserController(UserManager<User> userManager, RoleManager<Role> roleManager, IMapper mapping, EmailService emailService, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapping = mapping;
            _emailService = emailService;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var usersView = _mapping.Map<List<UserViewModel>>(users);
            return View(usersView);
        }
        public async Task<IActionResult> UserInfo(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var userView = _mapping.Map<UserViewModel>(user);
            ViewBag.UserRoles = await _userManager.GetRolesAsync(user);
            return View(userView);
        }
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Users");

        }
        public IActionResult ChangeUserPassword(string id)
        {
            var model = new ChangePasswordViewModel { Id = id };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ChangeUserPasswordAsync(ChangePasswordViewModel passwordModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(passwordModel.Id);
                var newPassword = _userManager.PasswordHasher.HashPassword(user, passwordModel.NewPassword);
                user.PasswordHash = newPassword;
                await _userManager.UpdateAsync(user);
                return Redirect($"UserInfo?id={passwordModel.Id}");
            }
            return View(passwordModel);
        }
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<ActionResult> CreateAsync(UserViewModel user)
        {
            if (ModelState.IsValid)
            {
                var userDb = _mapping.Map<User>(user);
                var result = await _userManager.CreateAsync(userDb, user.Password);
                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(userDb);
                    var callbackUrl = Url.Action(
                        nameof(ConfirmEmail),
                        "User",
                        values: new { userId = userDb.Id, code = code },
                        protocol: HttpContext.Request.Scheme);

                    await _emailService.SendEmailConfirmAsync(userDb.Email, callbackUrl);
                    
                    await _userManager.AddToRoleAsync(userDb, Constants.UserRoleName);
                    
                    return RedirectToAction(nameof(Users));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(user);
        }
        public async Task<IActionResult> EditUserInfo(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var userView = _mapping.Map<EditUserViewModel>(user);
            return View(userView);
        }
        [HttpPost]
        public async Task<ActionResult> EditUserInfoAsync(EditUserViewModel user)
        {
            if (ModelState.IsValid)
            {
                var userDb = await _userManager.FindByIdAsync(user.Id);

                if (userDb == null)
                {
                    ModelState.AddModelError(string.Empty, "Пользователь, которого вы пытаетесь отредактирвать, был удален другим администратором! Вернитесь на страницу продуктов!");
                    return View(user);
                }

                userDb.NikName = user.NikName;
                userDb.RealName = user.Name;
                userDb.SerName = user.SerName;
                userDb.Email = user.Email;
                userDb.PhoneNumber = user.Phone;
                userDb.ConcurrencyStamp = user.ConcurrencyStamp;
                try
                {
                    var result = await _userManager.UpdateAsync(userDb);
                    if (result.Succeeded)
                    {
                        return Redirect($"UserInfo?id={user.Id}");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                catch(DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    var clientValues = (User)entry.Entity;
                    var databaseEntry = entry.GetDatabaseValues();

                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty, "Пользователь, которого вы пытаетесь отредактирвать, был удален другим администратором! Вернитесь на страницу продуктов!");
                        return View(user);
                    }
                    else
                    {
                        var databaseValues = (User)entry.GetDatabaseValues().ToObject();
                        if (databaseValues.NikName != clientValues.NikName)
                        {
                            ModelState.AddModelError("NikName", "Текущее значение: " + databaseValues.NikName);
                        }
                        if (databaseValues.RealName != clientValues.RealName)
                        {
                            ModelState.AddModelError("RealName", "Текущее значение: " + databaseValues.RealName);
                        }
                        if (databaseValues.SerName != clientValues.SerName)
                        {
                            ModelState.AddModelError("SerName", "Текущее значение: " + databaseValues.SerName);
                        }
                        if (databaseValues.UserName != clientValues.UserName)
                        {
                            ModelState.AddModelError("UserName", "Текущее значение: " + databaseValues.UserName);
                        }                        
                        if (databaseValues.Email != clientValues.Email)
                        {
                            ModelState.AddModelError("Email", "Текущее значение: " + databaseValues.Email);
                        }
                        if (databaseValues.PhoneNumber != clientValues.PhoneNumber)
                        {
                            ModelState.AddModelError("PhoneNumber", "Текущее значение: " + databaseValues.PhoneNumber);
                        }

                        ModelState.AddModelError(string.Empty, "Запись, которую вы пытались отредактировать, была изменена другим пользователем после того, как вы получили исходное значение. Операция редактирования была отменена, и были отображены текущие значения в базе данных. Если вы все еще хотите отредактировать эту запись, нажмите кнопку Изменить еще раз или вернитесь на исходную страницу");
                        user.ConcurrencyStamp = databaseValues.ConcurrencyStamp;
                        ModelState.Remove("ConcurrencyStamp");

                    }
                }                
            }
            return View(user);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
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
                await _signInManager.SignInAsync(user, false);
                await _signInManager.PasswordSignInAsync(user.UserName, user.PasswordHash, true, false);
                await _emailService.SendRegistrationConfirmMessageAsync(user.Email, null);
                return Redirect("/Home/Index");
            }
            else
            {
                return View("EmailConfirmError");
            }
        }

    }
}
