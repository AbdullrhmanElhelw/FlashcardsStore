using FlashcardsStore.Domain.Common;

namespace FlashcardsStore.Domain.Entities;

public class PaymentProof : BaseEntity
{
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public string? BankOrWalletName { get; set; }
    public string? SenderPhoneNumber { get; set; }
    public string? TransactionReference { get; set; }
    public bool IsVerified { get; set; } = false;
    public DateTime? VerifiedAt { get; set; }
    public string? VerificationNotes { get; set; }
}
