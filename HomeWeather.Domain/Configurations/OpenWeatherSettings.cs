namespace HomeWeather.Domain.Configurations
{
    public class OpenWeatherSettings
    {
        public string OpenWeatherAPIKey { get; set; }
        public int[] Cities { get; set; }
        public int ForecastCityID { get; set; }
        public string EmailForRequest { get; set; }
    }
}
