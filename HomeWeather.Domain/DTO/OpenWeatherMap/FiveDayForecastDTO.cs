using System;

namespace HomeWeather.Domain.DTO.OpenWeatherMap
{
    public class FiveDayForecastDTO
    {
        public string cod { get; set; }
        public int message { get; set; }
        public int cnt { get; set; }
        public DayHour[] list { get; set; }
        public City city { get; set; }
    }

    public class City
    {
        public int dt
        {
            init => DataDate = DateTimeOffset.FromUnixTimeSeconds(value).DateTime.ToLocalTime();
        }
        public DateTime DataDate { get; init; }
        public string name { get; set; }
        public Coord coord { get; set; }
        public string country { get; set; }
        public int population { get; set; }
        public int timezone { get; set; }
        public int sunrise
        {
            init => SunriseDate = DateTimeOffset.FromUnixTimeSeconds(value).DateTime.ToLocalTime();
        }
        public DateTime SunriseDate { get; init; }
        public int sunset
        {
            init => SunsetDate = DateTimeOffset.FromUnixTimeSeconds(value).DateTime.ToLocalTime();
        }
        public DateTime SunsetDate { get; init; }
    }

    public class DayHour
    {
        public int dt
        {
            init => DataDate = DateTimeOffset.FromUnixTimeSeconds(value).DateTime;
        }
        public DateTime DataDate { get; init; }
        public Main main { get; set; }
        public Weather[] weather { get; set; }
        public Clouds clouds { get; set; }
        public Wind wind { get; set; }
        public int visibility { get; set; }
        public float pop { get; set; }
        public Rain rain { get; set; }
        public DayHourSys sys { get; set; }
        public string dt_txt { get; set; }
    }

    public class DayHourSys
    {
        public string pod { get; set; }
    }
}
