using HomeWeather.Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HomeWeather.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemperatureController : ControllerBase
    {
        private readonly ISensorTempReader tempReader;

        public TemperatureController(ISensorTempReader tempReader)
        {
            this.tempReader = tempReader;
        }

        // GET: api/Temperature
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await tempReader.GetTempAllSensors());
        }

        // GET: api/Temperature/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(long id)
        {
            return Ok(await tempReader.GetTempBySensor(id));
        }
    }
}
