using FlashcardsStore.Domain.Entities;

namespace FlashcardsStore.Application.Interfaces.Persistence;

public interface IPaymentProofRepository : IRepository<PaymentProof>
{
    Task<PaymentProof?> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default);
}
