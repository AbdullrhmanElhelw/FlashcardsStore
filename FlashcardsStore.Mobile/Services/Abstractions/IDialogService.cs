namespace FlashcardsStore.Mobile.Services.Abstractions;

public interface IDialogService
{
    Task ShowAlertAsync(string title, string message, string? cancel = null);
    Task<bool> ShowConfirmAsync(string title, string message, string? accept = null, string? cancel = null);
    Task ShowToastAsync(string message);
}
