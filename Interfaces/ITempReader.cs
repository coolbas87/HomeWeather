using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface ITempReader
    {
        string Name { get; }
        Task<IEnumerable> LastMeasuredTemp();
        Task<object> LastMeasuredTempBySensor(long snID);
        IEnumerable SensorsList();
        object SensorInfo(long sensorID);
    }
}
