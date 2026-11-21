using FlashcardsStore.Mobile.ViewModels;

namespace FlashcardsStore.Mobile.Pages;

public partial class OrderTrackPage : ContentPage
{
    public OrderTrackPage(OrderTrackViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
