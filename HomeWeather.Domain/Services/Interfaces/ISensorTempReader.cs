using HomeWeather.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeWeather.Domain.Services.Interfaces
{
    public interface ISensorTempReader
    {
        Task<IEnumerable<TempDTO>> GetTempAllSensors();
        Task<TempDTO> GetTempBySensor(long snID);
    }
}
