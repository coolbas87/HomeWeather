using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeWeather.Models;

namespace HomeWeather.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TempHistoryController : ControllerBase
    {
        private readonly HWDbContext _context;

        public TempHistoryController(HWDbContext context)
        {
            _context = context;
        }

        // GET: api/TempHistory/2020-01-01/2020-01-02
        [HttpGet("{from:DateTime}/{to:DateTime}")]
        public async Task<ActionResult<IEnumerable<TempHistory>>> GetTempHistory(DateTime from, DateTime to)
        {
            var tempHistory = await _context.TempHistory.Include(th => th.Sensors).AsQueryable().Where(th => ((th.Date >= from) && (th.Date < to.AddDays(1)))).ToListAsync();

            if (tempHistory == null)
            {
                return NotFound();
            }

            return tempHistory;
        }

        // GET: api/TempHistory/5/2020-01-01/2020-01-02
        [HttpGet("{id}/{from:DateTime}/{to:DateTime}")]
        public async Task<ActionResult<IEnumerable<TempHistory>>> GetTempHistory(long id, DateTime from, DateTime to)
        {
            var tempHistory = await _context.TempHistory.Include(th => th.Sensors).AsQueryable().Where(th => ((th.snID == id) && (th.Date >= from) && (th.Date < to.AddDays(1)))).ToListAsync();

            if (tempHistory == null)
            {
                return NotFound();
            }

            return tempHistory;
        }
    }
}
