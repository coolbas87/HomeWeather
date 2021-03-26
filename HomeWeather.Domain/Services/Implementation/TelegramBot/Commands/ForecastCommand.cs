using System.Threading.Tasks;
using HomeWeather.Domain.Services.Implementation.TelegramBot.Callbacks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace HomeWeather.Domain.Services.Implementation.TelegramBot.Commands
{
    public class ForecastCommand : Command
    {
        public override string Name => @"/forecast";
        public override string Description => "Show weather forecast";

        public override InlineCallback Callback => null;

        public override bool IsHidden => false;

        public override async Task Execute(Message message, TelegramBotClient bot)
        {
            var chatId = message.Chat.Id;
            await bot.SendTextMessageAsync(chatId, "Here will be weather forecast", parseMode: ParseMode.Markdown);
        }
    }
}
