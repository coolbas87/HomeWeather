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
    public class DeleteTrustedUserCommand : Command
    {
        private DeleteTrustedUserCommandInlineCallback callback;
        private ObservableCollection<TrustedUserDTO> trustedUsers;

        public override string Name => @"/DeleteTrustedUser";

        public override string Description => string.Empty;

        public override InlineCallback Callback => callback;

        public override bool IsHidden => true;

        public DeleteTrustedUserCommand(ObservableCollection<TrustedUserDTO> trustedUsers)
        {
            this.trustedUsers = trustedUsers;
            callback = new DeleteTrustedUserCommandInlineCallback(trustedUsers);
        }

        public async override Task Execute(Message message, TelegramBotClient bot)
        {
            if (trustedUsers.Any(u => u.ID == message.From.Id))
            {
                int count = trustedUsers.Count;
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
                            $"User {trustedUsers[i].Username ?? trustedUsers[i].FirstName} (ID: {trustedUsers[i].ID})",
                            $"{Name}|{trustedUsers[i].ID}")
                        );
                        if (i == count - 1)
                        {
                            buttons[j] = buttonsRow.ToArray();
                        }
                    }
                    var inlineKeyboard = new InlineKeyboardMarkup(buttons);
                    await bot.SendTextMessageAsync(chatId: message.Chat.Id, text: "Select user for delete", replyMarkup: inlineKeyboard);
                }
                else
                {
                    await bot.SendTextMessageAsync(chatId: message.Chat.Id, text: "No trusted users");
                }
            }
            else
            {
                await bot.SendTextMessageAsync(chatId: message.Chat.Id, text: "You are not trusted user. You can't delete users");
            }
            await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);
        }
    }
}
