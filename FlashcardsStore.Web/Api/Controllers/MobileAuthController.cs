using FlashcardsStore.Application.Interfaces.Persistence;
using FlashcardsStore.Application.Interfaces.Services;
using FlashcardsStore.Domain.Entities;
using FlashcardsStore.Web.Api.Common;
using FlashcardsStore.Web.Api.Contracts.Auth;
using FlashcardsStore.Web.Api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsStore.Web.Api.Controllers;

/// <summary>Handles stateless customer registration and authentication for the mobile app.</summary>
public sealed class MobileAuthController : MobileApiControllerBase
{
    private readonly ICustomerRepository _customers;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public MobileAuthController(
        ICustomerRepository customers,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _customers = customers;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
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

        return Ok(MobileCustomerMapper.ToProfile(customer));
    }

    [HttpPost("auth/login")]
    public async Task<ActionResult<MobileCustomerProfile>> Login(
        [FromBody] MobileCustomerLoginRequest request,
        CancellationToken ct)
    {
        var customer = await _customers.GetByEmailAsync(request.Email, ct);
        if (customer?.PasswordHash is null || !_passwordHasher.VerifyPassword(request.Password, customer.PasswordHash))
            return UnauthorizedError("البريد الإلكتروني أو كلمة المرور غير صحيحة");

        return Ok(MobileCustomerMapper.ToProfile(customer));
    }
}
