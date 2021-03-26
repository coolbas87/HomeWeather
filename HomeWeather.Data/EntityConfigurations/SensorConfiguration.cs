using HomeWeather.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeWeather.Data.EntityConfigurations
{
    public class SensorConfiguration : IEntityTypeConfiguration<Sensor>
    {
        public void Configure(EntityTypeBuilder<Sensor> builder)
        {
            builder.HasKey(sensor => sensor.snID);

            builder.Property(sensor => sensor.Name).HasMaxLength(50).IsRequired();
            builder.Property(sensor => sensor.ROM).HasMaxLength(100).IsRequired();
        }
    }
}