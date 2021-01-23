using System;
using System.Collections.Generic;
using System.Text;

namespace Services.TempReaderModels
{
    public class CityWeather
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
