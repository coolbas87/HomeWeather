using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    public interface IDataBaseOperation
    {
        public IDBSensor GetSensorByROM(string rom);
        public void WriteTempToHistory(long sensorID, float temp, DateTime date);
    }
}
