using System.Threading.Tasks;

namespace HomeWeather.Domain.Services.Interfaces
{
    public interface IBotService
    {
        public Task StartBot();
        public Task StopBot();
        public bool IsStopped { get; }
    }
}
