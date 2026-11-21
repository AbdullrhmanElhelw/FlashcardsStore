using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashcardsStore.Mobile.Models.Api;
using FlashcardsStore.Mobile.Services;
using FlashcardsStore.Mobile.Services.Abstractions;

namespace FlashcardsStore.Mobile.ViewModels;

public sealed partial class PaymentViewModel : BaseViewModel
{
    private readonly IMobileApiClient _api;
    private readonly INavigationService _navigation;
    private readonly IDialogService _dialogs;

    private OrderDetail? _order;
    private string _phone = string.Empty;
    private FileResult? _selectedFile;

    public IReadOnlyList<string> WalletOptions { get; } = new[]
    {
        "Vodafone Cash", "InstaPay", "Orange Money", "Etisalat Cash", "WE Pay"
    };

    [ObservableProperty] private string _orderNumber = string.Empty;
    [ObservableProperty] private decimal _grandTotal;
    [ObservableProperty] private string _vodafoneCashNumber = string.Empty;
    [ObservableProperty] private string _instaPayAddress = string.Empty;
    [ObservableProperty] private string _instaPayAccountName = string.Empty;
    [ObservableProperty] private string _paymentInstructionsAr = string.Empty;
    [ObservableProperty] private string _whatsAppDirectLink = string.Empty;
    [ObservableProperty] private string _selectedWallet = "Vodafone Cash";
    [ObservableProperty] private string _senderPhone = string.Empty;
    [ObservableProperty] private string _transactionReference = string.Empty;
    [ObservableProperty] private string _selectedFileName = "لم يتم اختيار صورة إيصال";

    public bool HasWhatsAppLink => !string.IsNullOrWhiteSpace(WhatsAppDirectLink);

    public PaymentViewModel(
        IMobileApiClient api,
        INavigationService navigation,
        IDialogService dialogs)
    {
        _api = api;
        _navigation = navigation;
        _dialogs = dialogs;
        Title = "تعليمات الدفع";
    }

    public void Initialize(OrderDetail order, string phone)
    {
        _order = order;
        _phone = phone;
        OrderNumber = order.OrderNumber;
        GrandTotal = order.GrandTotal;
        WhatsAppDirectLink = order.WhatsAppDirectLink ?? string.Empty;
        OnPropertyChanged(nameof(HasWhatsAppLink));
    }

    [RelayCommand]
    public async Task LoadConfigAsync()
    {
        ErrorMessage = string.Empty;
        try
        {
            var config = await _api.GetConfigAsync();
            VodafoneCashNumber = config.VodafoneCashNumber;
            InstaPayAddress = config.InstaPayAddress;
            InstaPayAccountName = config.InstaPayAccountName;
            PaymentInstructionsAr = config.PaymentInstructionsAr ?? string.Empty;
        }
        catch (MobileApiException ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task CopyVodafoneAsync()
    {
        if (string.IsNullOrWhiteSpace(VodafoneCashNumber)) return;
        await Clipboard.Default.SetTextAsync(VodafoneCashNumber);
        await _dialogs.ShowToastAsync($"تم نسخ رقم فودافون كاش: {VodafoneCashNumber}");
    }

    [RelayCommand]
    private async Task CopyInstaPayAsync()
    {
        if (string.IsNullOrWhiteSpace(InstaPayAddress)) return;
        await Clipboard.Default.SetTextAsync(InstaPayAddress);
        await _dialogs.ShowToastAsync($"تم نسخ عنوان InstaPay: {InstaPayAddress}");
    }

    [RelayCommand]
    private async Task OpenWhatsAppAsync()
    {
        if (!string.IsNullOrWhiteSpace(WhatsAppDirectLink))
        {
            await Launcher.Default.OpenAsync(WhatsAppDirectLink);
        }
    }

    [RelayCommand]
    private async Task PickReceiptAsync()
    {
        try
        {
            _selectedFile = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "اختر صورة إيصال التحويل",
                FileTypes = FilePickerFileType.Images
            });
            SelectedFileName = _selectedFile?.FileName ?? "لم يتم اختيار صورة إيصال";
        }
        catch (OperationCanceledException) { }
    }

    [RelayCommand]
    private async Task UploadReceiptAsync()
    {
        if (_order is null) return;
        if (_selectedFile is null)
        {
            await _dialogs.ShowAlertAsync("تنبيه", "اختر صورة الإيصال أولاً");
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            await _api.UploadPaymentProofAsync(
                _order.OrderNumber,
                _phone,
                _selectedFile,
                SelectedWallet,
                string.IsNullOrWhiteSpace(SenderPhone) ? null : SenderPhone.Trim(),
                string.IsNullOrWhiteSpace(TransactionReference) ? null : TransactionReference.Trim());

            await _dialogs.ShowAlertAsync("تم الإرسال ✓", "تم رفع إيصال التحويل بنجاح وجارٍ التحقق منه من قبل الأكاديمية.");
            await _navigation.PopToRootAsync();
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
}
