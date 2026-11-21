using System.Net;
using System.Net.Mail;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashcardsStore.Mobile.Models.Api;
using FlashcardsStore.Mobile.Services;
using FlashcardsStore.Mobile.Services.Abstractions;

namespace FlashcardsStore.Mobile.ViewModels;

public sealed partial class LoginViewModel : BaseViewModel
{
    private readonly IMobileApiClient _api;
    private readonly ICustomerSessionService _session;
    private readonly INavigationService _navigation;
    private readonly IDialogService _dialogs;

    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private bool _isPasswordHidden = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotSignedIn))]
    private bool _isSignedIn;

    [ObservableProperty] private string _customerName = string.Empty;
    [ObservableProperty] private string _customerEmail = string.Empty;
    [ObservableProperty] private string _customerPhone = string.Empty;
    [ObservableProperty] private string _customerInitial = "خ";

    [ObservableProperty] private bool _isConnectionSettingsVisible;
    [ObservableProperty] private string _serverUrl = string.Empty;

    public bool IsNotSignedIn => !IsSignedIn;

    public LoginViewModel(
        IMobileApiClient api,
        ICustomerSessionService session,
        INavigationService navigation,
        IDialogService dialogs)
    {
        _api = api;
        _session = session;
        _navigation = navigation;
        _dialogs = dialogs;
        Title = "حسابي";
        ServerUrl = _api.BaseUrl;
    }

    public void RefreshSessionState()
    {
        IsSignedIn = _session.IsSignedIn;
        CustomerName = _session.CustomerName;
        CustomerEmail = _session.CustomerEmail;
        CustomerPhone = _session.CustomerPhone;
        CustomerInitial = string.IsNullOrWhiteSpace(CustomerName)
            ? "خ"
            : CustomerName.Trim().Substring(0, 1);
        ServerUrl = _api.BaseUrl;
    }

    [RelayCommand]
    private void TogglePasswordVisibility() => IsPasswordHidden = !IsPasswordHidden;

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "يرجى إدخال البريد الإلكتروني وكلمة المرور.";
            return;
        }
        if (!MailAddress.TryCreate(Email.Trim(), out _))
        {
            ErrorMessage = "أدخلي بريداً إلكترونياً صحيحاً.";
            return;
        }

        IsBusy = true;
        try
        {
            var customer = await _api.LoginAsync(new CustomerLoginRequest
            {
                Email = Email.Trim(),
                Password = Password
            });
            _session.Save(customer);
            RefreshSessionState();
            await _navigation.CompleteAuthFlowAsync();
            await _dialogs.ShowToastAsync($"أهلاً بعودتك يا {customer.FullName}");
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
    private Task GoToRegisterAsync() => _navigation.PushRegisterAsync();

    [RelayCommand]
    private Task ForgotPasswordAsync()
        => OpenWhatsAppAsync("مرحباً مس خلود، نسيت كلمة المرور الخاصة بحسابي في أكاديمية خلود النادي.");

    [RelayCommand]
    private Task ContactWhatsAppAsync()
        => OpenWhatsAppAsync("مرحباً مس خلود، لدي استفسار بخصوص بطاقات الفلاش كاردز.");

    [RelayCommand]
    private void OpenOrders() => _navigation.SwitchToOrdersTab();

    [RelayCommand]
    private void OpenWishlist() => _navigation.SwitchToWishlistTab();

    [RelayCommand]
    private void OpenCart() => _navigation.SwitchToCartTab();

    [RelayCommand]
    private void ReplayOnboarding() => _navigation.SetOnboardingRoot();

    [RelayCommand]
    private void ToggleConnectionSettings() => IsConnectionSettingsVisible = !IsConnectionSettingsVisible;

    [RelayCommand]
    private async Task SaveServerUrlAsync()
    {
        try
        {
            _api.SetBaseUrl(ServerUrl);
            IsConnectionSettingsVisible = false;
            await _dialogs.ShowToastAsync("تم حفظ عنوان الخادم بنجاح");
        }
        catch (ArgumentException ex)
        {
            await _dialogs.ShowAlertAsync("خطأ", ex.Message);
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        if (!await _dialogs.ShowConfirmAsync("تسجيل الخروج", "هل تريدين تسجيل الخروج من حسابك؟"))
            return;

        _session.Clear();
        Email = string.Empty;
        Password = string.Empty;
        RefreshSessionState();
        await _dialogs.ShowToastAsync("تم تسجيل الخروج بنجاح");
    }

    private async Task OpenWhatsAppAsync(string message)
    {
        try
        {
            var config = await _api.GetConfigAsync();
            var url = $"https://wa.me/{config.WhatsAppNumberInternational}?text={WebUtility.UrlEncode(message)}";
            await Launcher.Default.OpenAsync(url);
        }
        catch (MobileApiException ex)
        {
            await _dialogs.ShowAlertAsync("تنبيه", ex.Message);
        }
    }
}
