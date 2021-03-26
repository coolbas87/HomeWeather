using HomeWeather.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeWeather.Data.EntityConfigurations
{
    public class TelegramBotTrustedUserConfiguration : IEntityTypeConfiguration<TelegramBotTrustedUser>
    {
        public void Configure(EntityTypeBuilder<TelegramBotTrustedUser> builder)
        {
            builder.HasKey(trustedUser => trustedUser.tbtuID);

            builder.Property(trustedUser => trustedUser.FirstName).HasMaxLength(64).IsRequired();
            builder.Property(trustedUser => trustedUser.Username).HasMaxLength(32);
            builder.Property(trustedUser => trustedUser.userID).IsRequired();
            builder.HasIndex(trustedUser => trustedUser.userID).IsUnique();
        }
    }
}
