using FlashcardsStore.PhonicsGame.ViewModels;

namespace FlashcardsStore.PhonicsGame;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override bool OnBackButtonPressed()
    {
        var shell = Application.Current?.Handler?.MauiContext?.Services.GetService<AppShellViewModel>();
        if (shell is not null && shell.HandleHardwareBackButton())
            return true;

        return base.OnBackButtonPressed();
    }
}
