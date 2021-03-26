using HomeWeather.Domain.DTO;
using HomeWeather.Domain.Services.Interfaces;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace HomeWeather.Domain.Services.Implementation.TelegramBot.Callbacks
{
    public class SensorTempCommandInlineCallback : InlineCallback
    {
        private readonly ISensorTempReader sensorTempReader;

        public SensorTempCommandInlineCallback(ISensorTempReader sensorTempReader)
        {
            this.sensorTempReader = sensorTempReader;
        }
        public override async Task Execute(CallbackQuery callbackQuery, TelegramBotClient bot)
        {
            string[] data = callbackQuery.Data.Split("|");
            long sensorID;

            if ((data.Length > 1) && long.TryParse(data[1], out sensorID))
            {
                TempDTO sensorTemp = await sensorTempReader.GetTempBySensor(sensorID);
                await bot.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id, text: $"Selected sensor with ID: {sensorID}");

                await bot.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: $"Sensor \"{sensorTemp.Name}\": {Math.Round(sensorTemp.Temperature, 1)}°C");
            }
            else
            {
                await bot.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: "Wrong command. SensorID is not found");
            }
        }
    }
}
