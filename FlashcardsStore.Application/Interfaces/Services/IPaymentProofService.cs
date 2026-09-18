using FlashcardsStore.Application.Common;
using FlashcardsStore.Application.DTOs.PaymentProof;

namespace FlashcardsStore.Application.Interfaces.Services;

public interface IPaymentProofService
{
    Task<Result<PaymentProofDto>> UploadProofAsync(PaymentProofUploadDto uploadDto, CancellationToken cancellationToken = default);
    Task<PaymentProofDto?> GetProofByOrderIdAsync(int orderId, CancellationToken cancellationToken = default);
    Task<Result<bool>> VerifyProofAsync(int proofId, string adminUsername, string? notes, CancellationToken cancellationToken = default);
}
