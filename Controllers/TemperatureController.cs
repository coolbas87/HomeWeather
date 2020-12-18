using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWeather.Models;
using HomeWeather.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeWeather.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemperatureController : ControllerBase
    {
        private readonly ITempReader _tempReader;
        private readonly HWDbContext _context;

        public TemperatureController(ITempReader tempReader, HWDbContext context)
        {
            _tempReader = tempReader;
            _context = context;
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
