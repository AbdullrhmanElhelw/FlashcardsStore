using FlashcardsStore.Mobile.Models.Api;
using FlashcardsStore.Mobile.Pages;
using FlashcardsStore.Mobile.Services.Abstractions;
using FlashcardsStore.Mobile.ViewModels;

namespace FlashcardsStore.Mobile.Services;

public sealed class MauiNavigationService(IServiceProvider services) : INavigationService
{
    public void SetStoreRoot()
    {
        var window = Application.Current?.Windows.FirstOrDefault();
        if (window is not null)
        {
            window.Page = services.GetRequiredService<StoreShell>();
        }
    }

    public void SetLoginRoot()
    {
        var window = Application.Current?.Windows.FirstOrDefault();
        if (window is not null)
        {
            var loginPage = services.GetRequiredService<LoginPage>();
            window.Page = new NavigationPage(loginPage);
        }
    }

    public void SwitchToCartTab()
    {
        if (Shell.Current is StoreShell shell)
        {
            shell.SelectCartTab();
        }
    }

    public void SwitchToHomeTab()
    {
        if (Shell.Current is StoreShell shell)
        {
            shell.SelectHomeTab();
        }
    }

    public async Task PushRegisterAsync()
    {
        var nav = GetNavigation();
        if (nav is not null)
        {
            await nav.PushAsync(services.GetRequiredService<RegisterPage>());
        }
    }

    public async Task PopAsync()
    {
        var nav = GetNavigation();
        if (nav is not null)
        {
            await nav.PopAsync();
        }
    }

    public async Task PushProductDetailAsync(string slug)
    {
        var nav = GetNavigation();
        if (nav is null) return;

        var page = services.GetRequiredService<ProductDetailPage>();
        if (page.BindingContext is ProductDetailViewModel vm)
        {
            vm.SetSlug(slug);
        }
        await nav.PushAsync(page);
    }

    public async Task PushCheckoutAsync()
    {
        var nav = GetNavigation();
        if (nav is not null)
        {
            await nav.PushAsync(services.GetRequiredService<CheckoutPage>());
        }
    }

    public async Task PushPaymentAsync(OrderDetail order, string phone)
    {
        var nav = GetNavigation();
        if (nav is null) return;

        var page = services.GetRequiredService<PaymentPage>();
        if (page.BindingContext is PaymentViewModel vm)
        {
            vm.Initialize(order, phone);
        }
        await nav.PushAsync(page);
    }

    public async Task PopToRootAsync()
    {
        var nav = GetNavigation();
        if (nav is not null)
        {
            await nav.PopToRootAsync();
        }
    }

    private static INavigation? GetNavigation()
    {
        var root = Application.Current?.Windows.FirstOrDefault()?.Page;
        if (root is Shell shell)
        {
            return shell.Navigation;
        }
        return root?.Navigation;
    }
}
