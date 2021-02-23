using HomeWeather.Data.Entities;
using HomeWeather.Data.Interfaces;
using HomeWeather.Domain.Configurations;
using HomeWeather.Domain.DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneWireTempLib;
using System.Collections.Generic;
using System.Linq;

namespace HomeWeather.Domain.Services.Implementation
{
    public class UART_TempReadingService : TempReadingService
    {
        private UART_Adapter uart;
        private readonly UARTSettings UARTSettings;

        public UART_TempReadingService(
            ILogger<TempReadingService> logger,
            IServiceScopeFactory scopeFactory,
            IOptions<TempServiceSettings> options,
            IOptions<UARTSettings> UARTSettings) : base(logger, scopeFactory, options) 
        {
            this.UARTSettings = UARTSettings.Value;
        }

        protected override void DoStartAsync(object stoppingToken)
        {
            uart = new UART_Adapter(UARTSettings.COMPort);
            uart.Open();
            Logger.LogInformation($"UART is now listennig port {UARTSettings.COMPort}");
            OneWireSensor sensor = new DS18B20(uart);
            List<byte[]> ROMs = sensor.GetConnectedROMs();
            Logger.LogInformation($"{ROMs.Count} connected sensor(s)");

            foreach (byte[] item in ROMs)
            {
                OneWireSensor physSensor = Utils.CreateSensor(item[0], uart, item);
                using (var scope = ScopeFactory.CreateScope())
                {
                    IUnitOfWork<Sensor> sensorsUnitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<Sensor>>();
                    var dbSensor = sensorsUnitOfWork.GetRepository().Query().FirstOrDefault(sn => sn.ROM == physSensor.ROM);
                    if (dbSensor == null)
                        Sensors.Add(new UARTSensorDTO(physSensor) { SensorID = (Sensors.Count + 1) * -1, Name = "Not in DB", ROM = physSensor.ROM, DeviceName = physSensor.DeviceName(physSensor.FamilyCode) });
                    else
                        Sensors.Add(new UARTSensorDTO(physSensor) { SensorID = dbSensor.snID, Name = dbSensor.Name, ROM = physSensor.ROM, DeviceName = physSensor.DeviceName(physSensor.FamilyCode) });
                    Logger.LogInformation($"Added to list sensor with ROM: {physSensor.ROM}, Family Code = {physSensor.FamilyCode}");
                }
            }
        }

        protected override void DoStopAsync(object stoppingToken)
        {
            uart?.Close();
            Logger.LogInformation($"UART stoped listennig port {UARTSettings.COMPort}");
        }

        protected override void ReadTemperature()
        {
            if (uart.IsOpened)
            {
                foreach (UARTSensorDTO sensor in Sensors)
                {
                    AddValueToTempCache((id: sensor.SensorID, temperature: sensor.PhysSensor.GetTemperature()));
                }
            }
        }

        protected override SensorDTO DoGetSensorObjectInfo(long sensorID)
        {
            var sensor = Sensors.FirstOrDefault(sn => sn.SensorID == sensorID);

            if ((sensor != null) && (sensor is UARTSensorDTO))
            {
                return new SensorDTO() { SensorID = (sensor.SensorID), ROM = sensor.ROM, DeviceName = sensor.DeviceName, Info = ((UARTSensorDTO)sensor).PhysSensor.Info() };
            }
            else
            {
                return null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                uart?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
