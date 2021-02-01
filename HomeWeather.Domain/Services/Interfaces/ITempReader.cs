using HomeWeather.Domain.DTO;
using System.Collections.Generic;

namespace HomeWeather.Domain.Services.Interfaces
{
    public interface ITempReader
    {
        string Name { get; }
        IEnumerable<TempDTO> LastMeasuredTemp();
        TempDTO LastMeasuredTempBySensor(long snID);
        IEnumerable<SensorDTO> SensorsList();
        SensorDTO SensorInfo(long sensorID);
    }
}
