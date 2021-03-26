using HomeWeather.Domain.DTO.TelegramBot;
using HomeWeather.Domain.Services.Implementation.TelegramBot.Callbacks;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace HomeWeather.Domain.Services.Implementation.TelegramBot.Commands
{
    public class AddUserToTrustedComand : Command
    {
        private ObservableCollection<TrustedUserDTO> trustedUsers;
        private readonly string password;
        public override string Name => @"/AddUserToTrusted";

        public override string Description => string.Empty;

        public override InlineCallback Callback => null;

        public override bool IsHidden => true;

        public AddUserToTrustedComand(ObservableCollection<TrustedUserDTO> trustedUsers, string password)
        {
            this.trustedUsers = trustedUsers;
            this.password = password;
        }

        private void AddUser(Telegram.Bot.Types.User user)
        {
            if (!trustedUsers.Any(u => u.ID == user.Id))
            {
                trustedUsers.Add(new TrustedUserDTO() { ID = user.Id, Username = user.Username, FirstName = user.FirstName });
            }
        }

        public override async Task Execute(Message message, TelegramBotClient bot)
        {
            string[] splittedMessage = message.Text.Split(' ');

            if ((splittedMessage.Length >= 2) && (splittedMessage[1] == password))
            {
                await Task.Run(() => AddUser(message.From));
                await bot.SendTextMessageAsync(message.Chat.Id, $"User {message.From.Username ?? message.From.FirstName} added as trusted");
            }
            else
            {
                await bot.SendTextMessageAsync(message.Chat.Id, "This command requires password. Enter command name then whitespace and password");
            }
            await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);
        }
    }
}
