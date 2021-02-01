using HomeWeather.Data.Entities;
using HomeWeather.Data.Interfaces;
using HomeWeather.Domain.Configurations;
using HomeWeather.Domain.DTO;
using HomeWeather.Domain.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HomeWeather.Domain.Services.Implementation
{
    public class TempReadingService : IHostedService, ITempReader, IDisposable
    {
        private readonly ILogger<TempReadingService> logger;
        private readonly IUnitOfWork<Sensors> sensorsUnitOfWork;
        private readonly IUnitOfWork<TempHistory> tempHistUnitOfWork;
        private readonly object lockAccess = new object();
        private Timer timer;
        private List<SensorDTO> sensors;
        private readonly ConcurrentDictionary<long, float> tempCache;
        private readonly TempService tempServiceOptions;
        private DateTime nextTimeForHistory;

        protected List<SensorDTO> Sensors => sensors;
        protected ILogger<TempReadingService> Logger => logger;
        protected Timer Timer => timer;
        protected TempService TempServiceOptions => tempServiceOptions;
        protected IUnitOfWork<Sensors> SensorsUnitOfWork => sensorsUnitOfWork;
        protected IUnitOfWork<TempHistory> TempHistUnitOfWork => tempHistUnitOfWork;

        public TempReadingService(
            ILogger<TempReadingService> logger, 
            IUnitOfWork<Sensors> sensorsUnitOfWork, 
            IUnitOfWork<TempHistory> tempHistUnitOfWork,
            IOptions<TempService> options)
        {
            this.logger = logger;
            tempCache = new ConcurrentDictionary<long, float>();
            tempServiceOptions = options.Value;
        }

        #region IHostedService
        public Task StartAsync(CancellationToken cancellationToken)
        {
            sensors = new List<SensorDTO>();

            DoStartAsync(cancellationToken);

            SetNextTimeForHist();

            timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(tempServiceOptions.RefreshTempInterval));

            logger.LogInformation($"{Name} running.");

            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"{Name} is stopping.");
            lock (lockAccess)
            {
                DoStopAsync(cancellationToken);
                sensors?.Clear();
                timer?.Change(Timeout.Infinite, 0);
            }

            return Task.CompletedTask;
        }
        #endregion

        protected virtual void DoStartAsync(object stoppingToken) { }

        protected virtual void DoStopAsync(object stoppingToken) { }

        private void SetNextTimeForHist()
        {
            DateTime recodedDate;

            if (tempServiceOptions.HistorySettings.MinuteInterval == 30)
            {
                recodedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, tempServiceOptions.HistorySettings.MinuteInterval, 0);
                if (recodedDate <= DateTime.Now)
                {
                    recodedDate = recodedDate.AddMinutes(tempServiceOptions.HistorySettings.MinuteInterval);
                }
            }
            else if (tempServiceOptions.HistorySettings.MinuteInterval <= 0)
            {
                recodedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            }
            else
            {
                recodedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                recodedDate = recodedDate.AddMinutes(tempServiceOptions.HistorySettings.MinuteInterval);
            }

            nextTimeForHistory = recodedDate.AddHours(tempServiceOptions.HistorySettings.HourInterval);
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
            tempCache.TryRemove(id, out _);
        }

        private void WriteTempToHistory(long sensorID, float temp)
        {
            var dbSensor = sensorsUnitOfWork.GetRepository().GetById(sensorID);

            if (dbSensor != null)
            {
                tempHistUnitOfWork.GetRepository().Add(new TempHistory
                { 
                    snID = sensorID,
                    Temperature = temp,
                    Date = nextTimeForHistory
                });
            }
        }

        #region IDisposable
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                timer?.Dispose();
            }

            disposed = true;
        }
        #endregion

        #region ITempReader
        public IEnumerable<TempDTO> LastMeasuredTemp()
        {
            var cache = new List<TempDTO>();

            if (tempCache.Count > 0)
            {
                foreach (KeyValuePair<long, float> item in tempCache)
                {
                    SensorDTO sensor = sensors.FirstOrDefault(sn => sn.SensorID == item.Key);
                    if (sensor == null)
                    {
                        cache.Add(new TempDTO()
                        {
                            snID = item.Key,
                            Temperature = item.Value,
                            ROM = "N/A",
                            Name = "N/A"
                        });
                    }
                    else
                    {
                        cache.Add(new TempDTO()
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

        public TempDTO LastMeasuredTempBySensor(long snID)
        {
            if (tempCache.Count > 0)
            {
                foreach (KeyValuePair<long, float> item in tempCache)
                {
                    if (item.Key == snID)
                    {
                        SensorDTO sensor = sensors.FirstOrDefault(sn => sn.SensorID == item.Key);
                        if (sensor != null)
                            return new TempDTO() { snID = item.Key, Temperature = item.Value, ROM = sensor.ROM, Name = sensor.Name };
                        break;
                    }
                }
            }

            return new TempDTO() { snID = -1, Temperature = 0.0F, ROM = "N/A", Name = "N/A" };
        }

        public IEnumerable<SensorDTO> SensorsList()
        {
            return sensors.AsEnumerable();
        }

        public SensorDTO SensorInfo(long sensorID)
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

        public virtual string Name => nameof(TempReadingService);
        #endregion

        protected virtual SensorDTO DoGetSensorObjectInfo(long sensorID)
        {
            var sensor = sensors.FirstOrDefault(sn => sn.SensorID == sensorID);

            if (sensor != null)
            {
                return new SensorDTO() { SensorID = (sensor.SensorID), ROM = sensor.ROM, DeviceName = sensor.DeviceName };
            }
            else
            {
                return null;
            }
        }
    }
}
