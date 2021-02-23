using HomeWeather.Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HomeWeather.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConnectedSensorsController : ControllerBase
    {
        private readonly IPhysSensorInfo sensorInfo;

        public ConnectedSensorsController(IPhysSensorInfo sensorInfo)
        {
            this.sensorInfo = sensorInfo;
        }

        // GET: api/ConnectedSensors
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await sensorInfo.GetSensors());
        }

        // GET: api/ConnectedSensors/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> Get(long id)
        {
            return Ok(await sensorInfo.GetSensorByID(id));
        }
    }
}
