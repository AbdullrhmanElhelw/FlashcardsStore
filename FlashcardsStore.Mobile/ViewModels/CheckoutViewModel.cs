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

    public IReadOnlyList<string> Governorates { get; } = new[]
    {
        "القاهرة", "الجيزة", "الإسكندرية", "القليوبية", "الشرقية", "الدقهلية",
        "الغربية", "المنوفية", "البحيرة", "دمياط", "كفر الشيخ", "بورسعيد",
        "الإسماعيلية", "السويس", "الفيوم", "بني سويف", "المنيا", "أسيوط",
        "سوهاج", "قنا", "الأقصر", "أسوان", "البحر الأحمر", "مطروح"
    };

    public IReadOnlyList<string> CustomerTypeOptions { get; } = new[] { "ولي أمر", "معلمة", "مدرسة" };
    public IReadOnlyList<string> PaymentMethodOptions { get; } = new[]
    {
        "تحويل محفظة إلكترونية / InstaPay",
        "التواصل عبر واتساب"
    };

    [ObservableProperty] private string _fullName = string.Empty;
    [ObservableProperty] private string _phoneNumber = string.Empty;
    [ObservableProperty] private string _whatsAppNumber = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _selectedGovernorate = "القاهرة";
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
        IDialogService dialogs)
    {
        _api = api;
        _cart = cart;
        _session = session;
        _navigation = navigation;
        _dialogs = dialogs;
        Title = "إتمام الشراء";

        FullName = session.CustomerName;
        PhoneNumber = session.CustomerPhone;
        Email = session.CustomerEmail;
    }

    partial void OnSelectedGovernorateChanged(string value)
    {
        _ = RefreshShippingQuoteAsync();
    }

    [RelayCommand]
    public async Task RefreshShippingQuoteAsync()
    {
        if (_cart.Items.Count == 0)
        {
            ErrorMessage = "السلة فارغة. أضيفي منتجاً قبل إتمام الشراء.";
            TotalsSummary = string.Empty;
            return;
        }

        ErrorMessage = string.Empty;
        try
        {
            var quote = await _api.GetShippingQuoteAsync(SelectedGovernorate, _cart.Subtotal);
            IsFreeShipping = quote.IsFreeShipping;
            var shippingText = quote.IsFreeShipping ? "مجاني 🎉" : $"{quote.ShippingFee:0.##} ج.م";
            TotalsSummary = $"المجموع: {quote.Subtotal:0.##} ج.م   ·   الشحن: {shippingText}\nالإجمالي النهائي: {quote.GrandTotal:0.##} ج.م";
        }
        catch (MobileApiException ex)
        {
            TotalsSummary = $"المجموع قبل الشحن: {_cart.Subtotal:0.##} ج.م";
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task SubmitOrderAsync()
    {
        ErrorMessage = string.Empty;
        if (_cart.Items.Count == 0)
        {
            await _dialogs.ShowAlertAsync("تنبيه", "السلة فارغة");
            return;
        }
        if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(PhoneNumber) ||
            string.IsNullOrWhiteSpace(City) || string.IsNullOrWhiteSpace(ShippingAddress))
        {
            ErrorMessage = "أدخلي الاسم، الهاتف، المدينة، والعنوان بالتفصيل.";
            return;
        }
        if (PhoneNumber.Count(char.IsDigit) < 10)
        {
            ErrorMessage = "أدخلي رقم هاتف مصري صحيح.";
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
                Governorate = SelectedGovernorate,
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

            await _dialogs.ShowAlertAsync("تم تسجيل الطلب ✓", $"رقم الطلب: {order.OrderNumber}\nالإجمالي: {order.GrandTotal:0.##} ج.م");
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
