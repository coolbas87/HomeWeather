using System;
using System.Collections.Generic;
using System.Text;

namespace Services.TempReaderModels
{
    public class CityWeather
    {
        public Main main { get; set; }
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Main
    {
        public float temp { get; set; }
        public float feels_like { get; set; }
    }
}
