using FlashcardsStore.Mobile.Models.Api;

namespace FlashcardsStore.Mobile.Services.Abstractions;

public interface INavigationService
{
    void SetOnboardingRoot();
    void SetStoreRoot();
    void SetLoginRoot();
    void SwitchToCartTab();
    void SwitchToHomeTab();
    void SwitchToWishlistTab();
    void SwitchToOrdersTab();
    Task PushRegisterAsync();
    Task PushLoginForCheckoutAsync();
    Task CompleteAuthFlowAsync();
    Task PopAsync();
    Task PushProductDetailAsync(string slug);
    Task PushCheckoutAsync();
    Task PushPaymentAsync(OrderDetail order, string phone);
    Task PopToRootAsync();
}
