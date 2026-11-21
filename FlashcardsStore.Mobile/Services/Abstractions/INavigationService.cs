using FlashcardsStore.Mobile.Models.Api;

namespace FlashcardsStore.Mobile.Services.Abstractions;

public interface INavigationService
{
    void SetStoreRoot();
    void SetLoginRoot();
    void SwitchToCartTab();
    void SwitchToHomeTab();
    Task PushRegisterAsync();
    Task PopAsync();
    Task PushProductDetailAsync(string slug);
    Task PushCheckoutAsync();
    Task PushPaymentAsync(OrderDetail order, string phone);
    Task PopToRootAsync();
}
