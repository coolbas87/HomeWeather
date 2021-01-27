using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    public interface IDBSensor
    {
        long sensorID { get; set; }
        string Name { get; set; }
        string ROM { get; set; }
    }
}
