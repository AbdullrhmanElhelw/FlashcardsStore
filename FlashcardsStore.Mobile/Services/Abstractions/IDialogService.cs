namespace FlashcardsStore.Mobile.Services.Abstractions;

public interface IDialogService
{
    Task ShowAlertAsync(string title, string message, string cancel = "حسناً");
    Task<bool> ShowConfirmAsync(string title, string message, string accept = "نعم", string cancel = "لا");
    Task ShowToastAsync(string message);
}
