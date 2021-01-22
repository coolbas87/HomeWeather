using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWeather.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeWeather.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        private readonly HWDbContext _context;

        public SensorsController(HWDbContext context)
        {
            _context = context;
        }

        // GET: api/Sensors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sensors>>> GetSensors()
        {
            return await _context.Sensors.ToListAsync();
        }

        // GET: api/Sensors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sensors>> GetSensors(long id)
        {
            var sensors = await _context.Sensors.FindAsync(id);

            if (sensors == null)
            {
                return NotFound();
            }

            return sensors;
        }

        // POST: api/Sensors
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Sensors>> PostSensors(Sensors sensors)
        {
            _context.Sensors.Add(sensors);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSensors", new { id = sensors.snID }, sensors);
        }

        // PUT: api/Sensors/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSensors(long id, Sensors sensors)
        {
            if (id != sensors.snID)
            {
                return BadRequest();
            }

            _context.Entry(sensors).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SensorsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Sensors/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Sensors>> DeleteSensors(long id)
        {
            var sensors = await _context.Sensors.FindAsync(id);
            if (sensors == null)
            {
                return NotFound();
            }

            _context.Sensors.Remove(sensors);
            await _context.SaveChangesAsync();

            return sensors;
        }

        private bool SensorsExists(long id)
        {
            return _context.Sensors.Any(e => e.snID == id);
        }
    }
}