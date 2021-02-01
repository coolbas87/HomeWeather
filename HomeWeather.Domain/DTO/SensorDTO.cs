using System.Text.Json.Serialization;

namespace HomeWeather.Domain.DTO
{
    public class SensorDTO
    {
        public long SensorID { get; set; }
        public string ROM { get; set; }
        [JsonIgnore]
        public string Name { get; set; }
        public string DeviceName { get; set; }
        public string Info { get; set; }
    }
}
