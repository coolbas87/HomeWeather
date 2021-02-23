using HomeWeather.Data.Entities;
using HomeWeather.Data.Interfaces;
using HomeWeather.Domain.DTO;
using HomeWeather.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;

namespace HomeWeather.Domain.Services.Implementation
{
    public class SensorService : ISensorService
    {
        private readonly IUnitOfWork<Sensor> unitOfWork;

        public SensorService(IUnitOfWork<Sensor> unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Sensor AddSensor(SensorDTO sensorDto)
        {
            var newSensor = new Sensor
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
            var entity = unitOfWork.GetRepository().GetById(sensorId);

            if (entity == null)
            {
                throw new Exceptions.AppException($"This entity id - {sensorId} wasn't found")
                {
                    StatusCode = (int)HttpStatusCode.NotFound
                };
            }

            return unitOfWork.GetRepository().Delete(entity);
        }

        public Sensor GetSensorById(long sensorId)
        {
            return unitOfWork.GetRepository().GetById(sensorId);
        }

        public IEnumerable<Sensor> GetSensors()
        {
            return unitOfWork.GetRepository().Query();
        }

        public Sensor UpdateSensor(SensorDTO sensorDto)
        {
            if (sensorDto == null)
            {
                throw new Exceptions.AppException("Received sensor is null")
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }

            var entity = unitOfWork.GetRepository().GetById(sensorDto.SensorID);

            if (entity == null)
            {
                throw new Exceptions.AppException($"This entity id - {sensorDto.SensorID} wasn't found")
                {
                    StatusCode = (int)HttpStatusCode.NotFound
                };
            }

            entity.Name = sensorDto.Name;
            entity.ROM = sensorDto.ROM;
            entity.EditAt = DateTime.Now;

            var newEntity = unitOfWork.GetRepository().Update(entity);

            unitOfWork.SaveChanges();
            return newEntity;
        }
    }
}
