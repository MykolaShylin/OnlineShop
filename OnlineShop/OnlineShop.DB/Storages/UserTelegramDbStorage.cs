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
        public User TryGetByTelegramUserId(long? telegramUserId)
        {
            return identityContext.Users.FirstOrDefault(x => x.TelegramUserId == telegramUserId);
        }

        public User TryGetByName(string name)
        {
            return identityContext.Users.FirstOrDefault(x => x.UserName == name);
        }

        /// <summary>
        /// Метод добавляет в БД TelegramUserId по номеру телефона
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UpdateTelegramUserId(string phone, long userId)
        {
            var trimNumber = (string number) => number.StartsWith("+") ? number.Substring(1) : number;
            var user = identityContext.Users.FirstOrDefault(x => (x.PhoneNumber.StartsWith("+") ? x.PhoneNumber.Substring(1) : x.PhoneNumber) == phone);
            if (user != null)
            {
                user.TelegramUserId = userId;
                identityContext.SaveChanges();
                return true;
            }

            return false;
        }

        public void UpdateUser()
        {
            identityContext.SaveChanges();
        }
    }
}
