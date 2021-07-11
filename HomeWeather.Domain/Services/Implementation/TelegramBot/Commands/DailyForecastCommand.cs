using HomeWeather.Domain.Services.Implementation.TelegramBot.Callbacks;
using HomeWeather.Domain.Services.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace HomeWeather.Domain.Services.Implementation.TelegramBot.Commands
{
    public class DailyForecastCommand : Command
    {
        private readonly IWeatherForecastService weatherForecastService;

        public DailyForecastCommand(IWeatherForecastService weatherForecastService)
        {
            this.weatherForecastService = weatherForecastService;
        }

        public override string Name => @"/daily_forecast";
        public override string Description => "Show daily weather forecast";

        public override InlineCallback Callback => null;

        public override bool IsHidden => false;

        public override async Task Execute(Message message, TelegramBotClient bot)
        {
            var chatId = message.Chat.Id;

            await bot.SendTextMessageAsync(chatId, "Sorry, this command is in development", parseMode: ParseMode.Html);
        }
    }
}
