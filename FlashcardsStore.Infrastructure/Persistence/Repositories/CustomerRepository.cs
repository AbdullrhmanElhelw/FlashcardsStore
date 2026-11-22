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

    public Task<Customer?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default)
        => _dbSet.Where(customer => customer.PhoneNumber == phone)
            .OrderByDescending(customer => customer.PasswordHash != null)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<Customer?> GetAccountByPhoneAsync(string phone, CancellationToken cancellationToken = default)
        => _dbSet.FirstOrDefaultAsync(
            customer => customer.PhoneNumber == phone && customer.PasswordHash != null,
            cancellationToken);

    public Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLower();
        return _dbSet.FirstOrDefaultAsync(
            customer => customer.Email != null && customer.Email.ToLower() == normalizedEmail,
            cancellationToken);
    }
}
