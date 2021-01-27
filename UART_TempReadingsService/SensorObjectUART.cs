using OneWireTempLib;
using System.Text.Json.Serialization;

namespace Services.TempReaderModels
{
    public class SensorObjectUART: SensorObject
    {
        private readonly OneWireSensor _physSensor;

        [JsonIgnore]
        public OneWireSensor PhysSensor
        {
            get { return _physSensor; }
        }
        public SensorObjectUART(): base() { }
        public SensorObjectUART(OneWireSensor physSensor): base()
        {
            _physSensor = physSensor;
        }
    }
}
