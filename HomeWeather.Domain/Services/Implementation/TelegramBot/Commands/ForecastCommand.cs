using System;
using System.Text;
using System.Threading.Tasks;
using HomeWeather.Domain.DTO;
using HomeWeather.Domain.Services.Implementation.TelegramBot.Callbacks;
using HomeWeather.Domain.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace HomeWeather.Domain.Services.Implementation.TelegramBot.Commands
{
    public class ForecastCommand : Command
    {
        private readonly string thunderstorm = "\U000026C8";    // Code: 200's, 900, 901, 902, 905
        private readonly string drizzle = "\U0001F327";         // Code: 300's
        private readonly string rain = "\U0001F326";            // Code: 500's
        private readonly string snowflake = "\U0001F328";       // Code: 600's snowflake
        private readonly string atmosphere = "\U0001F32B";      // Code: 700's foogy
        private readonly string clearSky = "\U00002600";        // Code: 800 clear sky
        private readonly string fewClouds = "\U000026C5";       // Code: 801 sun behind clouds
        private readonly string clouds = "\U00002601";          // Code: 802-803-804 clouds general
        private readonly string brokenClouds = "\U0001F325";    // Code: broken clouds
        private readonly string thermometer = "\U0001F321";
        private readonly string roundPushpin = "\U0001F4CD";
        private readonly string cyclone = "\U0001F300";
        private readonly string dashing = "\U0001F4A8";
        private readonly string leafFluttering = "\U0001F343";
        private readonly string droplet = "\U0001F4A7";
        private readonly string beachWithUmbrella = "\U0001F3D6";
        private readonly string sunrise = "\U0001F305";
        private readonly string sunset = "\U0001F307";
        private readonly string spiralCalendar = "\U0001F5D3";
        private readonly string telescope = "\U0001F52D";

        private readonly IWeatherForecastService weatherForecastService;

        public ForecastCommand(IWeatherForecastService weatherForecastService)
        {
            this.weatherForecastService = weatherForecastService;
        }

        public override string Name => @"/forecast";
        public override string Description => "Show weather forecast";

        public override InlineCallback Callback => null;

        public override bool IsHidden => false;

        private string GetEmojiByCode(string code)
        {
            string emoji = cyclone;

            switch (code)
            {
                case "01d":
                    emoji = clearSky;
                    break;

                case "02d":
                    emoji = fewClouds;
                    break;

                case "03d":
                    emoji = clouds;
                    break;

                case "04d":
                    emoji = brokenClouds;
                    break;

                case "09d":
                    emoji = drizzle;
                    break;

                case "10d":
                    emoji = rain;
                    break;

                case "11d":
                    emoji = thunderstorm;
                    break;

                case "13d":
                    emoji = snowflake;
                    break;

                case "50d":
                    emoji = atmosphere;
                    break;

                default:
                    break;
            }

            return emoji;
        }

        public override async Task Execute(Message message, TelegramBotClient bot)
        {
            var chatId = message.Chat.Id;
            WeatherForecastDTO forecast = await weatherForecastService.GetForecast();
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.AppendLine($"{roundPushpin} Current weather in <b>{forecast.Name}</b>");
            messageBuilder.AppendLine("");
            messageBuilder.AppendLine($"{GetEmojiByCode(forecast.Current.WeatherCondition.IconName)} <b>{forecast.Current.WeatherCondition.Name}</b>");
            messageBuilder.AppendLine($"{thermometer} <b>Temperature</b>: {Math.Round(forecast.Current.Temp, 1)}\u00B0C");
            messageBuilder.AppendLine($"{thermometer} <b>Feels like:</b> {Math.Round(forecast.Current.TempFeelsLike, 1)}\u00B0C");
            messageBuilder.AppendLine("");
            messageBuilder.AppendLine($"{dashing} <b>Wind:</b> {forecast.Current.WindSpeed} m/s");
            messageBuilder.AppendLine($"{leafFluttering} <b>Wind gust:</b> {forecast.Current.WindGust} m/s");
            messageBuilder.AppendLine($"{cyclone} <b>Pressure:</b> {forecast.Current.Pressure} hPa");
            messageBuilder.AppendLine($"{droplet} <b>Humidity:</b> {forecast.Current.Humidity}%");
            messageBuilder.AppendLine($"{telescope} <b>Visibility:</b> {forecast.Current.Visibility} m");
            messageBuilder.AppendLine($"{beachWithUmbrella} <b>UVI:</b> {forecast.Current.UVI}");
            messageBuilder.AppendLine("");
            messageBuilder.AppendLine($"{sunrise} <b>Sunrise:</b> {forecast.Current.SunriseDate}");
            messageBuilder.AppendLine($"{sunset} <b>Sunset:</b> {forecast.Current.SunsetDate}");
            messageBuilder.AppendLine("");
            messageBuilder.AppendLine($"{spiralCalendar} <b>Forecast date:</b> {forecast.Current.Date}");
            await bot.SendTextMessageAsync(chatId, messageBuilder.ToString(), parseMode: ParseMode.Html);
        }
    }
}
