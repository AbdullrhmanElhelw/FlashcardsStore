namespace FlashcardsStore.Application.DTOs.PaymentProof;

public class PaymentProofUploadDto
{
    public int OrderId { get; set; }
    public string? BankOrWalletName { get; set; } // Vodafone Cash, InstaPay, etc.
    public string? SenderPhoneNumber { get; set; }
    public string? TransactionReference { get; set; }
    public Stream FileStream { get; set; } = Stream.Null;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileLength { get; set; }
}
