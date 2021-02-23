using OneWireTempLib;
using System.Text.Json.Serialization;

namespace HomeWeather.Domain.DTO
{
    public class UARTSensorDTO : SensorDTO
    {
        private readonly OneWireSensor _physSensor;

        [JsonIgnore]
        public OneWireSensor PhysSensor
        {
            get { return _physSensor; }
        }
        public UARTSensorDTO() : base() { }
        public UARTSensorDTO(OneWireSensor physSensor) : base()
        {
            _physSensor = physSensor;
        }
    }
}
