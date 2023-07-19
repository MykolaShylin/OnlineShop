using Microsoft.EntityFrameworkCore;
using OnlineShop.DB.Contexts;
using OnlineShop.DB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.DB.Storages
{
    public class UserTelegramDbStorage
    {
        private readonly IdentityContext identityContext;

        public UserTelegramDbStorage(IdentityContext identityContext)
        {
            this.identityContext = identityContext;
        }
        /// <summary>
        /// Возвращает пользователя по телеграмЮзерАйди, если он есть в БД. 
        /// </summary>
        /// <param name="telegramUserId"></param>
        /// <returns></returns>
        public async Task<User> TryGetByTelegramUserIdAsync(long? telegramUserId)
        {
            return await identityContext.Users.FirstOrDefaultAsync(x => x.TelegramUserId == telegramUserId);
        }

        public async Task<User> TryGetByNameAsync(string name)
        {
            return await identityContext.Users.FirstOrDefaultAsync(x => x.UserName == name);
        }

        /// <summary>
        /// Метод добавляет в БД TelegramUserId по номеру телефона
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> UpdateTelegramUserIdAsync(string phone, long userId)
        {
            var trimNumber = (string number) => number.StartsWith("+") ? number.Substring(1) : number;
            var user = await identityContext.Users.FirstOrDefaultAsync(x => (x.PhoneNumber.StartsWith("+") ? x.PhoneNumber.Substring(1) : x.PhoneNumber) == phone);
            if (user != null)
            {
                user.TelegramUserId = userId;
                identityContext.SaveChanges();
                return true;
            }

            return false;
        }
    }
}
