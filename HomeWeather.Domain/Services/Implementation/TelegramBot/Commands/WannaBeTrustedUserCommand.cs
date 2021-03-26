using HomeWeather.Domain.DTO.TelegramBot;
using HomeWeather.Domain.Services.Implementation.TelegramBot.Callbacks;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace HomeWeather.Domain.Services.Implementation.TelegramBot.Commands
{
    public class WannaBeTrustedUserCommand : Command
    {
        private List<TrustedUserDTO> trustedUsersForApprove;

        public override string Name => @"/wanna_be_trusted_user";

        public override string Description => "Adds you to list of users for approve";

        public override InlineCallback Callback => null;

        public override bool IsHidden => false;

        public WannaBeTrustedUserCommand(List<TrustedUserDTO> trustedUsersForApprove)
        {
            this.trustedUsersForApprove = trustedUsersForApprove;
        }

        private void AddUser(Telegram.Bot.Types.User user)
        {
            if (!trustedUsersForApprove.Any(u => u.ID == user.Id))
            {
                trustedUsersForApprove.Add(new TrustedUserDTO() { ID = user.Id, Username = user.Username, FirstName = user.FirstName });
            }
        }

        public async override Task Execute(Message message, TelegramBotClient bot)
        {
            await Task.Run(() => AddUser(message.From));
            await bot.SendTextMessageAsync(message.Chat.Id, $"User {message.From.Username ?? message.From.FirstName} added to list users for approve");
        }
    }
}
