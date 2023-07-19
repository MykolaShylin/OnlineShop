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

        void SendContactRequest(long chatId);
        void SendWelcomeMessage(long chatId, string firstName);

        void SendResponse(long chatId, string text);

        void SendKeyboard(long chatId, string text);
    }
}
