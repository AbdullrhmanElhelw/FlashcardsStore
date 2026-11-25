using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashcardsStore.Mobile.Models.Api;
using FlashcardsStore.Mobile.Models.Enums;
using FlashcardsStore.Mobile.Services;
using FlashcardsStore.Mobile.Services.Abstractions;

namespace FlashcardsStore.Mobile.ViewModels;

public sealed partial class CheckoutViewModel : BaseViewModel
{
    private readonly IMobileApiClient _api;
    private readonly ICartStore _cart;
    private readonly ICustomerSessionService _session;
    private readonly INavigationService _navigation;
    private readonly IDialogService _dialogs;

    private static readonly string[] GovernorateKeys =
    {
        "Governorate.Cairo", "Governorate.Giza", "Governorate.Alexandria", "Governorate.Qalyubia",
        "Governorate.Sharqia", "Governorate.Dakahlia", "Governorate.Gharbia", "Governorate.Monufia",
        "Governorate.Beheira", "Governorate.Damietta", "Governorate.KafrElSheikh", "Governorate.PortSaid",
        "Governorate.Ismailia", "Governorate.Suez", "Governorate.Fayoum", "Governorate.BeniSuef",
        "Governorate.Minya", "Governorate.Assiut", "Governorate.Sohag", "Governorate.Qena",
        "Governorate.Luxor", "Governorate.Aswan", "Governorate.RedSea", "Governorate.Matrouh"
    };

    private static readonly string[] GovernorateApiValues =
    {
        "القاهرة", "الجيزة", "الإسكندرية", "القليوبية", "الشرقية", "الدقهلية",
        "الغربية", "المنوفية", "البحيرة", "دمياط", "كفر الشيخ", "بورسعيد",
        "الإسماعيلية", "السويس", "الفيوم", "بني سويف", "المنيا", "أسيوط",
        "سوهاج", "قنا", "الأقصر", "أسوان", "البحر الأحمر", "مطروح"
    };

    public IReadOnlyList<string> Governorates => GovernorateKeys.Select(Loc.Get).ToArray();
    public IReadOnlyList<string> CustomerTypeOptions => new[] { Loc["ParentOption"], Loc["TeacherOption"], Loc["SchoolOption"] };
    public IReadOnlyList<string> PaymentMethodOptions => new[] { Loc["WalletTransferOption"], Loc["WhatsAppPaymentOption"] };

    [ObservableProperty] private string _fullName = string.Empty;
    [ObservableProperty] private string _phoneNumber = string.Empty;
    [ObservableProperty] private string _whatsAppNumber = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private int _selectedGovernorateIndex;
    [ObservableProperty] private string _city = string.Empty;
    [ObservableProperty] private string _shippingAddress = string.Empty;
    [ObservableProperty] private string _notes = string.Empty;
    [ObservableProperty] private int _selectedCustomerTypeIndex;
    [ObservableProperty] private int _selectedPaymentMethodIndex;

    [ObservableProperty] private string _totalsSummary = string.Empty;
    [ObservableProperty] private bool _isFreeShipping;

    public CheckoutViewModel(
        IMobileApiClient api,
        ICartStore cart,
        ICustomerSessionService session,
        INavigationService navigation,
        IDialogService dialogs,
        ILocalizationService loc) : base(loc)
    {
        _api = api;
        _cart = cart;
        _session = session;
        _navigation = navigation;
        _dialogs = dialogs;
        Title = Loc["CheckoutTitle"];
        loc.LanguageChanged += (_, _) =>
        {
            Title = Loc["CheckoutTitle"];
            OnPropertyChanged(nameof(Governorates));
            OnPropertyChanged(nameof(CustomerTypeOptions));
            OnPropertyChanged(nameof(PaymentMethodOptions));
            _ = RefreshShippingQuoteAsync();
        };

        FullName = session.CustomerName;
        PhoneNumber = session.CustomerPhone;
        Email = session.CustomerEmail;
    }

    partial void OnSelectedGovernorateIndexChanged(int value)
    {
        if (value >= 0 && value < GovernorateApiValues.Length)
            _ = RefreshShippingQuoteAsync();
    }

    [RelayCommand]
    public async Task RefreshShippingQuoteAsync()
    {
        if (_cart.Items.Count == 0)
        {
            ErrorMessage = Loc["ValidationCartEmpty"];
            TotalsSummary = string.Empty;
            return;
        }

        ErrorMessage = string.Empty;
        try
        {
            var governorate = GovernorateApiValues[Math.Clamp(SelectedGovernorateIndex, 0, GovernorateApiValues.Length - 1)];
            var quote = await _api.GetShippingQuoteAsync(governorate, _cart.Subtotal);
            IsFreeShipping = quote.IsFreeShipping;
            var shippingText = quote.IsFreeShipping ? Loc["ShippingFree"] : Loc.Format("CurrencyAmount", quote.ShippingFee);
            TotalsSummary = Loc.Format("ShippingSummary", quote.Subtotal, shippingText, quote.GrandTotal);
        }
        catch (MobileApiException ex)
        {
            TotalsSummary = Loc.Format("SubtotalBeforeShipping", _cart.Subtotal);
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task SubmitOrderAsync()
    {
        ErrorMessage = string.Empty;
        if (_cart.Items.Count == 0)
        {
            await _dialogs.ShowAlertAsync(Loc["AlertTitle"], Loc["EmptyCartAlert"]);
            return;
        }
        if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(PhoneNumber) ||
            string.IsNullOrWhiteSpace(City) || string.IsNullOrWhiteSpace(ShippingAddress))
        {
            ErrorMessage = Loc["ValidationCheckoutRequired"];
            return;
        }
        if (PhoneNumber.Count(char.IsDigit) < 10)
        {
            ErrorMessage = Loc["ValidationEgyptPhone"];
            return;
        }

        IsBusy = true;
        try
        {
            var customerType = SelectedCustomerTypeIndex switch
            {
                1 => CustomerType.Teacher,
                2 => CustomerType.School,
                _ => CustomerType.Parent
            };
            var paymentMethod = SelectedPaymentMethodIndex == 1
                ? PaymentMethod.WhatsAppDirect
                : PaymentMethod.WalletTransfer;

            var order = await _api.CreateOrderAsync(new CreateOrderRequest
            {
                FullName = FullName.Trim(),
                PhoneNumber = PhoneNumber.Trim(),
                WhatsAppNumber = string.IsNullOrWhiteSpace(WhatsAppNumber) ? null : WhatsAppNumber.Trim(),
                Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim(),
                Governorate = GovernorateApiValues[Math.Clamp(SelectedGovernorateIndex, 0, GovernorateApiValues.Length - 1)],
                City = City.Trim(),
                ShippingAddress = ShippingAddress.Trim(),
                Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim(),
                CustomerType = customerType,
                PaymentMethod = paymentMethod,
                Items = _cart.Items.Select(i => new OrderLineRequest { ProductId = i.ProductId, Quantity = i.Quantity }).ToList()
            });

            _cart.Clear();

            if (paymentMethod == PaymentMethod.WalletTransfer)
            {
                await _navigation.PushPaymentAsync(order, PhoneNumber.Trim());
                return;
            }

            await _dialogs.ShowAlertAsync(Loc["OrderPlacedSuccess"], Loc.Format("OrderTotalToast", order.OrderNumber, order.GrandTotal));
            if (!string.IsNullOrWhiteSpace(order.WhatsAppDirectLink))
            {
                await Launcher.Default.OpenAsync(order.WhatsAppDirectLink);
            }
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
