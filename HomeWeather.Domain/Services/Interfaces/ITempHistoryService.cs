using HomeWeather.Data.Entities;
using System;
using System.Collections.Generic;

namespace HomeWeather.Domain.Services.Interfaces
{
    public interface ITempHistoryService
    {
        public IEnumerable<TempHistory> GetTempHistory(DateTime from, DateTime to);
        public IEnumerable<TempHistory> GetTempHistoryBySensor(long sensorId, DateTime from, DateTime to);
    }
}
