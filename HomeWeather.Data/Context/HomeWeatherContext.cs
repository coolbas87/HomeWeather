using HomeWeather.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeWeather.Data.Context
{
    public class HomeWeatherContext : DbContext
    {
        //public HWDbContext(DbContextOptions<HWDbContext> options) : base(options)
        //{
        //    Database.Migrate();
        //}

        public DbSet<TempHistory> TempHistory { get; set; }
        public DbSet<Sensors> Sensors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SensorsConfiguration());
            modelBuilder.ApplyConfiguration(new TempHistoryConfiguration());
            //modelBuilder.HasSequence<int>("thID");
            //modelBuilder.Entity<TempHistory>()
            //    .Property(th => th.thID)
            //    .HasDefaultValueSql($"NEXT VALUE FOR thID");

            //modelBuilder.HasSequence<int>("snID");
            //modelBuilder.Entity<Sensors>()
            //    .Property(sn => sn.snID)
            //    .HasDefaultValueSql($"NEXT VALUE FOR snID");
            //modelBuilder.Entity<Sensors>()
            //    .Property(s => s.CreateAt)
            //    .HasDefaultValueSql("getdate()");
        }
    }
}
