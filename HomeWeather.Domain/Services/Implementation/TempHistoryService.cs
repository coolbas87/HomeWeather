using HomeWeather.Data.Entities;
using HomeWeather.Data.Interfaces;
using HomeWeather.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeWeather.Domain.Services.Implementation
{
    public class TempHistoryService : ITempHistoryService
    {
        private readonly IUnitOfWork<TempHistory> unitOfWork;

        public TempHistoryService(IUnitOfWork<TempHistory> unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IEnumerable<TempHistory> GetTempHistory(DateTime from, DateTime to)
        {
            var history = unitOfWork.GetRepository()
                .Query(
                    sensor => sensor.Sensors
                    )
                .Where(tempHistory => (tempHistory.Date >= from) && (tempHistory.Date < to.AddDays(1)))
                .OrderByDescending(x => x.Date)
                .ToList();
            return history;
        }

        public IEnumerable<TempHistory> GetTempHistoryBySensor(long sensorId, DateTime from, DateTime to)
        {
            var history = unitOfWork.GetRepository()
                .Query(
                    sensor => sensor.Sensors
                    )
                .Where(tempHistory => (tempHistory.snID == sensorId) && (tempHistory.Date >= from) && (tempHistory.Date < to.AddDays(1)))
                .OrderByDescending(x => x.Date)
                .ToList();
            return history;
        }
    }
}
