using System.Text.Json.Serialization;

namespace HomeWeather.Domain.DTO.OpenWeatherMap
{
    public class Rain
    {
        [JsonPropertyName("1h")]
        public float volLast1Hour { get; set; }
    }
}
