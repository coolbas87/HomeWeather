using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HomeWeather.Models
{
    public class TempHistory
    {
        [Key]
        [Required]
        public int thID { get; set; }
        [Required]
        public long snID { get; set; }
        [ForeignKey("snID")]
        public Sensors Sensors { get; set; }
        [Required]
        public float Temperature { get; set; }
        public DateTime Date { get; set; }
    }
}
