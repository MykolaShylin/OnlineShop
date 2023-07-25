
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineShop.DB.Contexts;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.DB
{
    public class UserTelegramDbStorage : ITelegramBot
    {
        private readonly IdentityContext _userManager;

        public UserTelegramDbStorage(IdentityContext userManager)
        {
            _userManager = userManager;
        }
        /// <summary>
        /// Возвращает пользователя по телеграмЮзерАйди, если он есть в БД. 
        /// </summary>
        /// <param name="telegramUserId"></param>
        /// <returns></returns>
        public async Task<User> TryGetByTelegramUserIdAsync(long? telegramUserId)
        {
            return await _userManager.Users.FirstOrDefaultAsync(x=> x.TelegramUserId == telegramUserId);
        }
        /// <summary>
        /// Метод добавляет в БД TelegramUserId по номеру телефона
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateTelegramUserIdAsync(string phone, long userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber.Contains(phone));
            if (user != null)
            {
                user.TelegramUserId = userId;
                await _userManager.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
