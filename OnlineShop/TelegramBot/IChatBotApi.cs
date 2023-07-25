using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot
{
    public interface IChatBotAPI
    {
        void Init();

        Task SendContactRequest(long chatId);
        Task SendWelcomeMessage(long chatId, string firstName);

        Task SendResponse(long chatId, string text);

        Task SendKeyboard(long chatId, string text);
    }
}
