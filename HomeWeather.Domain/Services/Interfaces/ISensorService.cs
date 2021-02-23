﻿using HomeWeather.Data.Entities;
using HomeWeather.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWeather.Domain.Services.Interfaces
{
    public interface ISensorService
    {
        IEnumerable<Sensor> GetSensors();
        Sensor GetSensorById(long sensorId);
        Sensor AddSensor(SensorDTO sensorDto);
        Sensor UpdateSensor(SensorDTO sensorDto);
        bool DeleteSensor(long sensorId);
    }
}
