using System;

namespace HomeWeather.Domain.DTO.OpenWeatherMap
{
    public class CurrentWeatherDTO
    {
        public int dt
        {
            init => DataDate = DateTimeOffset.FromUnixTimeSeconds(value).DateTime.ToLocalTime();
        }
        public DateTime DataDate { get; init; }
        public Coord coord { get; set; }
        public Weather[] weather { get; set; }
        public string _base { get; set; }
        public Main main { get; set; }
        public int visibility { get; set; }
        public Wind wind { get; set; }
        public Rain rain { get; set; }
        public Clouds clouds { get; set; }
        public Sys sys { get; set; }
        public int timezone { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }
    }

    public class Clouds
    {
        public int all { get; set; }
    }

    public class Sys
    {
        public int type { get; set; }
        public int id { get; set; }
        public string country { get; set; }
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
}
