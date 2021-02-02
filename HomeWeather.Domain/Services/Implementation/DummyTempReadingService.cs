using HomeWeather.Data.Entities;
using HomeWeather.Data.Interfaces;
using HomeWeather.Domain.Configurations;
using HomeWeather.Domain.DTO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace HomeWeather.Domain.Services.Implementation
{
    public class DummyTempReadingService : TempReadingService
    {
        public DummyTempReadingService(
            ILogger<TempReadingService> logger,
            IUnitOfWork<Sensors> sensorsUnitOfWork,
            IUnitOfWork<TempHistory> tempHistUnitOfWork,
            IOptions<TempService> options) : base(logger, sensorsUnitOfWork, tempHistUnitOfWork, options) { }

        protected override void DoStartAsync(object stoppingToken)
        {
            for (int i = 0; i <= 1; i++)
            {
                int rndROM = 111111111 + i;
                var dbSensor = SensorsUnitOfWork.GetRepository().Query().FirstOrDefault(sn => sn.ROM == rndROM.ToString());

                if (dbSensor == null)
                    Sensors.Add(new SensorDTO() { SensorID = rndROM, Name = $"Dummy sensor {i}", ROM = rndROM.ToString(), DeviceName = $"Sensor {rndROM}" });
                else
                    Sensors.Add(new SensorDTO() { SensorID = dbSensor.snID, Name = dbSensor.Name, ROM = dbSensor.ROM, DeviceName = $"Sensor {rndROM}" });
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

        public override string Name => nameof(DummyTempReadingService);
    }
}
