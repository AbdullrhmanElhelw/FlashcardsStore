namespace FlashcardsStore.Web.Api.Common;

/// <summary>Strongly-typed JWT configuration for stateless mobile API authentication.</summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "FlashcardsStore.Api";
    public string Audience { get; set; } = "FlashcardsStore.Mobile";
    public string SecretKey { get; set; } = "KholoudElnady_FlashcardsStore_JWT_SecretKey_2026_Min32Chars!";
    public int ExpiryDays { get; set; } = 30;
}
