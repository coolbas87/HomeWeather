﻿using HomeWeather.Domain.DTO;
using System.Threading.Tasks;

namespace HomeWeather.Domain.Services.Interfaces
{
    public interface IWeatherForecastService
    {
        public Task<WeatherForecastDTO> GetForecast();
        public Task<WeatherForecastDTO> GetForecastByCoords(float Lat, float Lon);
    }
}
