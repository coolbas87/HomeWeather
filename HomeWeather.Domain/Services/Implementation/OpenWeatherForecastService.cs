using HomeWeather.Domain.Configurations;
using HomeWeather.Domain.DTO;
using HomeWeather.Domain.DTO.OpenWeatherMap;
using HomeWeather.Domain.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HomeWeather.Domain.Services.Implementation
{
    public class OpenWeatherForecastService : IWeatherForecastService
    {
        private readonly ILocationService locationService;
        private readonly HttpClient client = new HttpClient();
        private readonly ILogger logger;
        private readonly OpenWeatherSettings settings;
        private WeatherForecastDTO LastForecast = null;
        private WeatherForecastDTO LastLocationForecast = null;
        private DateTime LastDateTimeRequest = DateTime.MinValue;
        private DateTime LastLocationDateTimeRequest = DateTime.MinValue;
        private CityInfo LastCityInfo = default(CityInfo);

        struct CityInfo
        {
            public int CityID { get; init; }
            public float Lat { get; init; }
            public float Lon { get; init; }
            public string Name { get; init; }
        }

        public OpenWeatherForecastService(ILogger<OpenWeatherForecastService> logger, IOptions<OpenWeatherSettings> options, ILocationService locationService)
        {
            client.BaseAddress = new Uri("http://api.openweathermap.org/data/");
            this.logger = logger;
            settings = options.Value;
            this.locationService = locationService;
        }

        private WeatherForecastDTO ConvertToWeatherForecast(OneCallForecastDTO openWeatherForecast)
        {
            List<DailyWeatherForecast> dailyWeatherForecasts = new List<DailyWeatherForecast>();
            CurrentWeather current = new CurrentWeather()
            {
                Date = openWeatherForecast.current.DataDate,
                SunriseDate = openWeatherForecast.current.SunriseDate,
                SunsetDate = openWeatherForecast.current.SunsetDate,
                CloudinessPerc = openWeatherForecast.current.clouds,
                DewPoint = openWeatherForecast.current.dew_point,
                Humidity = openWeatherForecast.current.humidity,
                Pressure = openWeatherForecast.current.pressure,
                Temp = openWeatherForecast.current.temp,
                TempFeelsLike = openWeatherForecast.current.feels_like,
                UVI = openWeatherForecast.current.uvi,
                Visibility = openWeatherForecast.current.visibility,
                WindDegrees = openWeatherForecast.current.wind_deg,
                WindGust = openWeatherForecast.current.wind_gust,
                WindSpeed = openWeatherForecast.current.wind_speed,
                RainVolLast1Hour = openWeatherForecast.current.rain?.volLast1Hour ?? 0,
                SnowVolLast1Hour = openWeatherForecast.current.snow?.volLast1Hour ?? 0,
                WeatherCondition = openWeatherForecast.current.weather.Length > 0 ? new WeatherCondition()
                {
                    Name = openWeatherForecast.current.weather[0].main,
                    Description = openWeatherForecast.current.weather[0].description,
                    IconName = openWeatherForecast.current.weather[0].icon
                }
                : default(WeatherCondition)
            };

            foreach (Daily openWeatherDaily in openWeatherForecast.daily)
            {
                DailyWeatherForecast daily = new DailyWeatherForecast()
                {
                    Date = openWeatherDaily.DataDate,
                    SunriseDate = openWeatherDaily.SunriseDate,
                    SunsetDate = openWeatherDaily.SunsetDate,
                    CloudinessPerc = openWeatherDaily.clouds,
                    DewPoint = openWeatherDaily.dew_point,
                    Humidity = openWeatherDaily.humidity,
                    Pressure = openWeatherDaily.pressure,
                    TempMax = openWeatherDaily.temp.max,
                    TempMin = openWeatherDaily.temp.min,
                    UVI = openWeatherDaily.uvi,
                    PrecipPercProbability = (int)openWeatherDaily.pop * 100,
                    WindDegrees = openWeatherDaily.wind_deg,
                    WindGust = openWeatherDaily.wind_gust,
                    WindSpeed = openWeatherDaily.wind_speed,
                    RainVol = openWeatherDaily.rain,
                    SnowVol = openWeatherDaily.snow,
                    WeatherCondition = openWeatherDaily.weather.Length > 0 ? new WeatherCondition()
                    {
                        Name = openWeatherDaily.weather[0].main,
                        Description = openWeatherDaily.weather[0].description,
                        IconName = openWeatherDaily.weather[0].icon
                    }
                    : default(WeatherCondition)
                };

                dailyWeatherForecasts.Add(daily);
            }

            return new WeatherForecastDTO() { Name = LastCityInfo.Name, Lat = openWeatherForecast.lat, 
                Lon = openWeatherForecast.lon, Current = current, Daily = dailyWeatherForecasts.ToArray() };
        }

        private WeatherForecastDTO ConvertToWeatherForecast(CurrentWeatherDTO currentWeather, FiveDayForecastDTO fiveDayForecast)
        {
            List<DailyWeatherForecast> dailyWeatherForecasts = new List<DailyWeatherForecast>();
            CurrentWeather current = new CurrentWeather()
            {
                Date = currentWeather.DataDate,
                SunriseDate = currentWeather.sys.SunriseDate,
                SunsetDate = currentWeather.sys.SunsetDate,
                CloudinessPerc = currentWeather.clouds.all,
                DewPoint = 0,
                Humidity = currentWeather.main.humidity,
                Pressure = currentWeather.main.pressure,
                Temp = currentWeather.main.temp,
                TempFeelsLike = currentWeather.main.feels_like,
                UVI = 0,
                Visibility = currentWeather.visibility,
                WindDegrees = currentWeather.wind.deg,
                WindGust = currentWeather.wind.gust,
                WindSpeed = currentWeather.wind.speed,
                RainVolLast1Hour = currentWeather.rain?.volLast1Hour ?? 0,
                SnowVolLast1Hour = 0,
                WeatherCondition = currentWeather.weather.Length > 0 ? new WeatherCondition()
                {
                    Name = currentWeather.weather[0].main,
                    Description = currentWeather.weather[0].description,
                    IconName = currentWeather.weather[0].icon
                }
                : default(WeatherCondition)
            };

            foreach (DayHour dayHour in fiveDayForecast.list.Where(x => x.DataDate.Hour == 0 || x.DataDate.Hour == 6 || x.DataDate.Hour == 12 || x.DataDate.Hour == 21))
            {
                DailyWeatherForecast daily = new DailyWeatherForecast()
                {
                    Date = dayHour.DataDate,
                    SunriseDate = fiveDayForecast.city.SunriseDate,
                    SunsetDate = fiveDayForecast.city.SunsetDate,
                    CloudinessPerc = dayHour.clouds.all,
                    DewPoint = 0,
                    Humidity = dayHour.main.humidity,
                    Pressure = dayHour.main.pressure,
                    TempMax = dayHour.main.temp_max,
                    TempMin = dayHour.main.temp_min,
                    UVI = 0,
                    PrecipPercProbability = (int)dayHour.pop * 100,
                    WindDegrees = dayHour.wind.deg,
                    WindGust = dayHour.wind.gust,
                    WindSpeed = dayHour.wind.speed,
                    RainVol = dayHour.rain?.volLast1Hour ?? 0,
                    SnowVol = 0,
                    WeatherCondition = dayHour.weather.Length > 0 ? new WeatherCondition()
                    {
                        Name = dayHour.weather[0].main,
                        Description = dayHour.weather[0].description,
                        IconName = dayHour.weather[0].icon
                    }
                    : default(WeatherCondition)
                };

                dailyWeatherForecasts.Add(daily);
            }

            return new WeatherForecastDTO()
            {
                Name = LastCityInfo.Name,
                Lat = currentWeather.coord.lat,
                Lon = currentWeather.coord.lon,
                Current = current,
                Daily = dailyWeatherForecasts.ToArray()
            };
        }

        private async Task<T> MakeRequest<T>(string uri)
        {
            T reqObject = default(T);

            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStreamAsync();
                reqObject = await JsonSerializer.DeserializeAsync<T>(responseBody);
            }
            catch (HttpRequestException e)
            {
                logger.LogError("\nException Caught!");
                logger.LogError("\nURI: {0}{1} ", client.BaseAddress, uri);
                logger.LogError("\nMessage: {0} ", e.Message);
            }

            return reqObject;
        }

        private async Task<CityInfo> GetCityInfo()
        {
            string uri = $"2.5/weather?id={settings.ForecastCityID}&units=metric&appid={settings.OpenWeatherAPIKey}";

            CityWeatherDTO cityInfo = await MakeRequest<CityWeatherDTO>(uri);

            return new CityInfo() { CityID = settings.ForecastCityID, Lat = cityInfo.coord.lat, Lon = cityInfo.coord.lon, Name = cityInfo.name };
        }

        public async Task<WeatherForecastDTO> GetForecast()
        {
            TimeSpan dateDiff = DateTime.Now - LastDateTimeRequest;
            if (dateDiff.TotalHours >= 1)
            {
                if (LastCityInfo.CityID != settings.ForecastCityID)
                {
                    LastCityInfo = await GetCityInfo();
                }

                LastDateTimeRequest = DateTime.Now;

                LastForecast = await DoGetForecastByCoords(LastCityInfo.Lat, LastCityInfo.Lon);
            }

            return LastForecast;
        }

        public async Task<WeatherForecastDTO> GetForecastByCoords(float Lat, float Lon)
        {
            TimeSpan dateDiff = DateTime.Now - LastLocationDateTimeRequest;
            if ((dateDiff.TotalHours >= 1) || (Math.Round(LastLocationForecast.Lat, 2) - Math.Round(Lat, 2) > 0.02) ||
                (Math.Round(LastLocationForecast.Lon, 2) - Math.Round(Lon, 2) > 0.02))
            {
                LastCityInfo = new CityInfo() { Lat = Lat, Lon = Lon, Name = await locationService.GetLocationNameByCoords(Lat, Lon) };

                LastLocationDateTimeRequest = DateTime.Now;

                LastLocationForecast = await DoGetForecastByCoords(LastCityInfo.Lat, LastCityInfo.Lon);
            }

            return LastLocationForecast;
        }

        private async Task<WeatherForecastDTO> DoGetForecastByCoords(float Lat, float Lon)
        {
            //string uri = $"3.0/onecall?lat={Lat}&lon={Lon}&exclude=minutely,alerts&units=metric&appid={settings.OpenWeatherAPIKey}";

            //OneCallForecastDTO forecast = await MakeRequest<OneCallForecastDTO>(uri);

            //return ConvertToWeatherForecast(forecast);

            string uri = $"2.5/weather?lat={Lat}&lon={Lon}&units=metric&appid={settings.OpenWeatherAPIKey}";
            CurrentWeatherDTO currentForecast = await MakeRequest<CurrentWeatherDTO>(uri);
            uri = $"2.5/forecast?lat={Lat}&lon={Lon}&units=metric&appid={settings.OpenWeatherAPIKey}";
            FiveDayForecastDTO fiveDayForecast = await MakeRequest<FiveDayForecastDTO>(uri);
            var lst = fiveDayForecast.list.Where(x => x.DataDate.Hour == 0 || x.DataDate.Hour == 6 || x.DataDate.Hour == 12 || x.DataDate.Hour == 21).ToArray();

            return ConvertToWeatherForecast(currentForecast, fiveDayForecast);
        }
    }
}
