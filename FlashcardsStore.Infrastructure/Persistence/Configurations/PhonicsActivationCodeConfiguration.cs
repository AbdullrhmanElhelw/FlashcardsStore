using FlashcardsStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashcardsStore.Infrastructure.Persistence.Configurations;

public class PhonicsActivationCodeConfiguration : IEntityTypeConfiguration<PhonicsActivationCode>
{
    public void Configure(EntityTypeBuilder<PhonicsActivationCode> builder)
    {
        builder.ToTable("PhonicsActivationCode");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(32);

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.BundleName)
            .HasMaxLength(150);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.MaxDevices)
            .HasDefaultValue(2);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.HasMany(x => x.Devices)
            .WithOne(d => d.ActivationCode)
            .HasForeignKey(d => d.PhonicsActivationCodeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
