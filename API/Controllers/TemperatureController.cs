using Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HomeWeather.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemperatureController : ControllerBase
    {
        private readonly ITempReader _tempReader;

        public TemperatureController(ITempReader tempReader)
        {
            _tempReader = tempReader;
        }

        // GET: api/Temperature
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _tempReader.LastMeasuredTemp());
        }

        // GET: api/Temperature/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(long id)
        {
            return Ok(await _tempReader.LastMeasuredTempBySensor(id));
        }
    }
}
