using FlashcardsStore.Domain.Entities;

namespace FlashcardsStore.Application.Interfaces.Persistence;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default);
    Task<Customer?> GetAccountByPhoneAsync(string phone, CancellationToken cancellationToken = default);
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
