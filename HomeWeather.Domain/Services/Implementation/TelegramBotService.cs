using HomeWeather.Data.Entities;
using HomeWeather.Data.Interfaces;
using HomeWeather.Domain.Configurations;
using HomeWeather.Domain.DTO.TelegramBot;
using HomeWeather.Domain.Services.Implementation.TelegramBot.Commands;
using HomeWeather.Domain.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace HomeWeather.Domain.Services.Implementation
{
    public class TelegramBotService : IBotService, IHostedService
    {
        private bool isStopped = true;
        private readonly ILogger<TelegramBotService> logger;
        private readonly ISensorTempReader sensorTempReader;
        private readonly IWeatherForecastService weatherForecastService;
        private readonly IPhysSensorInfo physSensorInfo;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly TelegramBotClient botClient;
        private readonly List<Command> commands = new List<Command>();
        private readonly List<UntrustedUserDTO> untrustedUsers = new List<UntrustedUserDTO>();
        private readonly ObservableCollection<TrustedUserDTO> trustedUsers = new ObservableCollection<TrustedUserDTO>();
        private readonly List<TrustedUserDTO> trustedUsersForApprove = new List<TrustedUserDTO>();
        private readonly string token;
        private string myUsername;
        private readonly TelegramBotSettings telegramBotSettings;

        public TelegramBotClient BotClient => botClient;


        public TelegramBotService(
            ILogger<TelegramBotService> logger,
            ISensorTempReader sensorTempReader,
            IPhysSensorInfo physSensorInfo,
            IWeatherForecastService weatherForecastService,
            IServiceScopeFactory scopeFactory,
            IOptions<TelegramBotSettings> options)
        {
            this.logger = logger;
            this.sensorTempReader = sensorTempReader;
            this.physSensorInfo = physSensorInfo;
            this.weatherForecastService = weatherForecastService;
            this.scopeFactory = scopeFactory;
            telegramBotSettings = options.Value;
            token = telegramBotSettings.TelegramBotAPIKey;
            LoadTrustedUsersFromDB();
            trustedUsers.CollectionChanged += TrustedUsers_CollectionChanged;

            botClient = new TelegramBotClient(token) { Timeout = TimeSpan.FromSeconds(10) };
            botClient.OnMessage += BotClient_OnMessage;
            botClient.OnCallbackQuery += BotClient_OnCallbackQuery;
        }

        private void LoadTrustedUsersFromDB()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                IUnitOfWork<TelegramBotTrustedUser> teleBotTrustedUsersUnitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<TelegramBotTrustedUser>>();
                
                trustedUsers.Clear();
                foreach (TelegramBotTrustedUser dbUser in teleBotTrustedUsersUnitOfWork.GetRepository().Query())
                {
                    trustedUsers.Add(new TrustedUserDTO()
                    {
                        ID = dbUser.userID,
                        FirstName = dbUser.FirstName,
                        Username = dbUser.Username
                    });
                }
            }
        }

        private void TrustedUsers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                IUnitOfWork<TelegramBotTrustedUser> teleBotTrustedUsersUnitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<TelegramBotTrustedUser>>();

                switch (eventArgs.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            if (eventArgs.NewItems.Count > 0)
                            {
                                foreach (var item in eventArgs.NewItems)
                                {
                                    if (item is TrustedUserDTO)
                                    {
                                        TrustedUserDTO trustedUser = (TrustedUserDTO)item;
                                        TelegramBotTrustedUser dbTrustedUser = teleBotTrustedUsersUnitOfWork.GetRepository().Query().FirstOrDefault(user => user.userID == trustedUser.ID);
                                        if (dbTrustedUser == null)
                                        {
                                            teleBotTrustedUsersUnitOfWork.GetRepository().Add(new TelegramBotTrustedUser
                                            {
                                                userID = trustedUser.ID,
                                                FirstName = trustedUser.FirstName,
                                                Username = trustedUser.Username
                                            });
                                        }
                                    }
                                }
                                teleBotTrustedUsersUnitOfWork.SaveChanges();
                            }
                            break;
                        }
                    case NotifyCollectionChangedAction.Remove:
                        {
                            if (eventArgs.OldItems.Count > 0)
                            {
                                foreach (var item in eventArgs.OldItems)
                                {
                                    if (item is TrustedUserDTO)
                                    {
                                        TrustedUserDTO trustedUser = (TrustedUserDTO)item;
                                        TelegramBotTrustedUser dbTrustedUser = teleBotTrustedUsersUnitOfWork.GetRepository().Query().FirstOrDefault(user => user.userID == trustedUser.ID);
                                        if (dbTrustedUser != null)
                                        {
                                            teleBotTrustedUsersUnitOfWork.GetRepository().Delete(dbTrustedUser);
                                        }
                                    }
                                }
                                teleBotTrustedUsersUnitOfWork.SaveChanges();
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        #region IHostedService
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var me = await botClient.GetMeAsync();
            myUsername = me.Username;

            await StartBot();
            logger.LogInformation($"Started listening for @{myUsername}");
        }
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await StopBot();
            logger.LogInformation($"Stop listening for @{myUsername}");
        }
        #endregion

        #region IHostedService
        public bool IsStopped => isStopped;
        public async Task StartBot()
        {
            RegisterCommands();
            List<BotCommand> botCommands = new List<BotCommand>();
            for (int i = 0; i < commands.Count; i++)
            {
                if (!commands[i].IsHidden)
                {
                    botCommands.Add(new BotCommand() { Command = commands[i].Name, Description = commands[i].Description });
                }
            }

            await botClient.SetMyCommandsAsync(botCommands);
            botClient.StartReceiving(Array.Empty<UpdateType>());
            isStopped = false;
        }

        public async Task StopBot()
        {
            await Task.Run(() => botClient.StopReceiving());
            isStopped = true;
        }
        #endregion

        private void RegisterCommands()
        {
            commands.Add(new TempCommand(sensorTempReader));
            commands.Add(new SensorTempCommand(sensorTempReader, physSensorInfo));
            commands.Add(new ForecastCommand(weatherForecastService));
            commands.Add(new DailyForecastCommand(weatherForecastService));
            commands.Add(new AddUserToTrustedComand(trustedUsers, token.Substring(token.Length - 8)));
            commands.Add(new ApproveTrustedUserCommand(trustedUsers, trustedUsersForApprove));
            commands.Add(new WannaBeTrustedUserCommand(trustedUsersForApprove));
            commands.Add(new DeleteTrustedUserCommand(trustedUsers));
        }

        private async void BotClient_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs callbackQueryEventArgs)
        {
            CallbackQuery callbackQuery = callbackQueryEventArgs.CallbackQuery;

            if (await IsCanAnswer(callbackQuery.From, callbackQuery.Message))
            {
                await botClient.SendChatActionAsync(callbackQuery.Message.Chat.Id, ChatAction.Typing);
                await Task.Delay(500);

                string[] callbackData = callbackQuery.Data.Split('|');
                if (callbackData.Length > 1)
                {
                    Command command = commands.FirstOrDefault(c => c.Name == callbackData[0]);
                    if (command != null)
                    {
                        if (command.Callback != null)
                        {
                            await command.Callback.Execute(callbackQuery, botClient);
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Unknown command").ConfigureAwait(false);
                    }
                }
            }
        }

        private async void BotClient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (await IsCanAnswer(message.From, message))
            {
                await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                await Task.Delay(500);

                if (message == null || message.Type != MessageType.Text)
                {
                    await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                    return;
                }

                logger.LogDebug($"You received message {message.Text}");

                Command cmd = commands.FirstOrDefault(c => c.Name == message.Text.Split(' ').First());

                if (cmd == null)
                {
                    await SendCommandsList(message);
                }
                else
                {
                    await cmd.Execute(message, botClient);
                }
            }
        }

        private async Task<bool> IsCanAnswer(Telegram.Bot.Types.User from, Message message)
        {
            int userID = from.Id;
            string userName = from.Username;

            if (!trustedUsers.Any(u => u.ID == userID))
            {
                UntrustedUserDTO user = untrustedUsers.FirstOrDefault(u => u.ID == userID);
                if (user != null)
                {
                    TimeSpan timeSpan = DateTime.Now - user.TimeLastRequest;
                    if (timeSpan.TotalMinutes < telegramBotSettings.ReqTimeoutForUntrustedUsers)
                    {
                        if (!user.IsNotified)
                        {
                            long chatID = message.Chat.Id;
                            await botClient.SendTextMessageAsync(chatID, 
                                $"You make requests too often. Try after {user.TimeLastRequest.AddMinutes(telegramBotSettings.ReqTimeoutForUntrustedUsers)}").ConfigureAwait(false);
                            user.IsNotified = true;
                        }
                        return false;
                    }
                    else
                    {
                        untrustedUsers.Remove(user);
                    }
                }
                else
                {
                    untrustedUsers.Add(new UntrustedUserDTO() { ID = userID, IsNotified = false, TimeLastRequest = DateTime.Now });
                }
            }
            return true;
        }

        private async Task SendCommandsList(Message message)
        {
            StringBuilder commandsList = new StringBuilder();

            foreach (Command command in commands)
            {
                if (!command.IsHidden)
                {
                    commandsList.Append($"{command.Name}\n");
                }
            }

            await botClient.SendTextMessageAsync(message.Chat.Id, $"List of commands:\n{commandsList}").ConfigureAwait(false);
        }
    }
}
