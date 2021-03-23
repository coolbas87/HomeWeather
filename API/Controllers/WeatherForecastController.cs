using HomeWeather.Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HomeWeather.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IWeatherForecastService weatherService;

        public WeatherForecastController(IWeatherForecastService weatherService)
        {
            this.weatherService = weatherService;
        }

        // GET: api/WeatherForecast
        [HttpGet]
        public async Task<IActionResult> GetSensors()
        {
            return Ok(await weatherService.GetForecast());
        }
    }
}
