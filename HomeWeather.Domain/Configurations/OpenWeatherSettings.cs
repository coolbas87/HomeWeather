namespace HomeWeather.Domain.Configurations
{
    public class OpenWeatherSettings
    {
        public string OpenWeatherAPIKey { get; set; }
        public int[] Cities { get; set; }
        public float Lat { get; set; }
        public float Lon { get; set; }
    }
}
