namespace FlashcardsStore.Mobile.Controls;

public partial class BrandHeaderView : ContentView
{
    public static readonly BindableProperty HeaderTitleProperty = BindableProperty.Create(
        nameof(HeaderTitle), typeof(string), typeof(BrandHeaderView), string.Empty);

    public static readonly BindableProperty HeaderSubtitleProperty = BindableProperty.Create(
        nameof(HeaderSubtitle), typeof(string), typeof(BrandHeaderView), "أكاديمية خلود النادي");

    public static readonly BindableProperty ShowBackButtonProperty = BindableProperty.Create(
        nameof(ShowBackButton), typeof(bool), typeof(BrandHeaderView), false);

    public string HeaderTitle
    {
        get => (string)GetValue(HeaderTitleProperty);
        set => SetValue(HeaderTitleProperty, value);
    }

    public string HeaderSubtitle
    {
        get => (string)GetValue(HeaderSubtitleProperty);
        set => SetValue(HeaderSubtitleProperty, value);
    }

    public bool ShowBackButton
    {
        get => (bool)GetValue(ShowBackButtonProperty);
        set => SetValue(ShowBackButtonProperty, value);
    }

    public BrandHeaderView()
    {
        InitializeComponent();
    }

    private async void OnBackTapped(object? sender, TappedEventArgs e)
    {
        if (Navigation.NavigationStack.Count > 1)
        {
            await Navigation.PopAsync();
            return;
        }
        var current = Application.Current?.Windows.FirstOrDefault()?.Page;
        if (current?.Navigation?.NavigationStack.Count > 1)
        {
            await current.Navigation.PopAsync();
        }
    }
}
