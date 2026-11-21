using FlashcardsStore.Mobile.ViewModels;

namespace FlashcardsStore.Mobile.Pages;

public partial class CheckoutPage : ContentPage
{
    private readonly CheckoutViewModel _viewModel;

    public CheckoutPage(CheckoutViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.RefreshShippingQuoteCommand.ExecuteAsync(null);
    }
}
