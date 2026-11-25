using System.Security.Claims;
using FlashcardsStore.Application.DTOs.Admin;
using FlashcardsStore.Application.Interfaces.Persistence;
using FlashcardsStore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsStore.Web.Controllers.Admin;

/// <summary>Handles administrator authentication (login and logout).</summary>
public sealed class AdminAuthController : AdminControllerBase
{
    private readonly IAdminUserRepository _adminUserRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public AdminAuthController(
        IAdminUserRepository adminUserRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _adminUserRepository = adminUserRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("Login")]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction(nameof(AdminDashboardController.Dashboard), "AdminDashboard");

        ViewBag.ReturnUrl = returnUrl;
        return AdminView("Login", new AdminLoginDto());
    }

    [HttpPost("LoginSubmit")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginSubmit(AdminLoginDto dto, string? returnUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
        {
            ModelState.AddModelError(string.Empty, "يرجى إدخال اسم المستخدم وكلمة المرور");
            return AdminView("Login", dto);
        }

        var user = await _adminUserRepository.GetByUsernameAsync(dto.Username.Trim(), cancellationToken);
        if (user is null || !_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "بيانات الدخول غير صحيحة");
            return AdminView("Login", dto);
        }

        // Auto-upgrade legacy password hash to salted PBKDF2 if needed
        if (!user.PasswordHash.StartsWith("$pbkdf2-sha256$", StringComparison.Ordinal))
            user.PasswordHash = _passwordHasher.HashPassword(dto.Password);

        user.LastLoginAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.GivenName, user.FullName ?? user.Username),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Role, user.Role)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction(nameof(AdminDashboardController.Dashboard), "AdminDashboard");
    }

    [HttpPost("Logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }
}
