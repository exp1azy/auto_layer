using Microsoft.EntityFrameworkCore;

namespace AutoLayer.Test
{
    public class MyContext : DbContext
    {
        public DbSet<WeatherForecast> Weather { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=test;User Id=postgres;Password=123");
        }
    }
}
