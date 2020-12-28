using HomeWeather.Controllers;
using HomeWeather.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HomeWeather.Services
{
    public class DummyTempReaderService : IHostedService, ITempReader
    {
        private readonly ILogger<DummyTempReaderService> _logger;
        private readonly object lockAccess = new object();
        private Timer _timer;
        private List<SensorObject> sensors;
        private readonly ConcurrentDictionary<long, float> tempCache;
        private readonly Settings settings;
        private DateTime nextTimeForHistory;
        private readonly IServiceScopeFactory _scopeFactory;

        public DummyTempReaderService(ILogger<DummyTempReaderService> logger, IOptions<Settings> options, IServiceScopeFactory scopeFactory): base()
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            tempCache = new ConcurrentDictionary<long, float>();
            settings = options.Value;
            sensors = new List<SensorObject>();
            for (int i = 0; i <= 1; i++)
            {
                int rndROM = 111111111 + i;
                sensors.Add(new SensorObject() { SensorID = rndROM, ROM = rndROM.ToString(), DeviceName = $"Sensor {rndROM}"});
            }
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {       
            SetNextTimeForHist();
            ReadTemperature();

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(settings.RefreshTempInterval));

            _logger.LogInformation("DummyTempReaderService running.");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DummyTempReaderService is stopping.");
            lock (lockAccess)
            {
                _timer?.Change(Timeout.Infinite, 0);
            }

            return Task.CompletedTask;
        }

        private void ReadTemperature()
        {
            Random rnd = new Random();

            foreach (SensorObject sensor in sensors)
            {
                float Temp = (float)(rnd.Next(-35, 39) + rnd.NextDouble());

                tempCache.AddOrUpdate(sensor.SensorID, Temp, (key, existingVal) =>
                {
                    existingVal = Temp;
                    return existingVal;
                });
            }
        }

        private void DoWork(object state)
        {
            lock (lockAccess)
            {
                ReadTemperature();
                if (DateTime.Compare(nextTimeForHistory, DateTime.Now) <= 0)
                {
                    foreach (KeyValuePair<long, float> item in tempCache)
                    {
                        WriteTempToHistory(item.Key, item.Value);
                    }
                    SetNextTimeForHist();
                }
            }
        }

        private void SetNextTimeForHist()
        {
            DateTime recodedDate;

            if (settings.HistorySettings.MinuteInterval == 30)
            {
                recodedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, settings.HistorySettings.MinuteInterval, 0);
                if (recodedDate <= DateTime.Now)
                {
                    recodedDate = recodedDate.AddMinutes(settings.HistorySettings.MinuteInterval);
                }
            }
            else if (settings.HistorySettings.MinuteInterval <= 0)
            {
                recodedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            }
            else
            {
                recodedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                recodedDate = recodedDate.AddMinutes(settings.HistorySettings.MinuteInterval);
            }

            nextTimeForHistory = recodedDate.AddHours(settings.HistorySettings.HourInterval);
        }

        private void WriteTempToHistory(long sensorID, float temp)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<HWDbContext>();

                dbContext.TempHistory.Add(new TempHistory
                {
                    snID = sensorID,
                    Temperature = temp,
                    Date = nextTimeForHistory
                });
                dbContext.SaveChanges();
            }
        }

        public async Task<IEnumerable> LastMeasuredTemp()
        {
            List<TempCache> cache = new List<TempCache>();

            if (tempCache.Count > 0)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<HWDbContext>();

                    foreach (KeyValuePair<long, float> item in tempCache)
                    {
                        var sensor = await dbContext.Sensors.FindAsync(item.Key);
                        cache.Add(new TempCache()
                        {
                            snID = item.Key,
                            Temperature = item.Value,
                            ROM = sensor.ROM,
                            Name = sensor.Name
                        });
                    }

                }
            }

            return cache.AsEnumerable();
        }

        public async Task<object> LastMeasuredTempBySensor(long snID)
        {
            if (tempCache.Count > 0)
            {
                foreach (KeyValuePair<long, float> item in tempCache)
                {
                    if (item.Key == snID)
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<HWDbContext>();

                            var sensor = await dbContext.Sensors.FindAsync(item.Key);
                            return new TempCache() { snID = item.Key, Temperature = item.Value, ROM = sensor.ROM, Name = sensor.Name };
                        }
                    }
                }
            }

            return new TempCache() { snID = long.MinValue, Temperature = float.NaN };
        }

        public IEnumerable SensorsList()
        {
            List<SensorObject> sensorsList = new List<SensorObject>();

            foreach (SensorObject item in sensors)
            {
                lock (lockAccess)
                {
                    sensorsList.Add(new SensorObject() { ROM = item.ROM, SensorID = item.SensorID, DeviceName = item.DeviceName });
                }
            }

            return sensorsList.AsEnumerable();
        }

        public object SensorInfo(long sensorID)
        {
            foreach (SensorObject item in sensors)
            {
                if (item.SensorID == sensorID)
                {
                    lock (lockAccess)
                    {
                        return new SensorInfoObj() { SensorID = item.SensorID, ROM = item.ROM, DeviceName = item.DeviceName, Info = "Dummy Info field"};
                    }
                }
            }

            throw new NullReferenceException("Sensor is not found");
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public string Name => nameof(DummyTempReaderService);
    }
}
