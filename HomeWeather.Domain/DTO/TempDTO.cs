using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWeather.Domain.DTO
{
    public class TempDTO
    {
        public long snID { get; set; }
        public float Temperature { get; set; }
        public string ROM { get; set; }
        public string Name { get; set; }
    }
}
