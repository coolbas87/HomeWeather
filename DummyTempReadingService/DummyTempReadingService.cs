using Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.TempReaderModels;
using System;
using System.Threading;

namespace Services.Service
{
    public class DummyTempReadingService : TempReadingService
    {
        public DummyTempReadingService(ILogger<DummyTempReadingService> logger, IDataBaseOperation dataBase, IOptions<Settings> options) : base(logger, dataBase, options)
        { }

        protected override void DoStartAsync(CancellationToken stoppingToken)
        {
            for (int i = 0; i <= 1; i++)
            {
                int rndROM = 111111111 + i;
                var dbSensor = DataBase.GetSensorByROM(rndROM.ToString());

                if (dbSensor == null)
                    Sensors.Add(new SensorObject() { SensorID = rndROM, Name = $"Dummy sensor {i}", ROM = rndROM.ToString(), DeviceName = $"Sensor {rndROM}" } );
                else
                    Sensors.Add(new SensorObject() { SensorID = dbSensor.sensorID, Name = dbSensor.Name, ROM = dbSensor.ROM, DeviceName = $"Sensor {rndROM}" });
            }
        }

        protected override void ReadTemperature()
        {
            Random rnd = new Random();

            foreach (SensorObject sensor in Sensors)
            {
                float temp = (float)(rnd.Next(-35, 39) + rnd.NextDouble());

                AddValueToTempCache((id: sensor.SensorID, temperature: temp));
            }
        }

        public override string Name => nameof(DummyTempReadingService);
    }
}
