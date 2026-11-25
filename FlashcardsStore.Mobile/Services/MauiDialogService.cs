using FlashcardsStore.Mobile.Pages;
using FlashcardsStore.Mobile.Services.Abstractions;

namespace FlashcardsStore.Mobile.Services;

public sealed class MauiDialogService(ILocalizationService loc) : IDialogService
{
    public async Task ShowAlertAsync(string title, string message, string? cancel = null)
    {
        var nav = GetModalNavigation();
        if (nav is null) return;

        var dialog = new BrandDialogPage(title, message, cancel ?? loc["Okay"], null)
        {
            FlowDirection = loc.FlowDirection
        };
        await nav.PushModalAsync(dialog, false);
        await dialog.WaitForResultAsync();
    }

    public async Task<bool> ShowConfirmAsync(string title, string message, string? accept = null, string? cancel = null)
    {
        var nav = GetModalNavigation();
        if (nav is null) return false;

        var dialog = new BrandDialogPage(title, message, accept ?? loc["Yes"], cancel ?? loc["No"])
        {
            FlowDirection = loc.FlowDirection
        };
        await nav.PushModalAsync(dialog, false);
        return await dialog.WaitForResultAsync();
    }

    public async Task ShowToastAsync(string message)
    {
        if (GetActiveContentPage()?.Content is not Grid rootGrid)
        {
            await ShowAlertAsync(loc["BrandName"], message);
            return;
        }

        var toast = CreateToastBanner(message);
        if (rootGrid.RowDefinitions.Count > 1)
        {
            Grid.SetRowSpan(toast, rootGrid.RowDefinitions.Count);
        }
        if (rootGrid.ColumnDefinitions.Count > 1)
        {
            Grid.SetColumnSpan(toast, rootGrid.ColumnDefinitions.Count);
        }

        toast.Opacity = 0;
        toast.TranslationY = -20;
        rootGrid.Children.Add(toast);

        await Task.WhenAll(
            toast.FadeToAsync(1, 200, Easing.CubicOut),
            toast.TranslateToAsync(0, 0, 200, Easing.CubicOut));

        await Task.Delay(2000);

        await Task.WhenAll(
            toast.FadeToAsync(0, 180, Easing.CubicIn),
            toast.TranslateToAsync(0, -16, 180, Easing.CubicIn));

        rootGrid.Children.Remove(toast);
    }

    private static Border CreateToastBanner(string message) => new()
    {
        BackgroundColor = Color.FromArgb("#2A0B1E"),
        Stroke = Color.FromArgb("#D4AF37"),
        StrokeThickness = 1.2,
        StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 18 },
        Padding = new Thickness(16, 11),
        Margin = new Thickness(18, 16, 18, 0),
        VerticalOptions = LayoutOptions.Start,
        HorizontalOptions = LayoutOptions.Center,
        InputTransparent = true,
        ZIndex = 999,
        Shadow = new Shadow
        {
            Brush = Color.FromArgb("#18061A"),
            Opacity = 0.25f,
            Radius = 12,
            Offset = new Point(0, 4)
        },
        Content = new HorizontalStackLayout
        {
            Spacing = 8,
            Children =
            {
                new Label
                {
                    Text = "✓",
                    FontSize = 15,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#D4AF37"),
                    VerticalTextAlignment = TextAlignment.Center
                },
                new Label
                {
                    Text = message,
                    FontFamily = "Cairo",
                    FontSize = 13,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.White,
                    VerticalTextAlignment = TextAlignment.Center,
                    MaxLines = 2,
                    LineBreakMode = LineBreakMode.TailTruncation
                }
            }
        }
    };

    private static ContentPage? GetActiveContentPage()
    {
        var root = Application.Current?.Windows.FirstOrDefault()?.Page;
        if (root is Shell shell)
        {
            return shell.CurrentPage as ContentPage;
        }
        if (root is NavigationPage nav)
        {
            return nav.CurrentPage as ContentPage;
        }
        return root as ContentPage;
    }

    private static INavigation? GetModalNavigation()
        => Application.Current?.Windows.FirstOrDefault()?.Page?.Navigation;
}
