using HomeWeather.Data.Entities;
using HomeWeather.Data.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace HomeWeather.Data.Context
{
    public class HomeWeatherContext : DbContext
    {
        public HomeWeatherContext(DbContextOptions<HomeWeatherContext> options) : base(options)
        {
            Database.Migrate();
        }

        public DbSet<TempHistory> TempHistory { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<TelegramBotTrustedUser> TelegramBotTrustedUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SensorConfiguration());
            modelBuilder.ApplyConfiguration(new TempHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new TelegramBotTrustedUserConfiguration());
            modelBuilder.HasSequence<long>("thID");
            modelBuilder.Entity<TempHistory>()
                .Property(th => th.thID)
                .HasDefaultValueSql($"NEXT VALUE FOR thID");

            modelBuilder.HasSequence<long>("snID");
            modelBuilder.Entity<Sensor>()
                .Property(sn => sn.snID)
                .HasDefaultValueSql($"NEXT VALUE FOR snID");
            modelBuilder.Entity<Sensor>()
                .Property(s => s.CreateAt)
                .HasDefaultValueSql("getdate()");

            modelBuilder.HasSequence<long>("tbtuID");
            modelBuilder.Entity<TelegramBotTrustedUser>()
                .Property(tb => tb.tbtuID)
                .HasDefaultValueSql($"NEXT VALUE FOR tbtuID");
        }
    }
}
