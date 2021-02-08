﻿using HomeWeather.Data.Entities;
using HomeWeather.Data.Interfaces;
using HomeWeather.Domain.Configurations;
using HomeWeather.Domain.DTO;
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
    public class OpenWeatherTempReadingService : TempReadingService
    {
        static readonly HttpClient client = new HttpClient();
        private readonly OpenWeatherMap openWeatherMapOptions;

        public OpenWeatherTempReadingService(
            ILogger<TempReadingService> logger,
            IUnitOfWork<Sensors> sensorsUnitOfWork,
            IUnitOfWork<TempHistory> tempHistUnitOfWork,
            IOptions<TempService> options,
            IOptions<OpenWeatherMap> openWeatherMapOptions) : base(logger, sensorsUnitOfWork, tempHistUnitOfWork, options) 
        {
            this.openWeatherMapOptions = openWeatherMapOptions.Value;
        }

        protected override void DoStartAsync(object stoppingToken)
        {
            client.BaseAddress = new Uri("http://api.openweathermap.org/data/2.5/");

            for (int i = 0; i < openWeatherMapOptions.Cities.Length; i++)
            {
                string rom = openWeatherMapOptions.Cities[i].ToString();
                var dbSensor = SensorsUnitOfWork.GetRepository().Query().FirstOrDefault(sn => sn.ROM == rom);

                if (dbSensor == null)
                    Sensors.Add(new SensorDTO() { SensorID = -i - 1, ROM = rom, DeviceName = $"OpenWeather sensor for city id {rom}" });
                else
                    Sensors.Add(new SensorDTO() { SensorID = dbSensor.snID, ROM = rom, DeviceName = $"OpenWeather sensor for city id {rom}" });
            }
        }

        private async Task<string> MakeRequest(int cityID)
        {
            string uri = $"weather?id={cityID}&units=metric&appid={openWeatherMapOptions.OpenWeatherAPIKey}";
            string responseBody = string.Empty;

            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                responseBody = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                Logger.LogError("\nException Caught!");
                Logger.LogError("\nURI: {0}{1} ", client.BaseAddress, uri);
                Logger.LogError("\nMessage: {0} ", e.Message);
                Logger.LogError("\nResponse body: {0}", responseBody);
            }
            return responseBody;
        }

        protected override void ReadTemperature()
        {
            foreach (SensorDTO sensor in Sensors)
            {
                if (int.TryParse(sensor.ROM, out int cityID))
                {
                    var request = MakeRequest(cityID);
                    request.Wait();
                    string json = request.Result;
                    try
                    {
                        CityWeatherDTO weatherForecast = JsonSerializer.Deserialize<CityWeatherDTO>(json);
                        sensor.Name = weatherForecast.Name;
                        AddValueToTempCache((id: sensor.SensorID, temperature: weatherForecast.Main.Temp));
                    }
                    catch (Exception e)
                    {
                        Logger.LogError("\nException Caught!");
                        Logger.LogError("\nMessage: {0} ", e.Message);
                        DeleteValueInTempCahce(sensor.SensorID);
                    }
                }
            }
        }

        public override string Name => nameof(OpenWeatherTempReadingService);
    }
}