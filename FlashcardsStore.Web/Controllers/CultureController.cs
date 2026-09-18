using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsStore.Web.Controllers;

/// <summary>
/// Controller responsible for setting user's preferred language/culture in cookies.
/// Follows Single Responsibility Principle for culture switching.
/// </summary>
public class CultureController : Controller
{
    [HttpGet]
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public IActionResult SetCulture(string culture, string returnUrl = "/")
    {
        if (string.IsNullOrWhiteSpace(culture))
        {
            culture = "ar-EG";
        }

        // Validate culture is one of the supported ones
        if (culture != "ar-EG" && culture != "en-US" && culture != "ar" && culture != "en")
        {
            culture = "ar-EG";
        }

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                HttpOnly = true
            }
        );

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }
}
