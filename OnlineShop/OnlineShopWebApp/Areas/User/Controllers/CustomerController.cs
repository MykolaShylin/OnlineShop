
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.DB.Interfaces;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Interfaces;
using OnlineShop.DB.Patterns;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Areas.Admin.Controllers
{
    [Area(Constants.UserRoleName)]
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly UserManager<User> _userManager;        
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly IMapper _mapping;
        private readonly IUnitOfWork _unitOfWork;
        public CustomerController(UserManager<User> userManager, IWebHostEnvironment appEnvironment, IMapper mapping, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _appEnvironment = appEnvironment;
            _mapping = mapping;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> ClosedOrders()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var closedOrders = _mapping.Map<List<OrderViewModel>>((await _unitOfWork.ClosedPurchasesDbStorage.TryGetByUserIdAsync(user.Id)));
            return View(closedOrders);
        }

        public async Task<IActionResult> OrderDataAsync(Guid id)
        {
            var order = await _unitOfWork.ClosedPurchasesDbStorage.TryGetByIdAsync(id);
            var orderView = _mapping.Map<OrderViewModel>(order);
            return View(orderView);
        }
        public async Task<IActionResult> PersonalDataAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var userView = _mapping.Map<UserViewModel>(user);
            return View(userView);
        }

        public async Task<IActionResult> EditPersonalDataAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var userView = _mapping.Map<EditUserViewModel>(user);
            return View(userView);
        }

        [HttpPost]
        public async Task<ActionResult> EditPersonalDataAsync(EditUserViewModel userModel)
        {
            if (ModelState.IsValid)
            {
                var userDb = await _userManager.FindByIdAsync(userModel.Id);
                if (userDb != null)
                {
                    userDb.NikName = userModel.NikName;
                    userDb.RealName = userModel.Name;
                    userDb.SerName = userModel.SerName;
                    userDb.Email = userModel.Email;
                    userDb.PhoneNumber = userModel.Phone;

                    var result = await _userManager.UpdateAsync(userDb);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("PersonalData");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return View(userModel);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ChangePasswordAsync(ChangePersonalPassword passwordModel)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var identity = await _userManager.ChangePasswordAsync(user, passwordModel.OldPassword, passwordModel.NewPassword);
            if (identity.Succeeded)
            {
                return RedirectToAction("PersonalData");
            }
            else
            {
                foreach (var error in identity.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(passwordModel);
        }

        [HttpPost]
        public async Task<ActionResult> ChangeAvatarAsync(IFormFile uploadedAvatar)
        {
            if (uploadedAvatar != null)
            {
                string avatarImagePath = Path.Combine(_appEnvironment.WebRootPath + "/avatars/");

                if (!Directory.Exists(avatarImagePath))
                {
                    Directory.CreateDirectory(avatarImagePath);
                }

                var fileName = Guid.NewGuid() + "_" + uploadedAvatar.FileName;

                using (var fileStream = new FileStream(avatarImagePath + fileName, FileMode.Create))
                {
                    uploadedAvatar.CopyTo(fileStream);
                }

                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                if (System.IO.File.Exists(_appEnvironment.WebRootPath + user.Avatar))
                {
                    System.IO.File.Delete(_appEnvironment.WebRootPath + user.Avatar);
                }
                user.Avatar = "/avatars/" + fileName;
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("PersonalData");


        }
    }
}
