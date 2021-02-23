using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWeather.Data.Entities;
using HomeWeather.Domain.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeWeather.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TempHistoryController : ControllerBase
    {
        private readonly ITempHistoryService tempHistoryService;

        public TempHistoryController(ITempHistoryService tempHistoryService)
        {
            this.tempHistoryService = tempHistoryService;
        }

        // GET: api/TempHistory/2020-01-01/2020-01-02
        [HttpGet("{from:DateTime}/{to:DateTime}")]
        public IActionResult GetTempHistory(DateTime from, DateTime to)
        {
            return Ok(tempHistoryService.GetTempHistory(from, to));
        }

        // GET: api/TempHistory/5/2020-01-01/2020-01-02
        [HttpGet("{id}/{from:DateTime}/{to:DateTime}")]
        public IActionResult GetTempHistory(long id, DateTime from, DateTime to)
        {
            return Ok(tempHistoryService.GetTempHistoryBySensor(id, from, to));
        }
    }
}
