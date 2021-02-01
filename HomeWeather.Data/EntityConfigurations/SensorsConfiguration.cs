using HomeWeather.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeWeather.Data.Context
{
    public class SensorsConfiguration : IEntityTypeConfiguration<Sensors>
    {
        public void Configure(EntityTypeBuilder<Sensors> builder)
        {
            builder.HasKey(sensor => sensor.snID);

            builder.Property(sensor => sensor.Name).HasMaxLength(50).IsRequired();
            builder.Property(sensor => sensor.ROM).HasMaxLength(100).IsRequired();
        }
    }
}