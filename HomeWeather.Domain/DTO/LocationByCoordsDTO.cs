using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWeather.Domain.DTO
{
    public class LocationByCoordsDTO
    {
        public int place_id { get; set; }
        public string licence { get; set; }
        public string osm_type { get; set; }
        public int osm_id { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public int place_rank { get; set; }
        public string category { get; set; }
        public string type { get; set; }
        public float importance { get; set; }
        public string addresstype { get; set; }
        public string name { get; set; }
        public string display_name { get; set; }
        public string[] boundingbox { get; set; }
    }
}
