using HomeWeather.Domain.DTO.TelegramBot;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace HomeWeather.Domain.Services.Implementation.TelegramBot.Callbacks
{
    public class ApproveTrustedUserCommandInlineCallback : InlineCallback
    {
        private ObservableCollection<TrustedUserDTO> trustedUsers;
        private List<TrustedUserDTO> trustedUsersForApprove;

        public ApproveTrustedUserCommandInlineCallback(ObservableCollection<TrustedUserDTO> trustedUsers, List<TrustedUserDTO> trustedUsersForApprove)
        {
            this.trustedUsers = trustedUsers;
            this.trustedUsersForApprove = trustedUsersForApprove;
        }

        public override async Task Execute(CallbackQuery callbackQuery, TelegramBotClient bot)
        {
            if (trustedUsers.Any(u => u.ID == callbackQuery.From.Id))
            {
                string[] data = callbackQuery.Data.Split("|");
                int userID;

                if ((data.Length > 1) && int.TryParse(data[1], out userID) && trustedUsersForApprove.Any(u => u.ID == userID))
                {
                    TrustedUserDTO user = trustedUsersForApprove.First(u => u.ID == userID);
                    trustedUsers.Add(user);
                    trustedUsersForApprove.Remove(user);
                    await bot.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id, text: $"Selected user with ID: {user.ID}");

                    await bot.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: $"User {user.Username ?? user.FirstName} with ID {user.ID} added as trusted");
                }
                else
                {
                    await bot.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: "Wrong command. UserID is not found");
                }
            }
            else
            {
                await bot.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: "You are not trusted user. You can't approve users");
            }
        }
    }
}
