using HomeWeather.Models;
using Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeWeather.Services
{
    public class DatabaseOperations : IDataBaseOperation
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DatabaseOperations(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public IDBSensor GetSensorByROM(string rom)
        {            
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<HWDbContext>();
                var sensor = dbContext.Sensors.FirstOrDefault(sn => sn.ROM == rom);

                if (sensor == null)
                {
                    return null;
                }
                else
                {
                    return new DBSensor() { sensorID = sensor.snID, Name = sensor.Name, ROM = sensor.ROM };
                }

            }
        }

        public void WriteTempToHistory(long sensorID, float temp, DateTime date)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<HWDbContext>();
                var dbSensor = dbContext.Sensors.Find(sensorID);

                if (dbSensor != null)
                {
                    dbContext.TempHistory.Add(new TempHistory
                    {
                        snID = sensorID,
                        Temperature = temp,
                        Date = date
                    });
                    dbContext.SaveChanges();
                }
            }
        }
    }
}
