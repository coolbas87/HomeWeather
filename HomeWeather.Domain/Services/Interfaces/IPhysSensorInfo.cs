using HomeWeather.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeWeather.Domain.Services.Interfaces
{
    public interface IPhysSensorInfo
    {
        Task<IEnumerable<SensorDTO>> GetSensors();
        Task<SensorDTO> GetSensorByID(long sensorID);
    }
}
