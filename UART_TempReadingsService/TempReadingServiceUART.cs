using Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneWireTempLib;
using Services.TempReaderModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Services.Service
{
    public partial class TempReadingServiceUART : TempReadingService
    {
        private UART_Adapter uart;

        public TempReadingServiceUART(ILogger<TempReadingServiceUART> logger, IDataBaseOperation dataBase, IOptions<Settings> options) : base(logger, dataBase, options)
        { }

        protected override void DoStartAsync(CancellationToken stoppingToken)
        {
            uart = new UART_Adapter(Settings.COMPort);
            uart.Open();
            OneWireSensor sensor = new DS18B20(uart);
            List<byte[]> ROMs = sensor.GetConnectedROMs();

            foreach (byte[] item in ROMs)
            {
                OneWireSensor physSensor = Utils.CreateSensor(item[0], uart, item);
                var dbSensor = DataBase.GetSensorByROM(physSensor.ROM);
                if (dbSensor == null)
                    Sensors.Add(new SensorObjectUART(physSensor) { SensorID = (Sensors.Count + 1) * -1, Name = "Not in DB", ROM = physSensor.ROM, DeviceName = physSensor.DeviceName(physSensor.FamilyCode) });
                else
                    Sensors.Add(new SensorObjectUART(physSensor) { SensorID = dbSensor.sensorID, Name = dbSensor.Name, ROM = physSensor.ROM, DeviceName = physSensor.DeviceName(physSensor.FamilyCode) });
            }
        }

        protected override void DoStopAsync(CancellationToken stoppingToken)
        {
            uart?.Close();
        }

        protected override void ReadTemperature()
        {
            if (uart.IsOpened)
            {
                foreach (SensorObjectUART sensor in Sensors)
                {
                    AddValueToTempCache((id: sensor.SensorID, temperature: sensor.PhysSensor.GetTemperature()));
                }
            }
        }

        protected override object DoGetSensorObjectInfo(long sensorID)
        {
            var sensor = Sensors.FirstOrDefault(sn => sn.SensorID == sensorID);

            if ((sensor != null) && (sensor is SensorObjectUART))
            {
                return new SensorInfoObj() { SensorID = (sensor.SensorID), ROM = sensor.ROM, DeviceName = sensor.DeviceName, Info = ((SensorObjectUART)sensor).PhysSensor.Info() };
            }
            else
            {
                return null;
            }
        }

        public override void Dispose()
        {
            uart?.Dispose();
            base.Dispose();
        }

        public override string Name => nameof(TempReadingServiceUART);
    }
}
