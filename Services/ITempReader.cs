using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeWeather.Services
{
    public interface ITempReader
    {
        string Name { get; }
        Task<IEnumerable> LastMeasuredTemp();
        Task<object> LastMeasuredTempBySensor(long ROMInt);
        IEnumerable SensorsList();

        object SensorInfo(long ROMInt);
    }
}
