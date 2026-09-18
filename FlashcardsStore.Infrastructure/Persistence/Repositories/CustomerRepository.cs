using Microsoft.EntityFrameworkCore;
using FlashcardsStore.Application.Interfaces.Persistence;
using FlashcardsStore.Domain.Entities;
using FlashcardsStore.Infrastructure.Persistence;

namespace FlashcardsStore.Infrastructure.Persistence.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(FlashcardsStoreDbContext context) : base(context)
    {
    }

    public async Task<Customer?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.PhoneNumber == phone, cancellationToken);
    }
}
