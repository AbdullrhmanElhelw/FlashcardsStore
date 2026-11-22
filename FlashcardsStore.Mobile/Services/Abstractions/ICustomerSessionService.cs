using FlashcardsStore.Mobile.Models.Api;

namespace FlashcardsStore.Mobile.Services.Abstractions;

public interface ICustomerSessionService
{
    bool IsSignedIn { get; }
    int CustomerId { get; }
    string CustomerName { get; }
    string CustomerEmail { get; }
    string CustomerPhone { get; }
    void Save(CustomerProfile profile);
    void Clear();
}
