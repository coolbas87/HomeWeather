using HomeWeather.Data.Entities;
using HomeWeather.Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeWeather.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        private readonly ISensorService sensorService;

        public SensorsController(ISensorService sensorService)
        {
            this.sensorService = sensorService;
        }

        // GET: api/Sensors
        [HttpGet]
        public IActionResult GetSensors()
        {
            return Ok(sensorService.GetSensors());
        }

        // GET: api/Sensors/5
        [HttpGet("{id}")]
        public IActionResult GetSensors(long id)
        {
            return Ok(sensorService.GetSensorById(id));
        }

        //POST: api/Sensors
        //To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public IActionResult PostSensors(Sensor sensor)
        {
            return Ok(sensorService.AddSensor(sensor));
        }

        // PUT: api/Sensors/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public IActionResult PutSensors(Sensor sensor)
        {
            return Ok(sensorService.UpdateSensor(sensor));
        }

        // DELETE: api/Sensors/5
        [HttpDelete("{id}")]
        public IActionResult DeleteSensors(long id)
        {
            return Ok(sensorService.DeleteSensor(id));
        }
    }
}