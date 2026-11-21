using FlashcardsStore.Mobile.Models.Ui;

namespace FlashcardsStore.Mobile.Pages;

public partial class BrandPickerPage : ContentPage
{
    private readonly TaskCompletionSource<PickerOptionItem?> _tcs = new();

    public BrandPickerPage(string title, IReadOnlyList<PickerOptionItem> options)
    {
        InitializeComponent();
        TitleLabel.Text = string.IsNullOrWhiteSpace(title) ? "اختاري من القائمة" : title;
        OptionsList.ItemsSource = options;
    }

    public Task<PickerOptionItem?> WaitForSelectionAsync() => _tcs.Task;

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        PickerCard.Opacity = 0;
        PickerCard.Scale = 0.92;
        await Task.WhenAll(
            PickerCard.FadeToAsync(1, 170, Easing.CubicOut),
            PickerCard.ScaleToAsync(1, 170, Easing.CubicOut));
    }

    protected override bool OnBackButtonPressed()
    {
        _ = CloseAsync(null);
        return true;
    }

    private async void OnOptionTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is PickerOptionItem item)
        {
            await CloseAsync(item);
        }
    }

    private async void OnCancelClicked(object? sender, EventArgs e) => await CloseAsync(null);

    private async Task CloseAsync(PickerOptionItem? result)
    {
        await PickerCard.FadeToAsync(0, 110, Easing.CubicIn);
        if (Navigation.ModalStack.Contains(this))
        {
            await Navigation.PopModalAsync(false);
        }
        _tcs.TrySetResult(result);
    }
}
