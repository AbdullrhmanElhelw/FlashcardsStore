namespace FlashcardsStore.Web.Api.Contracts.Auth;

public sealed record MobileCustomerProfile(
    int Id,
    string FullName,
    string Email,
    string PhoneNumber);
