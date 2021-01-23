﻿namespace Services.Service
{
    public class Settings
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