using System.Net.Mail;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashcardsStore.Mobile.Models.Api;
using FlashcardsStore.Mobile.Services;
using FlashcardsStore.Mobile.Services.Abstractions;

namespace FlashcardsStore.Mobile.ViewModels;

public sealed partial class RegisterViewModel : BaseViewModel
{
    private readonly IMobileApiClient _api;
    private readonly ICustomerSessionService _session;
    private readonly INavigationService _navigation;
    private readonly IDialogService _dialogs;

    [ObservableProperty]
    private string _fullName = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _phoneNumber = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _confirmPassword = string.Empty;

    [ObservableProperty]
    private bool _isPasswordHidden = true;

    public RegisterViewModel(
        IMobileApiClient api,
        ICustomerSessionService session,
        INavigationService navigation,
        IDialogService dialogs,
        ILocalizationService loc) : base(loc)
    {
        _api = api;
        _session = session;
        _navigation = navigation;
        _dialogs = dialogs;
        Title = "إنشاء حساب جديد";
    }

    [RelayCommand]
    private void TogglePasswordVisibility() => IsPasswordHidden = !IsPasswordHidden;

    [RelayCommand]
    private async Task RegisterAsync()
    {
        ErrorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(PhoneNumber) || string.IsNullOrWhiteSpace(Password) ||
            string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            ErrorMessage = "يرجى استكمال جميع البيانات.";
            return;
        }
        if (!MailAddress.TryCreate(Email.Trim(), out _))
        {
            ErrorMessage = "أدخلي بريداً إلكترونياً صحيحاً.";
            return;
        }
        if (PhoneNumber.Count(char.IsDigit) < 7)
        {
            ErrorMessage = "أدخلي رقم هاتف صحيحاً.";
            return;
        }
        if (Password.Length < 8)
        {
            ErrorMessage = "يجب ألا تقل كلمة المرور عن 8 أحرف.";
            return;
        }
        if (Password != ConfirmPassword)
        {
            ErrorMessage = "كلمتا المرور غير متطابقتين.";
            return;
        }

        IsBusy = true;
        try
        {
            var customer = await _api.RegisterAsync(new CustomerRegisterRequest
            {
                FullName = FullName.Trim(),
                Email = Email.Trim(),
                PhoneNumber = PhoneNumber.Trim(),
                Password = Password,
                ConfirmPassword = ConfirmPassword
            });
            _session.Save(customer);
            await _navigation.CompleteAuthFlowAsync();
            await _dialogs.ShowToastAsync("تم إنشاء الحساب بنجاح، أهلاً بكِ");
        }
        catch (MobileApiException ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task GoBackToLoginAsync() => _navigation.PopAsync();
}
