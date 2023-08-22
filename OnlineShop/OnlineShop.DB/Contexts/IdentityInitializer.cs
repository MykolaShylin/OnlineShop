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
            var admin = new User
            {
                UserName = "BullBody_Admin",
                RealName = "Николай",
                SerName = "Шилин",
                Email = "bullbody.ua@gmail.com",
                PhoneNumber = "+380975288699",
                Avatar = null,
                NikName = "Admin",
                EmailConfirmed = true,
            };

            var moderator = new User
            {
                UserName = "BullBody_Moder",
                RealName = "Николай",
                SerName = "Шилин",
                Email = "shylin.mykola@gmail.com",
                PhoneNumber = "+380975288699",
                Avatar = null,
                NikName = "Moderator",
                EmailConfirmed = true,
            };

            var adminPassword = "1q2w3e4r";

            var adminRoleDesc = "Доступ ко всему функционалу сайта! Внимание, не предоставляйте эти права доступа людям, которым вы не доверяете, это может повлиять на работу сайта!";
            var userRoleDesc = "Назначается всем новым зарегестрированным пользователям. Возможность только совершать покупки";
            var moderatorRoleDesc = "Помощник админа! Доступ к оформлению заказов, редактированию пользователей и акциям";

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
            if (roleManager.FindByNameAsync(Constants.ModeratorRoleName).Result == null)
            {
                var moderatorRole = new Role
                {
                    Name = Constants.ModeratorRoleName,
                    Description = moderatorRoleDesc
                };
                roleManager.CreateAsync(moderatorRole).Wait();
            }
            if (userManager.FindByNameAsync(admin.UserName).Result == null)
            {                
                var result = userManager.CreateAsync(admin, adminPassword).Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(admin, Constants.AdminRoleName).Wait();
                }
            }
            //if (userManager.FindByNameAsync(moderator.UserName).Result == null)
            //{
            //    var result = userManager.CreateAsync(moderator, adminPassword).Result;
            //    if (result.Succeeded)
            //    {
            //        userManager.AddToRoleAsync(admin, Constants.ModeratorRoleName).Wait();
            //    }
            //}
        }
    }
}
