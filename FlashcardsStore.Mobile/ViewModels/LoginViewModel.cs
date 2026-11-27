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

    [ObservableProperty] private bool _isEditProfileExpanded;
    [ObservableProperty] private string _editFullName = string.Empty;
    [ObservableProperty] private string _editPhoneNumber = string.Empty;
    [ObservableProperty] private string _editShippingAddress = string.Empty;
    [ObservableProperty] private string _editCity = string.Empty;
    [ObservableProperty] private string _editGovernorate = string.Empty;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasProfileError))]
    private string _profileErrorMessage = string.Empty;
    public bool HasProfileError => !string.IsNullOrWhiteSpace(ProfileErrorMessage);

    [ObservableProperty] private bool _isChangePasswordExpanded;
    [ObservableProperty] private string _currentPassword = string.Empty;
    [ObservableProperty] private string _newPassword = string.Empty;
    [ObservableProperty] private string _confirmNewPassword = string.Empty;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasPasswordError))]
    private string _passwordErrorMessage = string.Empty;
    public bool HasPasswordError => !string.IsNullOrWhiteSpace(PasswordErrorMessage);

    public bool IsNotSignedIn => !IsSignedIn;

    public LoginViewModel(
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
        Title = Loc["AccountTitle"];
        loc.LanguageChanged += (_, _) =>
        {
            Title = Loc["AccountTitle"];
            RefreshSessionState();
        };
    }

    public void RefreshSessionState()
    {
        IsSignedIn = _session.IsSignedIn;
        CustomerName = _session.CustomerName;
        CustomerEmail = _session.CustomerEmail;
        CustomerPhone = _session.CustomerPhone;
        CustomerInitial = string.IsNullOrWhiteSpace(CustomerName)
            ? (Loc.IsRtl ? "خ" : "K")
            : CustomerName.Trim().Substring(0, 1);

        if (!IsSignedIn)
        {
            IsEditProfileExpanded = false;
            IsChangePasswordExpanded = false;
        }
    }

    [RelayCommand]
    private void TogglePasswordVisibility() => IsPasswordHidden = !IsPasswordHidden;

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = Loc["ValidationLoginRequired"];
            return;
        }
        if (!MailAddress.TryCreate(Email.Trim(), out _))
        {
            ErrorMessage = Loc["ValidationEmail"];
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
            await _dialogs.ShowToastAsync(Loc.Format("WelcomeCustomer", customer.FullName));
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
        => OpenWhatsAppAsync(Loc["LoginWhatsAppForgot"]);

    [RelayCommand]
    private Task ContactWhatsAppAsync()
        => OpenWhatsAppAsync(Loc["LoginWhatsAppInquiry"]);

    [RelayCommand]
    private void OpenOrders() => _navigation.SwitchToOrdersTab();

    [RelayCommand]
    private void OpenWishlist() => _navigation.SwitchToWishlistTab();

    [RelayCommand]
    private void OpenCart() => _navigation.SwitchToCartTab();

    [RelayCommand]
    private void ReplayOnboarding() => _navigation.SetOnboardingRoot();

    [RelayCommand]
    private void ToggleEditProfile()
    {
        ProfileErrorMessage = string.Empty;
        IsEditProfileExpanded = !IsEditProfileExpanded;
        if (IsEditProfileExpanded)
        {
            IsChangePasswordExpanded = false;
            EditFullName = CustomerName;
            EditPhoneNumber = CustomerPhone;
        }
    }

    [RelayCommand]
    private async Task SaveProfileAsync()
    {
        ProfileErrorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(EditFullName) || string.IsNullOrWhiteSpace(EditPhoneNumber))
        {
            ProfileErrorMessage = Loc["ProfileValidationRequired"];
            return;
        }

        IsBusy = true;
        try
        {
            var updated = await _api.UpdateProfileAsync(new UpdateProfileRequest
            {
                FullName = EditFullName.Trim(),
                PhoneNumber = EditPhoneNumber.Trim(),
                ShippingAddress = EditShippingAddress.Trim(),
                City = EditCity.Trim(),
                Governorate = EditGovernorate.Trim()
            });
            _session.Save(updated);
            RefreshSessionState();
            IsEditProfileExpanded = false;
            await _dialogs.ShowToastAsync(Loc["ProfileUpdatedToast"]);
        }
        catch (MobileApiException ex)
        {
            ProfileErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ToggleChangePassword()
    {
        PasswordErrorMessage = string.Empty;
        IsChangePasswordExpanded = !IsChangePasswordExpanded;
        if (IsChangePasswordExpanded)
        {
            IsEditProfileExpanded = false;
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmNewPassword = string.Empty;
        }
    }

    [RelayCommand]
    private async Task SavePasswordAsync()
    {
        PasswordErrorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword))
        {
            PasswordErrorMessage = Loc["PasswordChangeRequired"];
            return;
        }
        if (NewPassword.Length < 6)
        {
            PasswordErrorMessage = Loc["PasswordMinLength"];
            return;
        }
        if (!string.Equals(NewPassword, ConfirmNewPassword, StringComparison.Ordinal))
        {
            PasswordErrorMessage = Loc["PasswordMismatch"];
            return;
        }

        IsBusy = true;
        try
        {
            await _api.ChangePasswordAsync(new ChangePasswordRequest
            {
                CurrentPassword = CurrentPassword,
                NewPassword = NewPassword
            });
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmNewPassword = string.Empty;
            IsChangePasswordExpanded = false;
            await _dialogs.ShowToastAsync(Loc["PasswordChangedToast"]);
        }
        catch (MobileApiException ex)
        {
            PasswordErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        if (!await _dialogs.ShowConfirmAsync(Loc["LogoutConfirmTitle"], Loc["LogoutConfirmMessage"]))
            return;

        _session.Clear();
        Email = string.Empty;
        Password = string.Empty;
        RefreshSessionState();
        await _dialogs.ShowToastAsync(Loc["LogoutSuccess"]);
    }

    [RelayCommand]
    private async Task DeleteAccountAsync()
    {
        if (!await _dialogs.ShowConfirmAsync(Loc["DeleteAccountConfirmTitle"], Loc["DeleteAccountConfirmMessage"]))
            return;

        IsBusy = true;
        try
        {
            await _api.DeleteAccountAsync();
            _session.Clear();
            Email = string.Empty;
            Password = string.Empty;
            RefreshSessionState();
            await _dialogs.ShowToastAsync(Loc["AccountDeletedToast"]);
        }
        catch (MobileApiException ex)
        {
            await _dialogs.ShowAlertAsync(Loc["AlertTitle"], ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
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
            await _dialogs.ShowAlertAsync(Loc["AlertTitle"], ex.Message);
        }
    }
}
