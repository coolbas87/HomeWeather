using HomeWeather.Data.Entities;
using HomeWeather.Data.Interfaces;
using HomeWeather.Domain.Configurations;
using HomeWeather.Domain.DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace HomeWeather.Domain.Services.Implementation
{
    public class DummyTempReadingService : TempReadingService
    {
        private readonly int sensorsCount = 2;

        public DummyTempReadingService(
            ILogger<TempReadingService> logger,
            IServiceScopeFactory scopeFactory,
            IOptions<TempServiceSettings> options) : base(logger, scopeFactory, options) { }

        protected override void DoStartAsync(object stoppingToken)
        {
            Logger.LogInformation($"{sensorsCount} connected sensor(s)");

            for (int i = 0; i < sensorsCount; i++)
            {
                int rndROM = 111111111 + i;
                using (var scope = ScopeFactory.CreateScope())
                {
                    IUnitOfWork<Sensor> sensorsUnitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<Sensor>>();
                    var dbSensor = sensorsUnitOfWork.GetRepository().Query().FirstOrDefault(sn => sn.ROM == rndROM.ToString());

                    if (dbSensor == null)
                        Sensors.Add(new SensorDTO() { SensorID = rndROM, Name = $"Dummy sensor {i}", ROM = rndROM.ToString(), DeviceName = $"Sensor {rndROM}" });
                    else
                        Sensors.Add(new SensorDTO() { SensorID = dbSensor.snID, Name = dbSensor.Name, ROM = dbSensor.ROM, DeviceName = $"Sensor {rndROM}" });
                    Logger.LogInformation($"Added to list sensor with ROM: {rndROM}");
                }
            }
        }

        protected override void ReadTemperature()
        {
            Random rnd = new Random();

            foreach (SensorDTO sensor in Sensors)
            {
                float temp = (float)(rnd.Next(-35, 39) + rnd.NextDouble());

                AddValueToTempCache((id: sensor.SensorID, temperature: temp));
            }
        }
    }
}
