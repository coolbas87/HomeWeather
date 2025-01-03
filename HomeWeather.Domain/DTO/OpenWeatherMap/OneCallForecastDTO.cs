using System;
using System.Text.Json.Serialization;

namespace HomeWeather.Domain.DTO.OpenWeatherMap
{
    public class OneCallForecastDTO
    {
        public float lat { get; init; }
        public float lon { get; init; }
        public string timezone { get; init; }
        public int timezone_offset { get; init; }
        public Current current { get; set; }
        public Daily[] daily { get; init; }
    }

    public class Current
    {
        public int dt
        {
            init => DataDate = DateTimeOffset.FromUnixTimeSeconds(value).DateTime.ToLocalTime();
        }
        public DateTime DataDate { get; init; }
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
        public float temp { get; set; }
        public float feels_like { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public float dew_point { get; set; }
        public float uvi { get; set; }
        public int clouds { get; set; }
        public int visibility { get; set; }
        public float wind_speed { get; set; }
        public int wind_deg { get; set; }
        public float wind_gust { get; set; }
        public Weather[] weather { get; set; }
        public Rain rain { get; set; }
        public Snow snow { get; set; }
    }

    public class Daily
    {
        public int dt
        {
            init => DataDate = DateTimeOffset.FromUnixTimeSeconds(value).DateTime.ToLocalTime();
        }
        public DateTime DataDate { get; init; }
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
        public Temp temp { get; init; }
        public Feels_Like feels_like { get; init; }
        public int pressure { get; init; }
        public int humidity { get; init; }
        public float dew_point { get; init; }
        public float wind_speed { get; init; }
        public int wind_deg { get; init; }
        public float wind_gust { get; set; }
        public Weather[] weather { get; init; }
        public int clouds { get; init; }
        public float pop { get; init; }
        public float snow { get; init; }
        public float uvi { get; init; }
        public float rain { get; init; }
    }

    public class Temp
    {
        public float day { get; init; }
        public float min { get; init; }
        public float max { get; init; }
        public float night { get; init; }
        public float eve { get; init; }
        public float morn { get; init; }
    }

    public class Feels_Like
    {
        public float day { get; init; }
        public float night { get; init; }
        public float eve { get; init; }
        public float morn { get; init; }
    }
}
