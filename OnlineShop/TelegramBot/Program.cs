using Telegram.Bot;
using TelegramBot;

class Program
{
    static TelegramBotClient Bot;
    static void Main(string[] args)
    {
        var chat = new ChatBotAPI();
        chat.Init();
        Console.ReadLine();
    }

}