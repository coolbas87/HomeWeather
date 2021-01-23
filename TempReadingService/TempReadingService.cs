using Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
    public abstract class TempReadingService : IHostedService, IDisposable, ITempReader
    {
        private readonly ILogger<TempReadingService> logger;
        private readonly IDataBaseOperation dataBase;
        private readonly object lockAccess = new object();
        private Timer timer;
        private List<SensorObject> sensors;
        private readonly ConcurrentDictionary<long, float> tempCache;
        private readonly Settings settings;
        private DateTime nextTimeForHistory;

        protected List<SensorObject> Sensors => sensors;
        protected ILogger<TempReadingService> Logger => logger;
        protected IDataBaseOperation DataBase => dataBase;
        protected Timer Timer => timer;
        protected Settings Settings => settings;

        public TempReadingService(ILogger<TempReadingService> logger, IDataBaseOperation dataBase, IOptions<Settings> options) : base()
        {
            this.logger = logger;
            this.dataBase = dataBase;
            tempCache = new ConcurrentDictionary<long, float>();
            settings = options.Value;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            sensors = new List<SensorObject>();

            DoStartAsync(stoppingToken);

            SetNextTimeForHist();

            timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(settings.RefreshTempInterval));

            logger.LogInformation($"{Name} running.");

            return Task.CompletedTask;
        }

        protected virtual void DoStartAsync(CancellationToken stoppingToken) { }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation($"{Name} is stopping.");
            lock (lockAccess)
            {
                DoStopAsync(stoppingToken);
                sensors?.Clear();
                timer?.Change(Timeout.Infinite, 0);
            }

            return Task.CompletedTask;
        }

        protected virtual void DoStopAsync(CancellationToken stoppingToken) { }

        protected virtual void ReadTemperature()
        {
            AddValueToTempCache((-1, 0));
        }

        protected void AddValueToTempCache((long id, float temperature) value)
        {
            tempCache.AddOrUpdate(value.id, value.temperature, (key, existingVal) =>
            {
                existingVal = value.temperature;
                return existingVal;
            });
        }

        protected void DeleteValueInTempCahce(long id)
        {
            float value;
            tempCache.TryRemove(id, out value);
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
            dataBase.WriteTempToHistory(sensorID, temp, nextTimeForHistory);
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
            lock (lockAccess)
            {
                var sensorInfo = DoGetSensorObjectInfo(sensorID);
                
                if (sensorInfo != null)
                {
                    return sensorInfo;
                }
            }

            throw new NullReferenceException($"Sensor with ID = {sensorID} is not found");
        }

        protected virtual object DoGetSensorObjectInfo(long sensorID)
        {
            var sensor = sensors.FirstOrDefault(sn => sn.SensorID == sensorID);

            if (sensor != null)
            {
                return new SensorObject() { SensorID = (sensor.SensorID), ROM = sensor.ROM, DeviceName = sensor.DeviceName };
            }
            else
            {
                return null;
            }
        }

        public virtual void Dispose()
        {
            timer?.Dispose();
        }

        public virtual string Name => nameof(TempReadingService);
    }
}
