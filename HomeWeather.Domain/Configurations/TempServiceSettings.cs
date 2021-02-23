namespace HomeWeather.Domain.Configurations
{
    public class TempServiceSettings
    {
        public int RefreshTempInterval { get; set; }
        public HistorySettings HistorySettings { get; set; }
    }

    public class HistorySettings
    {
        public int HourInterval { get; set; }
        public int MinuteInterval { get; set; }
    }
}
