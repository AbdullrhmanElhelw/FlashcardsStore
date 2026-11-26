using FlashcardsStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashcardsStore.Infrastructure.Persistence.Configurations;

public class PhonicsDeviceActivationConfiguration : IEntityTypeConfiguration<PhonicsDeviceActivation>
{
    public void Configure(EntityTypeBuilder<PhonicsDeviceActivation> builder)
    {
        builder.ToTable("PhonicsDeviceActivation");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DeviceId)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.DeviceName)
            .HasMaxLength(128);

        builder.HasIndex(x => new { x.PhonicsActivationCodeId, x.DeviceId })
            .IsUnique();
    }
}
