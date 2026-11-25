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
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(idClaim, out var customerId))
            return UnauthorizedError("جلسة غير صالحة");

        var customer = await _customers.GetByIdAsync(customerId, ct);
        if (customer is null)
            return UnauthorizedError("الحساب غير موجود");

        var (token, expiresAtUtc) = _jwtTokens.GenerateCustomerToken(customer);
        return Ok(MobileCustomerMapper.ToProfile(customer, token, expiresAtUtc));
    }
}
