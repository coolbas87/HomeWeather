using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWeather.Domain.Configurations
{
    public class OpenWeatherMap
    {
        public string OpenWeatherAPIKey { get; set; }
        public int[] Cities { get; set; }
    }
}
