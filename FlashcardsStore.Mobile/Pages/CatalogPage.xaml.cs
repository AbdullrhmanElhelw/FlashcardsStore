using FlashcardsStore.Mobile.ViewModels;

namespace FlashcardsStore.Mobile.Pages;

public partial class CatalogPage : ContentPage
{
    private readonly CatalogViewModel _viewModel;

    public CatalogPage(CatalogViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeCommand.ExecuteAsync(null);
    }

    private async void OnSearchToggleClicked(object? sender, EventArgs e)
    {
        await HomeScroll.ScrollToAsync(SearchEntry, ScrollToPosition.Center, true);
        SearchEntry.Focus();
    }

    private async void OnBrowseBannerTapped(object? sender, TappedEventArgs e)
    {
        await HomeScroll.ScrollToAsync(ProductsSection, ScrollToPosition.Start, true);
    }
}
