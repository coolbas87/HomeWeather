using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWeather.Services;
using Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeWeather.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConnectedSensorsController : ControllerBase
    {
        private readonly ITempReader _tempReader;

        public ConnectedSensorsController(ITempReader tempReader)
        {
            _tempReader = tempReader;
        }

        // GET: api/ConnectedSensors
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_tempReader.SensorsList());
        }

        // GET: api/ConnectedSensors/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(long id)
        {
            return Ok(_tempReader.SensorInfo(id));
        }
    }
}
