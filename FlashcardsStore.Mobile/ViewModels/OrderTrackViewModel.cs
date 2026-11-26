using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashcardsStore.Mobile.Models.Api;
using FlashcardsStore.Mobile.Services;
using FlashcardsStore.Mobile.Services.Abstractions;

namespace FlashcardsStore.Mobile.ViewModels;

public sealed partial class OrderTrackViewModel : BaseViewModel
{
    private readonly IMobileApiClient _api;
    private readonly ICustomerSessionService _session;

    [ObservableProperty]
    private string _orderNumber = string.Empty;

    [ObservableProperty]
    private string _phoneNumber = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasResult))]
    [NotifyPropertyChangedFor(nameof(OrderLabel))]
    [NotifyPropertyChangedFor(nameof(OrderTotalLabel))]
    [NotifyPropertyChangedFor(nameof(CustomerLabel))]
    [NotifyPropertyChangedFor(nameof(StatusLabel))]
    [NotifyPropertyChangedFor(nameof(ProductCountLabel))]
    private OrderTracking? _trackingResult;

    [ObservableProperty]
    private string _proofNote = string.Empty;

    public ObservableCollection<MyOrderSummary> MyOrders { get; } = new();
    public bool HasMyOrders => MyOrders.Count > 0;

    public bool HasResult => TrackingResult is not null;
    public string OrderLabel => TrackingResult is null ? string.Empty : Loc.Format("OrderFormat", TrackingResult.OrderNumber);
    public string OrderTotalLabel => TrackingResult is null ? string.Empty : Loc.Format("CurrencyAmount", TrackingResult.GrandTotal);
    public string CustomerLabel => TrackingResult is null ? string.Empty : Loc.Format("CustomerFormat", TrackingResult.CustomerName);
    public string StatusLabel => TrackingResult is null ? string.Empty : Loc.Format("StatusFormat", Loc.GetLocalized(TrackingResult.StatusDescriptionAr, TrackingResult.StatusDescription));
    public string ProductCountLabel => TrackingResult is null ? string.Empty : Loc.Format("ProductCountFormat", TrackingResult.ItemCount);

    public OrderTrackViewModel(
        IMobileApiClient api,
        ICustomerSessionService session,
        ILocalizationService loc) : base(loc)
    {
        _api = api;
        _session = session;
        Title = Loc["TrackOrderTitle"];
        loc.LanguageChanged += (_, _) =>
        {
            Title = Loc["TrackOrderTitle"];
            OnPropertyChanged(nameof(OrderLabel));
            OnPropertyChanged(nameof(OrderTotalLabel));
            OnPropertyChanged(nameof(CustomerLabel));
            OnPropertyChanged(nameof(StatusLabel));
            OnPropertyChanged(nameof(ProductCountLabel));
            UpdateProofNote();
        };
        if (!string.IsNullOrWhiteSpace(session.CustomerPhone))
        {
            PhoneNumber = session.CustomerPhone;
        }
    }

    [RelayCommand]
    public async Task LoadMyOrdersAsync()
    {
        if (!_session.IsSignedIn || string.IsNullOrWhiteSpace(_session.Token))
        {
            MyOrders.Clear();
            OnPropertyChanged(nameof(HasMyOrders));
            return;
        }

        try
        {
            var orders = await _api.GetMyOrdersAsync();
            MyOrders.Clear();
            foreach (var o in orders)
            {
                MyOrders.Add(o);
            }
            OnPropertyChanged(nameof(HasMyOrders));
        }
        catch
        {
            // Keep manual order tracking working even if history fetch fails
        }
    }

    [RelayCommand]
    private async Task SelectMyOrderAsync(MyOrderSummary? order)
    {
        if (order is null) return;
        OrderNumber = order.OrderNumber;
        if (!string.IsNullOrWhiteSpace(order.CustomerPhone))
            PhoneNumber = order.CustomerPhone;
        await SearchOrderAsync();
    }

    [RelayCommand]
    private async Task SearchOrderAsync()
    {
        ErrorMessage = string.Empty;
        TrackingResult = null;
        ProofNote = string.Empty;

        if (string.IsNullOrWhiteSpace(OrderNumber) || string.IsNullOrWhiteSpace(PhoneNumber))
        {
            ErrorMessage = Loc["ValidationOrderRequired"];
            return;
        }

        IsBusy = true;
        try
        {
            var result = await _api.TrackOrderAsync(OrderNumber.Trim(), PhoneNumber.Trim());
            TrackingResult = result;
            UpdateProofNote();
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

    private void UpdateProofNote()
    {
        ProofNote = TrackingResult is null
            ? string.Empty
            : TrackingResult.RequiresPaymentProof
                ? Loc[TrackingResult.HasUploadedProof ? "OrderPaymentReceived" : "OrderPaymentMissing"]
                : Loc["OrderNoProofRequired"];
    }
}
