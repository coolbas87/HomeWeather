using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace HomeWeather.Domain.Services.Implementation.TelegramBot.Callbacks
{
    public abstract class InlineCallback
    { 
        public abstract Task Execute(CallbackQuery callbackQuery, TelegramBotClient bot);
    }
}
