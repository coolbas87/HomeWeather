using System.Text.Json.Serialization;

namespace Services.TempReaderModels
{
    public class SensorObject
    {
        public long SensorID { get; set; }
        public string ROM { get; set; }
        [JsonIgnore]
        public string Name { get; set; }
        public string DeviceName { get; set; }
    }
}
