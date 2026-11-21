using FlashcardsStore.Mobile.Services.Abstractions;

namespace FlashcardsStore.Mobile.Services;

public sealed class MauiDialogService : IDialogService
{
    public async Task ShowAlertAsync(string title, string message, string cancel = "حسناً")
    {
        var page = GetCurrentPage();
        if (page is not null)
        {
            await page.DisplayAlertAsync(title, message, cancel);
        }
    }

    public async Task<bool> ShowConfirmAsync(string title, string message, string accept = "نعم", string cancel = "لا")
    {
        var page = GetCurrentPage();
        return page is not null && await page.DisplayAlertAsync(title, message, accept, cancel);
    }

    public Task ShowToastAsync(string message)
        => ShowAlertAsync("أكاديمية خلود النادي", message, "حسناً");

    private static Page? GetCurrentPage()
        => Application.Current?.Windows.FirstOrDefault()?.Page;
}
