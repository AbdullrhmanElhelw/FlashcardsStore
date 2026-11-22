using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FlashcardsStore.Domain.Entities;

namespace FlashcardsStore.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customer");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.PhoneNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasIndex(c => c.PhoneNumber);

        builder.Property(c => c.WhatsAppNumber)
            .HasMaxLength(30);

        builder.Property(c => c.Email)
            .HasMaxLength(150);

        builder.Property(c => c.PasswordHash)
            .HasMaxLength(256);

        builder.Property(c => c.ShippingAddress)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Governorate)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.CustomerType)
            .IsRequired()
            .HasConversion<int>();
    }
}
