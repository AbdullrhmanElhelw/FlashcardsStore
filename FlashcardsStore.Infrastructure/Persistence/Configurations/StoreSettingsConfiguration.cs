using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FlashcardsStore.Domain.Entities;

namespace FlashcardsStore.Infrastructure.Persistence.Configurations;

public class StoreSettingsConfiguration : IEntityTypeConfiguration<StoreSettings>
{
    public void Configure(EntityTypeBuilder<StoreSettings> builder)
    {
        builder.ToTable("StoreSettings");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.StoreNameAr)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.StoreNameEn)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.WhatsAppNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.ContactPhone)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.ContactEmail)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(s => s.StoreAddressAr)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(s => s.StoreAddressEn)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(s => s.VodafoneCashNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.InstaPayAddress)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.InstaPayAccountName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(s => s.ShippingFeeCairoGiza)
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.ShippingFeeDeltaCanal)
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.ShippingFeeUpperEgypt)
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.FreeShippingThreshold)
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.EstimatedDeliveryTimeAr)
            .HasMaxLength(200);

        builder.Property(s => s.AnnouncementTextAr)
            .HasMaxLength(500);

        builder.Property(s => s.AnnouncementTextEn)
            .HasMaxLength(500);

        builder.Property(s => s.AnnouncementLink)
            .HasMaxLength(500);
    }
}
