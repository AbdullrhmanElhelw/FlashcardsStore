namespace FlashcardsStore.Mobile.Pages;

public partial class BrandDialogPage : ContentPage
{
    private readonly TaskCompletionSource<bool> _tcs = new();

    public BrandDialogPage(string title, string message, string accept, string? cancel)
    {
        InitializeComponent();
        TitleLabel.Text = title;
        MessageLabel.Text = message;
        IconLabel.Text = PickIcon(title);

        if (string.IsNullOrWhiteSpace(cancel))
        {
            OkButton.Text = accept;
            OkButton.IsVisible = true;
        }
        else
        {
            AcceptButton.Text = accept;
            CancelButton.Text = cancel;
            ConfirmButtonsGrid.IsVisible = true;
        }
    }

    public Task<bool> WaitForResultAsync() => _tcs.Task;

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        DialogCard.Opacity = 0;
        DialogCard.Scale = 0.9;
        await Task.WhenAll(
            DialogCard.FadeToAsync(1, 180, Easing.CubicOut),
            DialogCard.ScaleToAsync(1, 180, Easing.CubicOut));
    }

    protected override bool OnBackButtonPressed()
    {
        _ = CloseAsync(false);
        return true;
    }

    private async void OnAcceptClicked(object? sender, EventArgs e) => await CloseAsync(true);
    private async void OnCancelClicked(object? sender, EventArgs e) => await CloseAsync(false);
    private async void OnOkClicked(object? sender, EventArgs e) => await CloseAsync(true);

    private async Task CloseAsync(bool result)
    {
        await DialogCard.FadeToAsync(0, 120, Easing.CubicIn);
        if (Navigation.ModalStack.Contains(this))
        {
            await Navigation.PopModalAsync(false);
        }
        _tcs.TrySetResult(result);
    }

    private static string PickIcon(string title)
    {
        if (title.Contains("السلة")) return "🛒";
        if (title.Contains("تسجيل") || title.Contains("أهلاً")) return "🔐";
        if (title.Contains("تم") || title.Contains("شكراً")) return "✓";
        if (title.Contains("خطأ") || title.Contains("تنبيه")) return "⚠️";
        return "✨";
    }
}
