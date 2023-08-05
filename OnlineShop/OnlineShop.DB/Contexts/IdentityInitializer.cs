using Microsoft.AspNetCore.Identity;
using OnlineShop.DB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.DB.Contexts
{
    public class IdentityInitializer
    {
        public static void Initialize(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            var email = "bullbody.ua@gmail.com";
            var password = "1q2w3e4r";
            var userName = "Николай";
            var serName = "Шилин";
            var login = "BullBody_Admin";
            var nikName = "Admin";
            var phone = "+380975288699";
            var emailConfirmed = true;

            var adminRoleDesc = "Доступ ко всему функционалу сайта! Внимание, не предоставляйте эти права доступа людям, которым вы не доверяете, это может повлиять на работу сайта!";
            var userRoleDesc = "Назначается всем новым зарегестрированным пользователям. Возможность только совершать покупки";

            if (roleManager.FindByNameAsync(Constants.AdminRoleName).Result == null)
            {
                var adminRole = new Role
                {
                    Name = Constants.AdminRoleName,
                    Description = adminRoleDesc
                };
                roleManager.CreateAsync(adminRole).Wait();
            }
            if (roleManager.FindByNameAsync(Constants.UserRoleName).Result == null)
            {
                var userRole = new Role
                {
                    Name = Constants.UserRoleName,
                    Description = userRoleDesc
                };
                roleManager.CreateAsync(userRole).Wait();
            }
            if (userManager.FindByNameAsync(login).Result == null)
            {
                var admin = new User
                {
                    UserName = login,
                    RealName = userName,                    
                    SerName= serName,
                    Email = email, 
                    PhoneNumber = phone,
                    Avatar = null,
                    NikName = nikName,
                    EmailConfirmed= emailConfirmed,
                };
                var result = userManager.CreateAsync(admin, password).Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(admin, Constants.AdminRoleName).Wait();
                }
            }
        }
    }
}
