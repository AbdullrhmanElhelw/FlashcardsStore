using FlashcardsStore.Domain.Entities;

namespace FlashcardsStore.Application.Interfaces.Persistence;

public interface IStoreSettingsRepository : IRepository<StoreSettings>
{
    Task<StoreSettings?> GetCurrentSettingsAsync(CancellationToken cancellationToken = default);
}
