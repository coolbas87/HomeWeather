using HomeWeather.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeWeather.Data.Context
{
    internal class TempHistoryConfiguration : IEntityTypeConfiguration<TempHistory>
    {
        public void Configure(EntityTypeBuilder<TempHistory> builder)
        {
            builder.HasKey(tempHist => tempHist.thID);

            builder.HasOne(tempHist => tempHist.Sensors)
                .WithMany()
                .HasForeignKey(tempHist => tempHist.snID)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(tempHist => tempHist.Temperature).IsRequired();
            builder.Property(tempHist => tempHist.Date).IsRequired();
        }
    }
}