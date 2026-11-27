namespace FlashcardsStore.Mobile.Models.Api;

public sealed class UpdateProfileRequest
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Governorate { get; set; } = string.Empty;
}
