using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWeather.Domain.Configurations
{
    public class TempService
    {
        public string COMPort { get; set; }
        public int RefreshTempInterval { get; set; }
        public HistorySettings HistorySettings { get; set; }
    }

    public class HistorySettings
    {
        public int HourInterval { get; set; }
        public int MinuteInterval { get; set; }
    }
}
