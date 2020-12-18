using HomeWeather.Controllers;
using HomeWeather.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneWireTempLib;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace HomeWeather.Services
{
    public partial class TempReadingService : IHostedService, IDisposable, ITempReader
    {
        private readonly ILogger<TempReadingService> _logger;
        private readonly object lockAccess = new object();
        private Timer _timer;
        private UART_Adapter uart;
        private List<OneWireSensor> sensors;
        private readonly ConcurrentDictionary<long, float> tempCache;
        private readonly Settings settings;
        private DateTime nextTimeForHistory;
        private readonly IServiceScopeFactory _scopeFactory;

        public TempReadingService(ILogger<TempReadingService> logger, IOptions<Settings> options, IServiceScopeFactory scopeFactory): base()
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            tempCache = new ConcurrentDictionary<long, float>();
            settings = options.Value;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            uart = new UART_Adapter(settings.COMPort);
            uart.Open();
            OneWireSensor sensor = new DS18B20(uart);
            List<byte[]> ROMs = new List<byte[]>();
            ROMs = sensor.GetConnectedROMs();
            sensors = new List<OneWireSensor>();
            foreach (byte[] item in ROMs)
            {
                sensors.Add(Utils.CreateSensor(item[0], uart, item));
            }
            SetNextTimeForHist();
            ReadTemperature();

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(settings.RefreshTempInterval));

            _logger.LogInformation("TempReadingService running.");

            return Task.CompletedTask;
        }

        private void ReadTemperature()
        {
            if (uart.IsOpened)
            {
                foreach (OneWireSensor sensor in sensors)
                {
                    float Temp = sensor.GetTemperature();

                    tempCache.AddOrUpdate(sensor.ROMInt, Temp, (key, existingVal) =>
                    {
                        existingVal = Temp;
                        return existingVal;
                    });
                }
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

        private void WriteTempToHistory(long RomInt, float temp)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<HWDbContext>();

                dbContext.TempHistory.Add(new TempHistory {
                    snID = RomInt,
                    Temperature = temp,
                    Date = nextTimeForHistory
                });
                dbContext.SaveChanges();
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TempReadingService is stopping.");
            lock (lockAccess)
            {
                sensors?.Clear();

                uart?.Close();

                _timer?.Change(Timeout.Infinite, 0);
            }

            return Task.CompletedTask;
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
                        if (sensor == null)
                        {
                            cache.Add(new TempCache()
                            {
                                snID = item.Key,
                                Temperature = item.Value,
                                ROM = "N/A",
                                Name = "N/A"
                            });
                        }
                        else
                        {
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
            }

            return cache.AsEnumerable();
        }

        public async Task<object> LastMeasuredTempBySensor(long ROMInt)
        {
            if (tempCache.Count > 0)
            {
                foreach (KeyValuePair<long, float> item in tempCache)
                {
                    if (item.Key == ROMInt)
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

            return new TempCache() { snID = -1, Temperature = 0.0F };
        }

        public IEnumerable SensorsList()
        {
            List<SensorObject> sensorsList = new List<SensorObject>();

            foreach (OneWireSensor item in sensors)
            {
                lock (lockAccess)
                {
                    sensorsList.Add(new SensorObject() { ROM = item.ROM, RomInt = item.ROMInt, DeviceName = item.DeviceName(item.FamilyCode)});
                }
            }

            return sensorsList.AsEnumerable();
        }

        public object SensorInfo(long ROMInt)
        {
            foreach (OneWireSensor item in sensors)
            {
                if (item.ROMInt == ROMInt)
                {
                    lock (lockAccess)
                    {
                        return new SensorInfoObj() { RomInt = item.ROMInt, ROM = item.ROM, DeviceName = item.DeviceName(item.FamilyCode), Info = item.Info() };
                    }
                }
            }

            throw new NullReferenceException("Sensor is not found");
        }

        public void Dispose()
        {
            uart?.Dispose();
            _timer?.Dispose();
        }

        public string Name => nameof(TempReadingService);
    }
}
