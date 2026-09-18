namespace FlashcardsStore.Application.DTOs.PaymentProof;

public class PaymentProofDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string? BankOrWalletName { get; set; }
    public string? SenderPhoneNumber { get; set; }
    public string? TransactionReference { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? VerificationNotes { get; set; }
}
