using System;

namespace HomeWeather.Domain.DTO
{
    public record WeatherForecastDTO 
    {
        public float Lat { get; set; }
        public float Lon { get; set; }
        public string Name { get; init; }
        public CurrentWeather Current { get; init; }
        public DailyWeatherForecast[] Daily { get; init; }
    }

    public record WeatherCondition
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public string IconName { get; init; }
    }

    public record CurrentWeather
    {
        public DateTime Date { get; init; }
        public DateTime SunriseDate { get; init; }
        public DateTime SunsetDate { get; init; }
        public WeatherCondition WeatherCondition { get; init; }
        public float Temp { get; init; }
        public float TempFeelsLike { get; init; }
        public int Pressure { get; init; }
        public int Humidity { get; init; }
        public float DewPoint { get; init; }
        public float UVI { get; init; }
        public int CloudinessPerc { get; init; }
        public int Visibility { get; init; }
        public float WindSpeed { get; init; }
        public int WindDegrees { get; init; }
        public float WindGust { get; init; }
        public float RainVolLast1Hour { get; init; }
        public float SnowVolLast1Hour { get; init; }
    }

    public record DailyWeatherForecast
    {
        public DateTime Date { get; init; }
        public DateTime SunriseDate { get; init; }
        public DateTime SunsetDate { get; init; }
        public WeatherCondition WeatherCondition { get; init; }
        public float TempMin { get; init; }
        public float TempMax { get; init; }
        public int Pressure { get; init; }
        public int Humidity { get; init; }
        public float DewPoint { get; init; }
        public float WindSpeed { get; init; }
        public int WindDegrees { get; init; }
        public float WindGust { get; init; }
        public int CloudinessPerc { get; init; }
        public int PrecipPercProbability { get; init; }
        public float UVI { get; init; }
        public float RainVol { get; init; }
        public float SnowVol { get; init; }
    }
}
