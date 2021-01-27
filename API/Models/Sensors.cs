using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HomeWeather.Models
{
    public class Sensors
    {
        [Key]
        [Required]
        public long snID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string ROM { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? EditAt { get; set; }
    }
}
