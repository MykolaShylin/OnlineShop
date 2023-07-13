
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Interfaces;
using OnlineShopWebApp.Helpers;
using OnlineShopWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Constants = OnlineShop.DB.Models.Constants;
using AutoMapper;

namespace OnlineShopWebApp.Areas.Admin.Controllers
{
    [Area(Constants.AdminRoleName)]
    [Authorize(Roles = Constants.AdminRoleName)]
    public class RoleController : Controller
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapping;
        public RoleController(RoleManager<Role> roleManager, UserManager<User> userManager, IMapper mapping)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _mapping = mapping;
        }

        public async Task<IActionResult> Roles()
        {
            var rolesView = _mapping.Map<List<RoleViewModel>>(await _roleManager.Roles.ToListAsync());
            return View(rolesView);
        }
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            await _roleManager.DeleteAsync(role);
            return RedirectToAction("Roles");
        }

        [HttpPost]
        public async Task<ActionResult> EditRoleAsync(RoleViewModel roleModel)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(roleModel.Id);

                role.Name= roleModel.Name;
                role.Description = roleModel.Description;

                await _roleManager.UpdateAsync(role);
            }
            return RedirectToAction($"Roles");
        }

        public async Task<IActionResult> ChangeUserRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var userRoles = await _userManager.GetRolesAsync(user);

            var allRoles = await _roleManager.Roles.ToListAsync();

            ChangeRoleViewModel model = new ChangeRoleViewModel
            {
                UserId = user.Id,
                Login = user.UserName,
                UserRoles = userRoles,
                AllRoles = allRoles
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ChangeUserRoleAsync(string userId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var userRoles = await _userManager.GetRolesAsync(user);
            
            var addedRoles = roles.Except(userRoles);

            var removedRoles = userRoles.Except(roles);

            await _userManager.AddToRolesAsync(user, addedRoles);

            await _userManager.RemoveFromRolesAsync(user, removedRoles);

            return Redirect($"/Admin/User/UserInfo?id={userId}");
        }
    }
}
