using System;
using System.Linq;
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
    public class ForecastCommand : ForecastCommandBase
    {
        public ForecastCommand(IWeatherForecastService weatherForecastService): base(weatherForecastService) { }

        public override string Name => @"/forecast";
        public override string Description => "Show weather forecast";
        public override InlineCallback Callback => null;
        public override bool IsHidden => false;
        
        protected override async Task<WeatherForecastDTO> GetForecast(Message message)
        {
            return await weatherForecastService.GetForecast();
        }
    }
}
