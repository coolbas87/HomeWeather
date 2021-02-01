using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWeather.Data.Entities
{
    public class TempHistory : IEntity
    {
        public int thID { get; set; }
        public long snID { get; set; }
        public Sensors Sensors { get; set; }
        public float Temperature { get; set; }
        public DateTime Date { get; set; }
    }
}
