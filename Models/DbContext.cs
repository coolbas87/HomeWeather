using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeWeather.Models
{
    public class HWDbContext : DbContext
    {
        public HWDbContext(DbContextOptions<HWDbContext> options): base(options)
        { }

        public DbSet<TempHistory> TempHistory { get; set; }
        public DbSet<Sensors> Sensors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasSequence<int>("thID");
            modelBuilder.Entity<TempHistory>()
                .Property(th => th.thID)
                .HasDefaultValueSql($"NEXT VALUE FOR thID");

            modelBuilder.Entity<Sensors>()
                .Property(s => s.CreateAt)
                .HasDefaultValueSql("getdate()");
        }
    }
}
