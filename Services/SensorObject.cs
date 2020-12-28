using OneWireTempLib;
using System.Text.Json.Serialization;

namespace HomeWeather.Services
{
    internal class SensorObject
    {
        private OneWireSensor _physSensor;
        public long SensorID { get; set; }
        public string ROM { get; set; }
        [JsonIgnore]
        public string Name { get; set; }
        public string DeviceName { get; set; }
        [JsonIgnore]
        public OneWireSensor PhysSensor
        {
            get { return _physSensor; }
        }
        public SensorObject() { }
        public SensorObject(OneWireSensor physSensor)
        {
            _physSensor = physSensor;
        }
    }
}
