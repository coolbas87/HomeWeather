using HomeWeather.Domain.Configurations;
using HomeWeather.Domain.DTO;
using HomeWeather.Domain.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HomeWeather.Domain.Services.Implementation
{
    public class LocationService : ILocationService
    {
        private readonly HttpClient client = new HttpClient();
        private readonly ILogger logger;
        private readonly OpenWeatherSettings settings;

        public LocationService(ILogger<LocationService> logger, IOptions<OpenWeatherSettings> options)
        {
            client.BaseAddress = new Uri("http://nominatim.openstreetmap.org/");
            this.logger = logger;
            settings = options.Value;
        }

        private async Task<T> MakeRequest<T>(string uri)
        {
            T reqObject = default(T);

            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStreamAsync();
                reqObject = await JsonSerializer.DeserializeAsync<T>(responseBody);
            }
            catch (HttpRequestException e)
            {
                logger.LogError("\nException Caught!");
                logger.LogError("\nURI: {0}{1} ", client.BaseAddress, uri);
                logger.LogError("\nMessage: {0} ", e.Message);
            }

            return reqObject;
        }

        public async Task<string> GetLocationNameByCoords(float Lat, float Lon)
        {
            string uri = $"reverse?format=jsonv2&email={settings.EmailForRequest}&accept-language=en-US&zoom=10&addressdetails=0&lat={Lat.ToString().Replace(',', '.')}&lon={Lon.ToString().Replace(',', '.')}";

            LocationByCoordsDTO response = await MakeRequest<LocationByCoordsDTO>(uri);

            if (response != null)
            {
                return response.display_name;
            }
            else
            {
                return "";
            }
        }
    }
}
