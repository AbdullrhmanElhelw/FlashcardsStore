using Microsoft.EntityFrameworkCore;
using FlashcardsStore.Application.Interfaces.Persistence;
using FlashcardsStore.Domain.Entities;
using FlashcardsStore.Infrastructure.Persistence;

namespace FlashcardsStore.Infrastructure.Persistence.Repositories;

public class AdminUserRepository : Repository<AdminUser>, IAdminUserRepository
{
    public AdminUserRepository(FlashcardsStoreDbContext context) : base(context)
    {
    }

    public async Task<AdminUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Username == username && u.IsActive, cancellationToken);
    }
}
