using FlashcardsStore.Domain.Entities;
using FlashcardsStore.Web.Api.Contracts.Auth;

namespace FlashcardsStore.Web.Api.Mappers;

/// <summary>Maps domain customer entities to mobile API profile contracts.</summary>
public static class MobileCustomerMapper
{
    public static MobileCustomerProfile ToProfile(Customer customer, string token, DateTime expiresAtUtc)
        => new(customer.Id, customer.FullName, customer.Email ?? string.Empty, customer.PhoneNumber, token, expiresAtUtc);
}
