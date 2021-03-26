using HomeWeather.Domain.DTO.TelegramBot;
using HomeWeather.Domain.Services.Implementation.TelegramBot.Callbacks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace HomeWeather.Domain.Services.Implementation.TelegramBot.Commands
{
    public class ApproveTrustedUserCommand : Command
    {
        private readonly ApproveTrustedUserCommandInlineCallback callback;
        private ObservableCollection<TrustedUserDTO> trustedUsers;
        private List<TrustedUserDTO> trustedUsersForApprove;

        public override string Name => @"/ApproveTrustedUser";

        public override string Description => string.Empty;

        public override InlineCallback Callback => callback;

        public override bool IsHidden => true;

        public ApproveTrustedUserCommand(ObservableCollection<TrustedUserDTO> trustedUsers, List<TrustedUserDTO> trustedUsersForApprove)
        {
            this.trustedUsers = trustedUsers;
            this.trustedUsersForApprove = trustedUsersForApprove;
            callback = new ApproveTrustedUserCommandInlineCallback(trustedUsers, trustedUsersForApprove);
        }

        public async override Task Execute(Message message, TelegramBotClient bot)
        {
            if (trustedUsers.Any(u => u.ID == message.From.Id))
            {
                int count = trustedUsersForApprove.Count;
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
                        buttonsRow.Add(InlineKeyboardButton.WithCallbackData
                        (
                            $"User {trustedUsersForApprove[i].Username ?? trustedUsersForApprove[i].FirstName} (ID: {trustedUsersForApprove[i].ID})", 
                            $"{Name}|{trustedUsersForApprove[i].ID}")
                        );
                        if (i == count - 1)
                        {
                            buttons[j] = buttonsRow.ToArray();
                        }
                    }
                    var inlineKeyboard = new InlineKeyboardMarkup(buttons);
                    await bot.SendTextMessageAsync(chatId: message.Chat.Id, text: "Select user for approve", replyMarkup: inlineKeyboard);
                }
                else
                {
                    await bot.SendTextMessageAsync(chatId: message.Chat.Id, text: "No users for approve");
                }
            }
            else
            {
                await bot.SendTextMessageAsync(chatId: message.Chat.Id, text: "You are not trusted user. You can't approve users");
            }
            await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);
        }
    }
}
