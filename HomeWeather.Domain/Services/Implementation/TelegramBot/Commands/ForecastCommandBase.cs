using HomeWeather.Domain.DTO;
using HomeWeather.Domain.Services.Implementation.TelegramBot.Callbacks;
using HomeWeather.Domain.Services.Interfaces;
using System;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace HomeWeather.Domain.Services.Implementation.TelegramBot.Commands
{
    public abstract class ForecastCommandBase: Command
    {
        protected readonly string thunderstorm = "\U000026C8";    // Code: 200's, 900, 901, 902, 905
        protected readonly string drizzle = "\U0001F327";         // Code: 300's
        protected readonly string rain = "\U0001F326";            // Code: 500's
        protected readonly string snowflake = "\U0001F328";       // Code: 600's snowflake
        protected readonly string atmosphere = "\U0001F32B";      // Code: 700's foogy
        protected readonly string clearSky = "\U00002600";        // Code: 800 clear sky
        protected readonly string fewClouds = "\U000026C5";       // Code: 801 sun behind clouds
        protected readonly string clouds = "\U00002601";          // Code: 802-803-804 clouds general
        protected readonly string brokenClouds = "\U0001F325";    // Code: broken clouds
        protected readonly string thermometer = "\U0001F321";
        protected readonly string roundPushpin = "\U0001F4CD";
        protected readonly string cyclone = "\U0001F300";
        protected readonly string dashing = "\U0001F4A8";
        protected readonly string leafFluttering = "\U0001F343";
        protected readonly string droplet = "\U0001F4A7";
        protected readonly string beachWithUmbrella = "\U0001F3D6";
        protected readonly string sunrise = "\U0001F305";
        protected readonly string sunset = "\U0001F307";
        protected readonly string spiralCalendar = "\U0001F5D3";
        protected readonly string telescope = "\U0001F52D";
        protected readonly string clockOneThirty = "\U0001F55C";

        protected readonly IWeatherForecastService weatherForecastService;

        public ForecastCommandBase(IWeatherForecastService weatherForecastService)
        {
            this.weatherForecastService = weatherForecastService;
        }

        protected abstract Task<WeatherForecastDTO> GetForecast(Message message);

        public override async Task Execute(Message message, TelegramBotClient bot)
        {
            var chatId = message.Chat.Id;
            WeatherForecastDTO forecast = await GetForecast(message);
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
            messageBuilder.AppendLine("");
            messageBuilder.AppendLine("Short daily forecast");

            int daysInShortForecast = 12;
            DateTime lastDateTime = DateTime.MinValue;
            
            foreach (var dayForecast in forecast.Daily)
            {
                if (daysInShortForecast > 0)
                {
                    messageBuilder.AppendLine("");
                    
                    if (lastDateTime.Date.Ticks != dayForecast.Date.Date.Ticks)
                    {
                        lastDateTime = dayForecast.Date.Date;
                        messageBuilder.AppendLine($"{spiralCalendar} {dayForecast.Date.ToString("dd.MM.yyyy")}");
                    }
                    messageBuilder.AppendLine($"{clockOneThirty} {dayForecast.Date.ToString("HH:mm")}");

                    messageBuilder.AppendLine($"{thermometer} {Math.Round(dayForecast.TempMin, 0)} - {Math.Round(dayForecast.TempMax, 0)}\u00B0C {GetEmojiByCode(dayForecast.WeatherCondition.IconName)}");
                    messageBuilder.AppendLine($"{dashing} {Math.Round(dayForecast.WindSpeed, 0)} m/s");
                    daysInShortForecast--;
                }
                else
                {
                    break;
                }
            }

            messageBuilder.AppendLine("");
            messageBuilder.AppendLine("For detailed daily forecast use command /daily_forecast");

            await bot.SendTextMessageAsync(chatId, messageBuilder.ToString(), parseMode: ParseMode.Html);
        }

        protected virtual string GetEmojiByCode(string code)
        {
            string emoji = cyclone;

            switch (code)
            {
                case "01d":
                case "01n":
                    emoji = clearSky;
                    break;

                case "02d":
                case "02n":
                    emoji = fewClouds;
                    break;

                case "03d":
                case "03n":
                    emoji = clouds;
                    break;

                case "04d":
                case "04n":
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
    }
}