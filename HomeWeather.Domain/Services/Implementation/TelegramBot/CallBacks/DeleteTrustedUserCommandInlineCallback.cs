using HomeWeather.Domain.DTO.TelegramBot;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace HomeWeather.Domain.Services.Implementation.TelegramBot.Callbacks
{
    public class DeleteTrustedUserCommandInlineCallback : InlineCallback
    {
        private ObservableCollection<TrustedUserDTO> trustedUsers;

        public DeleteTrustedUserCommandInlineCallback(ObservableCollection<TrustedUserDTO> trustedUsers)
        {
            this.trustedUsers = trustedUsers;
        }

        public override async Task Execute(CallbackQuery callbackQuery, TelegramBotClient bot)
        {
            if (trustedUsers.Any(u => u.ID == callbackQuery.From.Id))
            {
                string[] data = callbackQuery.Data.Split("|");
                int userID;

                if ((data.Length > 1) && int.TryParse(data[1], out userID) && trustedUsers.Any(u => u.ID == userID))
                {
                    TrustedUserDTO user = trustedUsers.First(u => u.ID == userID);
                    trustedUsers.Remove(user);
                    await bot.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id, text: $"Selected user with ID: {user.ID}");

                    await bot.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: $"User {user.Username ?? user.FirstName} with ID {user.ID} deleted from trusted");
                }
                else
                {
                    await bot.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: "Wrong command. UserID is not found");
                }
            }
            else
            {
                await bot.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: "You are not trusted user. You can't delete users");
            }
        }
    }
}
