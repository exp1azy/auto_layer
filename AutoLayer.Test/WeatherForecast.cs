using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoLayer.Test
{
    [Table("weather")]
    public class WeatherForecast
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("date")]
        public DateOnly Date { get; set; }

        [Column("tc")]
        public int TemperatureC { get; set; }

        [Column("tf")]
        public int TemperatureF { get; set; }

        [Column("summary")]
        public string? Summary { get; set; }
    }
}
