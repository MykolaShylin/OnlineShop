using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.DB.Models.Interfaces
{
    public interface ITelegramBot
    {
        Task<User> TryGetByTelegramUserIdAsync(long? telegramUserId);
        Task<bool> UpdateTelegramUserIdAsync(string phone, long userId);
    }
}
