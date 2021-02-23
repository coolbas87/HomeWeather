using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWeather.Domain.DTO
{
    public class CityWeatherDTO
    {
        public Main Main { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class Main
    {
        public float Temp { get; set; }
        public float Feels_like { get; set; }
    }
}
