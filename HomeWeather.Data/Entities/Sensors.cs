using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWeather.Data.Entities
{
    public class Sensors : IEntity
    {
        public long snID { get; set; }
        public string Name { get; set; }
        public string ROM { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime EditAt { get; set; }
    }
}
