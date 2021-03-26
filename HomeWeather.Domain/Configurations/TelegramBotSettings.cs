using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWeather.Domain.Configurations
{
    public class TelegramBotSettings
    {
        public string TelegramBotAPIKey { get; set; }
        public int ReqTimeoutForUntrustedUsers { get; set; }
    }
}
