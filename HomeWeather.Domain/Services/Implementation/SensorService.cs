using HomeWeather.Data.Entities;
using HomeWeather.Data.Interfaces;
using HomeWeather.Domain.DTO;
using HomeWeather.Domain.Services.Interfaces;
using System.Collections.Generic;

namespace HomeWeather.Domain.Services.Implementation
{
    public class SensorService : ISensorService
    {
        private readonly IUnitOfWork<Sensors> unitOfWork;

        public SensorService(IUnitOfWork<Sensors> unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Sensors GetSensorById(long sensorId)
        {
            return unitOfWork.GetRepository().GetById(sensorId);
        }

        public Sensors AddSensor(SensorDTO sensorDto)
        {
            var newSensor = new Sensors
            {
                Name = sensorDto.Name,
                ROM = sensorDto.ROM
            };

            var sensor = unitOfWork.GetRepository().Add(newSensor);
            unitOfWork.SaveChanges();

            return sensor;
        }

        public bool DeleteSensor(long sensorId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Sensors> GetSensors()
        {
            return unitOfWork.GetRepository().Query();
        }

        public Sensors UpdateSensor(SensorDTO sensorDto)
        {
            throw new System.NotImplementedException();
        }
    }
}
