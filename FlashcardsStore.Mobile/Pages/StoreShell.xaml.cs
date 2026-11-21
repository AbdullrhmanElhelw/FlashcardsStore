namespace FlashcardsStore.Mobile.Pages;

public partial class StoreShell : Shell
{
    public StoreShell(
        CatalogPage catalogPage,
        WishlistPage wishlistPage,
        CartPage cartPage,
        OrderTrackPage orderTrackPage,
        LoginPage loginPage)
    {
        InitializeComponent();

        HomeTab.Items.Add(new ShellContent { Title = "الرئيسية", Content = catalogPage });
        WishlistTab.Items.Add(new ShellContent { Title = "المفضلة", Content = wishlistPage });
        CartTab.Items.Add(new ShellContent { Title = "السلة", Content = cartPage });
        OrdersTab.Items.Add(new ShellContent { Title = "تتبع الطلب", Content = orderTrackPage });
        AccountTab.Items.Add(new ShellContent { Title = "حسابي", Content = loginPage });
    }

    public void SelectCartTab() => MainTabBar.CurrentItem = CartTab;
    public void SelectHomeTab() => MainTabBar.CurrentItem = HomeTab;
}
