using FlashcardsStore.Domain.Entities;

namespace FlashcardsStore.Application.Interfaces.Persistence;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default);
}
