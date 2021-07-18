using HomeWeather.Domain.DTO;
using HomeWeather.Domain.Services.Implementation.TelegramBot.Callbacks;
using HomeWeather.Domain.Services.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace HomeWeather.Domain.Services.Implementation.TelegramBot.Commands
{
    public class ForecastByPositionCommand : ForecastCommandBase
    {
        public ForecastByPositionCommand(IWeatherForecastService weatherForecastService) : base(weatherForecastService) { }

        public override string Name => @"/forecast_location";
        public override string Description => "Show weather forecast at your location";

        public override InlineCallback Callback => null;

        public override bool IsHidden => false;

        public override async Task Execute(Message message, TelegramBotClient bot)
        {
            var chatId = message.Chat.Id;

            if (message.Type == MessageType.Text)
            {
                await bot.SendTextMessageAsync(chatId, "Just send me your current location", parseMode: ParseMode.Html);
                return;
            }
            else if (message.Type == MessageType.Location)
            {
                await base.Execute(message, bot);
            }
        }

        protected override async Task<WeatherForecastDTO> GetForecast(Message message)
        {
            return await weatherForecastService.GetForecastByCoords(
                    message.Location.Latitude, message.Location.Longitude);
        }
    }
}
