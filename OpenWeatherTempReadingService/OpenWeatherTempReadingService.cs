using Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.TempReaderModels;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Service
{
    public class OpenWeatherTempReadingService : TempReadingService
    {
        static readonly HttpClient client = new HttpClient();

        public OpenWeatherTempReadingService(ILogger<OpenWeatherTempReadingService> logger, IDataBaseOperation dataBase, IOptions<Settings> options) : base(logger, dataBase, options)
        { }

        protected override void DoStartAsync(CancellationToken stoppingToken)
        {
            client.BaseAddress = new Uri("http://api.openweathermap.org/data/2.5/");

            for (int i = 0; i < Settings.OpenWeatherMap.Cities.Length; i++)
            {
                string rom = Settings.OpenWeatherMap.Cities[i].ToString();
                var dbSensor = DataBase.GetSensorByROM(rom);

                if (dbSensor == null)
                    Sensors.Add(new SensorObject() { SensorID = -i - 1, ROM = rom, DeviceName = $"OpenWeather sensor for city id {rom}" });
                else
                    Sensors.Add(new SensorObject() { SensorID = dbSensor.sensorID, ROM = rom, DeviceName = $"OpenWeather sensor for city id {rom}" });
            }
        }

        private async Task<string> MakeRequest(int cityID)
        {
            string uri = $"weather?id={cityID}&units=metric&appid={Settings.OpenWeatherMap.OpenWeatherAPIKey}";
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
            foreach (SensorObject sensor in Sensors)
            {
                int cityID;
                if (int.TryParse(sensor.ROM, out cityID))
                {
                    Task<string> task = MakeRequest(cityID);
                    task.Wait();
                    string json = task.Result;
                    try
                    {
                        CityWeather weatherForecast = JsonSerializer.Deserialize<CityWeather>(json);
                        sensor.Name = weatherForecast.name;
                        AddValueToTempCache((id: sensor.SensorID, temperature: weatherForecast.main.temp));
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
