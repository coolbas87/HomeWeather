using System.Threading.Tasks;

namespace HomeWeather.Domain.Services.Interfaces
{
    public interface ILocationService
    {
        public Task<string> GetLocationNameByCoords(float Lat, float Lon);
    }
}
