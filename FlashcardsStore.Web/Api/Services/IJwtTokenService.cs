using FlashcardsStore.Domain.Entities;

namespace FlashcardsStore.Web.Api.Services;

/// <summary>Issues signed JWT bearer tokens for authenticated mobile customers.</summary>
public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) GenerateCustomerToken(Customer customer);
}
