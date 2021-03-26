using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomeWeather.Domain.DTO;
using HomeWeather.Domain.Services.Implementation.TelegramBot.Callbacks;
using HomeWeather.Domain.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace HomeWeather.Domain.Services.Implementation.TelegramBot.Commands
{
    public class SensorTempCommand : Command
    {
        private readonly SensorTempCommandInlineCallback callback;
        private readonly IPhysSensorInfo physSensorInfo;

        public override string Name => @"/sensor_temp";
        public override string Description => "Show temperature from selected sensor";
        public override bool IsHidden => false;

        public override InlineCallback Callback => callback;

        public SensorTempCommand(ISensorTempReader sensorTempReader, IPhysSensorInfo physSensorInfo)
        {
            this.physSensorInfo = physSensorInfo;
            callback = new SensorTempCommandInlineCallback(sensorTempReader);
        }

        public override async Task Execute(Message message, TelegramBotClient bot)
        {
            IEnumerable<SensorDTO> sensorsEnum = await physSensorInfo.GetSensors();
            List<SensorDTO> sensors = new List<SensorDTO>(sensorsEnum);

            int count = sensors.Count;
            if (count > 0)
            {
                int chunk = 2;
                int rows = (int)Math.Ceiling((double)count / chunk);
                InlineKeyboardButton[][] buttons = new InlineKeyboardButton[rows][];
                List<InlineKeyboardButton> buttonsRow = new List<InlineKeyboardButton>();
                for (int i = 0, j = 0; i < count; i++)
                {
                    if (i >= chunk && i % chunk == 0)
                    {
                        buttons[j] = buttonsRow.ToArray();
                        buttonsRow = new List<InlineKeyboardButton>();
                        j++;
                    }
                    buttonsRow.Add(InlineKeyboardButton.WithCallbackData($"{sensors[i].Name}", $"{Name}|{sensors[i].SensorID}"));
                    if (i == count - 1)
                    {
                        buttons[j] = buttonsRow.ToArray();
                    }
                }
                var inlineKeyboard = new InlineKeyboardMarkup(buttons);
                await bot.SendTextMessageAsync(chatId: message.Chat.Id, text: "Select sensor", replyMarkup: inlineKeyboard);
            }
        }
    }
}
