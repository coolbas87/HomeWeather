using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using HomeWeather.Domain.Services.Implementation.TelegramBot.Callbacks;

namespace HomeWeather.Domain.Services.Implementation.TelegramBot.Commands
{
    public abstract class Command
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract Task Execute(Message message, TelegramBotClient bot);
        public abstract InlineCallback Callback { get; }
        public abstract bool IsHidden { get; }
    }
}
