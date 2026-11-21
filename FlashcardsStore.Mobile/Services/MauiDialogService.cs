using FlashcardsStore.Mobile.Pages;
using FlashcardsStore.Mobile.Services.Abstractions;

namespace FlashcardsStore.Mobile.Services;

public sealed class MauiDialogService : IDialogService
{
    public async Task ShowAlertAsync(string title, string message, string cancel = "حسناً")
    {
        var nav = GetModalNavigation();
        if (nav is null) return;

        var dialog = new BrandDialogPage(title, message, cancel, null);
        await nav.PushModalAsync(dialog, false);
        await dialog.WaitForResultAsync();
    }

    public async Task<bool> ShowConfirmAsync(string title, string message, string accept = "نعم", string cancel = "لا")
    {
        var nav = GetModalNavigation();
        if (nav is null) return false;

        var dialog = new BrandDialogPage(title, message, accept, cancel);
        await nav.PushModalAsync(dialog, false);
        return await dialog.WaitForResultAsync();
    }

    public Task ShowToastAsync(string message)
        => ShowAlertAsync("أكاديمية خلود النادي", message, "حسناً");

    private static INavigation? GetModalNavigation()
        => Application.Current?.Windows.FirstOrDefault()?.Page?.Navigation;
}
