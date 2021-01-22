using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeWeather.Models
{
    public class DBSensor : IDBSensor
    {
        public long sensorID { get; set; }
        public string Name { get; set; }
        public string ROM { get; set; }
    }
}
