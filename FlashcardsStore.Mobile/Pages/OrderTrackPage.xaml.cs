using FlashcardsStore.Mobile.ViewModels;

namespace FlashcardsStore.Mobile.Pages;

public partial class OrderTrackPage : ContentPage
{
    private readonly OrderTrackViewModel _viewModel;

    public OrderTrackPage(OrderTrackViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadMyOrdersCommand.ExecuteAsync(null);
    }
}
