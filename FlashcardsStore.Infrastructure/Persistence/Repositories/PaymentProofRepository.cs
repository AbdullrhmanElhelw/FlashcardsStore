using Microsoft.EntityFrameworkCore;
using FlashcardsStore.Application.Interfaces.Persistence;
using FlashcardsStore.Domain.Entities;
using FlashcardsStore.Infrastructure.Persistence;

namespace FlashcardsStore.Infrastructure.Persistence.Repositories;

public class PaymentProofRepository : Repository<PaymentProof>, IPaymentProofRepository
{
    public PaymentProofRepository(FlashcardsStoreDbContext context) : base(context)
    {
    }

    public async Task<PaymentProof?> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);
    }
}
