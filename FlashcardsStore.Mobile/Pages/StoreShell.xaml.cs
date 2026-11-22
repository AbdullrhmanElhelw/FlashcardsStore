using FlashcardsStore.Mobile.Services.Abstractions;

namespace FlashcardsStore.Mobile.Pages;

public partial class StoreShell : Shell
{
    public StoreShell(
        ILocalizationService loc,
        CatalogPage catalogPage,
        WishlistPage wishlistPage,
        CartPage cartPage,
        OrderTrackPage orderTrackPage,
        LoginPage loginPage)
    {
        InitializeComponent();
        BindingContext = loc;

        HomeTab.Items.Add(new ShellContent { Content = catalogPage });
        WishlistTab.Items.Add(new ShellContent { Content = wishlistPage });
        CartTab.Items.Add(new ShellContent { Content = cartPage });
        OrdersTab.Items.Add(new ShellContent { Content = orderTrackPage });
        AccountTab.Items.Add(new ShellContent { Content = loginPage });
    }

    public void SelectCartTab() => MainTabBar.CurrentItem = CartTab;
    public void SelectHomeTab() => MainTabBar.CurrentItem = HomeTab;
    public void SelectWishlistTab() => MainTabBar.CurrentItem = WishlistTab;
    public void SelectOrdersTab() => MainTabBar.CurrentItem = OrdersTab;
}
