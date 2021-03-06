﻿namespace HomeWeather.Domain.DTO
{
    public class CityWeatherDTO
    {
        public Coord coord { get; set; }
        public Main main { get; set; }
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Main
    {
        public float temp { get; set; }
        public float feels_like { get; set; }
    }

    public class Coord
    {
        public float lat { get; init; }
        public float lon { get; init; }
    }
}
