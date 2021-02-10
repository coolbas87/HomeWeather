using HomeWeather.Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeWeather.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConnectedSensorsController : ControllerBase
    {
        private readonly ITempReader tempReader;

        public ConnectedSensorsController(ITempReader tempReader)
        {
            this.tempReader = tempReader;
        }

        // GET: api/ConnectedSensors
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(tempReader.SensorsList());
        }

        // GET: api/ConnectedSensors/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(long id)
        {
            return Ok(tempReader.SensorInfo(id));
        }
    }
}
