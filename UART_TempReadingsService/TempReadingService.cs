using Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OneWireTempLib;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Services.TempReaderModels;
using Microsoft.Extensions.Options;

namespace Services.Service
{
    public partial class TempReadingService : IHostedService, IDisposable, ITempReader
    {
        private readonly ILogger<TempReadingService> _logger;
        private readonly IDataBaseOperation _dataBase;
        private readonly object lockAccess = new object();
        private Timer _timer;
        private UART_Adapter uart;
        private List<SensorObject> sensors;
        private readonly ConcurrentDictionary<long, float> tempCache;
        private readonly Settings settings;
        private DateTime nextTimeForHistory;

        public TempReadingService(ILogger<TempReadingService> logger, IDataBaseOperation dataBase, IOptions<Settings> options): base()
        {
            _logger = logger;
            _dataBase = dataBase;
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
            sensors = new List<SensorObject>();

            foreach (byte[] item in ROMs)
            {
                OneWireSensor physSensor = Utils.CreateSensor(item[0], uart, item);
                var dbSensor = _dataBase.GetSensorByROM(physSensor.ROM);
                if (dbSensor == null)
                    sensors.Add(new SensorObject(physSensor) { SensorID = -1, Name = "Not in DB", ROM = physSensor.ROM, DeviceName = physSensor.DeviceName(physSensor.FamilyCode) });
                else
                    sensors.Add(new SensorObject(physSensor) { SensorID = dbSensor.sensorID, Name = dbSensor.Name, ROM = physSensor.ROM, DeviceName = physSensor.DeviceName(physSensor.FamilyCode) });
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
                foreach (SensorObject sensor in sensors)
                {
                    float temp = sensor.PhysSensor.GetTemperature();

                    tempCache.AddOrUpdate(sensor.SensorID, temp, (key, existingVal) =>
                    {
                        existingVal = temp;
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

        private void WriteTempToHistory(long sensorID, float temp)
        {
            _dataBase.WriteTempToHistory(sensorID, temp, nextTimeForHistory);
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
                foreach (KeyValuePair<long, float> item in tempCache)
                {
                    SensorObject sensor = await Task.Run(() => sensors.FirstOrDefault(sn => sn.SensorID == item.Key));
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
                        SensorObject sensor = await Task.Run(() => sensors.FirstOrDefault(sn => sn.SensorID == item.Key));
                        if (sensor != null)
                            return new TempCache() { snID = item.Key, Temperature = item.Value, ROM = sensor.ROM, Name = sensor.Name };
                        break;
                    }
                }
            }

            return new TempCache() { snID = -1, Temperature = 0.0F, ROM = "N/A", Name = "N/A" };
        }

        public IEnumerable SensorsList()
        {
            return sensors.AsEnumerable();
        }

        public object SensorInfo(long sensorID)
        {
            foreach (SensorObject item in sensors)
            {
                if (item.SensorID == sensorID)
                {
                    lock (lockAccess)
                    {
                        return new SensorInfoObj() { SensorID = item.SensorID, ROM = item.ROM, DeviceName = item.DeviceName, Info = item.PhysSensor.Info() };
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
