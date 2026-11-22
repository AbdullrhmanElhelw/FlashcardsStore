using FlashcardsStore.Mobile.Models.Api;
using FlashcardsStore.Mobile.Services.Abstractions;

namespace FlashcardsStore.Mobile.Services;

public sealed class CustomerSessionService : ICustomerSessionService
{
    private const string IdKey = "customer_id";
    private const string NameKey = "customer_name";
    private const string EmailKey = "customer_email";
    private const string PhoneKey = "customer_phone";

    public bool IsSignedIn => CustomerId > 0 && !string.IsNullOrWhiteSpace(CustomerName);
    public int CustomerId => Preferences.Default.Get(IdKey, 0);
    public string CustomerName => Preferences.Default.Get(NameKey, string.Empty);
    public string CustomerEmail => Preferences.Default.Get(EmailKey, string.Empty);
    public string CustomerPhone => Preferences.Default.Get(PhoneKey, string.Empty);

    public void Save(CustomerProfile profile)
    {
        Preferences.Default.Set(IdKey, profile.Id);
        Preferences.Default.Set(NameKey, profile.FullName);
        Preferences.Default.Set(EmailKey, profile.Email);
        Preferences.Default.Set(PhoneKey, profile.PhoneNumber);
    }

    public void Clear()
    {
        Preferences.Default.Remove(IdKey);
        Preferences.Default.Remove(NameKey);
        Preferences.Default.Remove(EmailKey);
        Preferences.Default.Remove(PhoneKey);
    }
}
