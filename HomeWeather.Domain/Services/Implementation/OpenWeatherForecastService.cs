using HomeWeather.Domain.Configurations;
using HomeWeather.Domain.DTO;
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
        private readonly HttpClient client = new HttpClient();
        private readonly ILogger logger;
        private readonly OpenWeatherSettings settings;

        public OpenWeatherForecastService(ILogger<OpenWeatherForecastService> logger, IOptions<OpenWeatherSettings> options)
        {
            client.BaseAddress = new Uri("http://api.openweathermap.org/data/2.5/");
            this.logger = logger;
            settings = options.Value;
        }

        public async Task<WeatherForecastDTO> GetForecast()
        {
            string uri = $"onecall?lat={settings.Lat}&lon={settings.Lon}&exclude=minutely,alerts&units=metric&appid={settings.OpenWeatherAPIKey}";
            OpenWeatherMapForecastDTO forecast = null;

            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStreamAsync();
                forecast = await JsonSerializer.DeserializeAsync<OpenWeatherMapForecastDTO>(responseBody);
            }
            catch (HttpRequestException e)
            {
                logger.LogError("\nException Caught!");
                logger.LogError("\nURI: {0}{1} ", client.BaseAddress, uri);
                logger.LogError("\nMessage: {0} ", e.Message);
            }

            return forecast;
        }
    }
}
