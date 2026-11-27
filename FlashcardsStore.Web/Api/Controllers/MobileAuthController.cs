using System.Security.Claims;
using FlashcardsStore.Application.Interfaces.Persistence;
using FlashcardsStore.Application.Interfaces.Services;
using FlashcardsStore.Domain.Entities;
using FlashcardsStore.Web.Api.Common;
using FlashcardsStore.Web.Api.Contracts.Auth;
using FlashcardsStore.Web.Api.Mappers;
using FlashcardsStore.Web.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsStore.Web.Api.Controllers;

/// <summary>Handles stateless customer registration, JWT login, and profile verification for the mobile app.</summary>
public sealed class MobileAuthController : MobileApiControllerBase
{
    private readonly ICustomerRepository _customers;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokens;

    public MobileAuthController(
        ICustomerRepository customers,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokens)
    {
        _customers = customers;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokens = jwtTokens;
    }

    [HttpPost("auth/register")]
    public async Task<ActionResult<MobileCustomerProfile>> Register(
        [FromBody] MobileCustomerRegisterRequest request,
        CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await _customers.GetByEmailAsync(email, ct) is not null)
            return ConflictError("يوجد حساب مسجل بهذا البريد الإلكتروني");

        var phone = request.PhoneNumber.Trim();
        if (await _customers.GetAccountByPhoneAsync(phone, ct) is not null)
            return ConflictError("رقم الهاتف مرتبط بحساب آخر بالفعل");

        if (request.PhoneNumber.Count(char.IsDigit) < 7)
            return Fail("أدخل رقم هاتف صحيحاً");

        var customer = new Customer
        {
            FullName = request.FullName.Trim(),
            Email = email,
            PhoneNumber = phone,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            ShippingAddress = string.Empty,
            City = string.Empty,
            Governorate = string.Empty
        };

        await _customers.AddAsync(customer, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var (token, expiresAtUtc) = _jwtTokens.GenerateCustomerToken(customer);
        return Ok(MobileCustomerMapper.ToProfile(customer, token, expiresAtUtc));
    }

    [HttpPost("auth/login")]
    public async Task<ActionResult<MobileCustomerProfile>> Login(
        [FromBody] MobileCustomerLoginRequest request,
        CancellationToken ct)
    {
        var customer = await _customers.GetByEmailAsync(request.Email, ct);
        if (customer?.PasswordHash is null || !_passwordHasher.VerifyPassword(request.Password, customer.PasswordHash))
            return UnauthorizedError("البريد الإلكتروني أو كلمة المرور غير صحيحة");

        var (token, expiresAtUtc) = _jwtTokens.GenerateCustomerToken(customer);
        return Ok(MobileCustomerMapper.ToProfile(customer, token, expiresAtUtc));
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("auth/me")]
    public async Task<ActionResult<MobileCustomerProfile>> Me(CancellationToken ct)
    {
        var customer = await GetAuthenticatedCustomerAsync(ct);
        if (customer is null)
            return UnauthorizedError("الحساب غير موجود");

        var (token, expiresAtUtc) = _jwtTokens.GenerateCustomerToken(customer);
        return Ok(MobileCustomerMapper.ToProfile(customer, token, expiresAtUtc));
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut("auth/me")]
    public async Task<ActionResult<MobileCustomerProfile>> UpdateProfile(
        [FromBody] MobileUpdateProfileRequest request,
        CancellationToken ct)
    {
        var customer = await GetAuthenticatedCustomerAsync(ct);
        if (customer is null)
            return UnauthorizedError("الحساب غير موجود");

        var phone = request.PhoneNumber.Trim();
        if (phone.Count(char.IsDigit) < 7)
            return Fail("أدخل رقم هاتف صحيحاً");

        var existingByPhone = await _customers.GetAccountByPhoneAsync(phone, ct);
        if (existingByPhone is not null && existingByPhone.Id != customer.Id)
            return ConflictError("رقم الهاتف مرتبط بحساب آخر بالفعل");

        customer.FullName = request.FullName.Trim();
        customer.PhoneNumber = phone;
        customer.ShippingAddress = (request.ShippingAddress ?? string.Empty).Trim();
        customer.City = (request.City ?? string.Empty).Trim();
        customer.Governorate = (request.Governorate ?? string.Empty).Trim();

        await _unitOfWork.SaveChangesAsync(ct);

        var (token, expiresAtUtc) = _jwtTokens.GenerateCustomerToken(customer);
        return Ok(MobileCustomerMapper.ToProfile(customer, token, expiresAtUtc));
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("auth/change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] MobileChangePasswordRequest request,
        CancellationToken ct)
    {
        var customer = await GetAuthenticatedCustomerAsync(ct);
        if (customer is null)
            return UnauthorizedError("الحساب غير موجود");

        if (customer.PasswordHash is null || !_passwordHasher.VerifyPassword(request.CurrentPassword, customer.PasswordHash))
            return Fail("كلمة المرور الحالية غير صحيحة");

        customer.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
        await _unitOfWork.SaveChangesAsync(ct);

        return Ok(new { message = "تم تغيير كلمة المرور بنجاح" });
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete("auth/me")]
    public async Task<IActionResult> DeleteAccount(CancellationToken ct)
    {
        var customer = await GetAuthenticatedCustomerAsync(ct);
        if (customer is null)
            return UnauthorizedError("الحساب غير موجود");

        // Anonymize PII while preserving historical order records for accounting/shipping integrity.
        customer.FullName = "حساب محذوف";
        customer.Email = $"deleted_{customer.Id}_{Guid.NewGuid():N}@deleted.local";
        customer.PhoneNumber = $"deleted-{customer.Id}";
        customer.WhatsAppNumber = null;
        customer.PasswordHash = null;
        customer.ShippingAddress = string.Empty;
        customer.City = string.Empty;
        customer.Governorate = string.Empty;

        await _unitOfWork.SaveChangesAsync(ct);

        return NoContent();
    }

    private async Task<Customer?> GetAuthenticatedCustomerAsync(CancellationToken ct)
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(idClaim, out var customerId))
            return null;

        return await _customers.GetByIdAsync(customerId, ct);
    }
}
