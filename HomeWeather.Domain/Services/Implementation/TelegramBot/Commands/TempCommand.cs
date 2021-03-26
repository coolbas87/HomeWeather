using System;
using System.Collections.Generic;
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
    public class TempCommand : Command
    {
        private readonly ISensorTempReader sensorTempReader;

        public override string Name => @"/temp";

        public override InlineCallback Callback => null;

        public override string Description => "Show temperature from all connected sensors";
        public override bool IsHidden => false;

        public TempCommand(ISensorTempReader sensorTempReader)
        {
            this.sensorTempReader = sensorTempReader;
        }

        public override async Task Execute(Message message, TelegramBotClient bot)
        {
            IEnumerable<TempDTO> temps = await sensorTempReader.GetTempAllSensors();

            var chatId = message.Chat.Id;

            StringBuilder response = new StringBuilder("Data from sensors is unavailable");

            bool isFirst = true;
            foreach (TempDTO item in temps)
            {
                if (!isFirst)
                {
                    response.Append('\n');
                }
                else
                {
                    response.Clear();
                    isFirst = false;
                }
                response.Append($"Sensor \"{item.Name}\": {Math.Round(item.Temperature, 1)}°C");
            }

            await bot.SendTextMessageAsync(chatId, response.ToString());
        }
    }
}
