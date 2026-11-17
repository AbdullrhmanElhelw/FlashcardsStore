using Microsoft.EntityFrameworkCore;
using FlashcardsStore.Application.Interfaces.Persistence;
using FlashcardsStore.Domain.Entities;

namespace FlashcardsStore.Infrastructure.Persistence.Repositories;

public class StoreSettingsRepository : Repository<StoreSettings>, IStoreSettingsRepository
{
    public StoreSettingsRepository(FlashcardsStoreDbContext context) : base(context)
    {
    }

    public async Task<StoreSettings?> GetCurrentSettingsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.OrderBy(s => s.Id).FirstOrDefaultAsync(cancellationToken);
    }
}
