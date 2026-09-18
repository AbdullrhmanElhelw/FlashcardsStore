using FlashcardsStore.Domain.Entities;

namespace FlashcardsStore.Application.Interfaces.Persistence;

public interface IAdminUserRepository : IRepository<AdminUser>
{
    Task<AdminUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}
