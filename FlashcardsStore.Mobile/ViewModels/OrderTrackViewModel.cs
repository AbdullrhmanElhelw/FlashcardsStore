using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashcardsStore.Mobile.Models.Api;
using FlashcardsStore.Mobile.Services;
using FlashcardsStore.Mobile.Services.Abstractions;

namespace FlashcardsStore.Mobile.ViewModels;

public sealed partial class OrderTrackViewModel : BaseViewModel
{
    private readonly IMobileApiClient _api;

    [ObservableProperty]
    private string _orderNumber = string.Empty;

    [ObservableProperty]
    private string _phoneNumber = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasResult))]
    private OrderTracking? _trackingResult;

    [ObservableProperty]
    private string _proofNote = string.Empty;

    public bool HasResult => TrackingResult is not null;

    public OrderTrackViewModel(IMobileApiClient api, ICustomerSessionService session)
    {
        _api = api;
        Title = "تتبع الطلب";
        if (!string.IsNullOrWhiteSpace(session.CustomerPhone))
        {
            PhoneNumber = session.CustomerPhone;
        }
    }

    [RelayCommand]
    private async Task SearchOrderAsync()
    {
        ErrorMessage = string.Empty;
        TrackingResult = null;
        ProofNote = string.Empty;

        if (string.IsNullOrWhiteSpace(OrderNumber) || string.IsNullOrWhiteSpace(PhoneNumber))
        {
            ErrorMessage = "أدخل رقم الطلب ورقم الهاتف.";
            return;
        }

        IsBusy = true;
        try
        {
            var result = await _api.TrackOrderAsync(OrderNumber.Trim(), PhoneNumber.Trim());
            TrackingResult = result;
            ProofNote = result.RequiresPaymentProof
                ? (result.HasUploadedProof
                    ? "تم استلام إيصال الدفع وجارٍ التحقق منه من قبل الأكاديمية ✓"
                    : "يرجى رفع إيصال الدفع من صفحة تعليمات الدفع لتأكيد الحجز.")
                : "لا توجد مستندات دفع مطلوبة لهذا الطلب.";
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
