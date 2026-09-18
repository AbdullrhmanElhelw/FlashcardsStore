using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FlashcardsStore.Domain.Entities;

namespace FlashcardsStore.Infrastructure.Persistence.Configurations;

public class PaymentProofConfiguration : IEntityTypeConfiguration<PaymentProof>
{
    public void Configure(EntityTypeBuilder<PaymentProof> builder)
    {
        builder.ToTable("PaymentProof");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.OriginalFileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.BankOrWalletName)
            .HasMaxLength(100);

        builder.Property(p => p.SenderPhoneNumber)
            .HasMaxLength(30);

        builder.Property(p => p.TransactionReference)
            .HasMaxLength(100);

        builder.Property(p => p.VerificationNotes)
            .HasMaxLength(500);

        builder.HasOne(p => p.Order)
            .WithOne(o => o.PaymentProof)
            .HasForeignKey<PaymentProof>(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
