using System.ComponentModel;
using FlashcardsStore.Mobile.Services.Abstractions;

namespace FlashcardsStore.Mobile.Services;

public sealed class MauiLocalizationService : ILocalizationService
{
    private const string LanguageKey = "app_language_v1";
    private string _currentLang;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? LanguageChanged;

    public MauiLocalizationService()
    {
        _currentLang = Preferences.Default.Get(LanguageKey, "ar");
    }

    public bool IsRtl => _currentLang != "en";
    public FlowDirection FlowDirection => IsRtl ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
    public string SwitchButtonLabel => IsRtl ? "EN" : "عربي";
    public string LanguageLabel => IsRtl ? "English" : "العربية";

    public string this[string key] => Get(key);

    public string Get(string key)
    {
        if (Strings.TryGetValue(key, out var pair))
        {
            return IsRtl ? pair.Ar : pair.En;
        }
        return key;
    }

    public string GetLocalized(string? arabic, string? english)
    {
        if (IsRtl)
            return !string.IsNullOrWhiteSpace(arabic) ? arabic : (english ?? string.Empty);
        return !string.IsNullOrWhiteSpace(english) ? english : (arabic ?? string.Empty);
    }

    public void ToggleLanguage()
    {
        _currentLang = IsRtl ? "en" : "ar";
        Preferences.Default.Set(LanguageKey, _currentLang);

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item"));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRtl)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FlowDirection)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SwitchButtonLabel)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LanguageLabel)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }

    private static readonly Dictionary<string, (string Ar, string En)> Strings = new()
    {
        // Brand & Navbar
        ["BrandName"] = ("أكاديمية خلود النادي", "Kholoud Elnady Academy"),
        ["BrandSubtitle"] = ("بطاقات الفلاش كاردز التعليمية", "Interactive Educational Flashcards"),

        // Bottom Tabs
        ["TabHome"] = ("الرئيسية", "Home"),
        ["TabWishlist"] = ("المفضلة", "Wishlist"),
        ["TabCart"] = ("السلة", "Cart"),
        ["TabOrders"] = ("تتبع الطلب", "Track Order"),
        ["TabAccount"] = ("حسابي", "Account"),

        // Home / Catalog
        ["HeroBadge"] = ("معتمد تعليمياً للأطفال", "Curriculum Aligned for Kids"),
        ["HeroTitle"] = ("أهلاً بيكي في أكاديمية خلود النادي", "Welcome to Kholoud Elnady Academy"),
        ["HeroSubtitle"] = ("تأسيس الإنجليزية بالصوتيات واللعب التفاعلي", "Phonics & English Mastery Through Play"),
        ["TrustAudio"] = ("نطق تفاعلي", "Interactive Audio"),
        ["TrustQuality"] = ("كوشيه 350 جم", "350gsm Cardstock"),
        ["TrustShipping"] = ("شحن لمصر", "Egypt Shipping"),
        ["BrowseByCategory"] = ("تصفحي حسب المجموعة", "Browse by Category"),
        ["SearchPlaceholder"] = ("ابحثي عن بطاقات أو صف دراسي", "Search flashcards or grade..."),
        ["SearchBtn"] = ("بحث", "Search"),
        ["SortByLabel"] = ("ترتيب حسب", "Sort by"),
        ["SortTitle"] = ("ترتيب المنتجات", "Sort Products"),
        ["ClearFilters"] = ("مسح الفلاتر", "Clear Filters"),
        ["FeaturedTitle"] = ("المنتجات المميزة", "Featured Packs"),
        ["BrowseBannerText"] = ("استكشفي جميع بطاقات الفلاش كاردز\nوجميع المستويات", "Explore All Flashcard Packs\n& Grade Levels"),
        ["AllCardsTitle"] = ("اكتشفي كل البطاقات والمستويات", "Discover All Packs & Levels"),
        ["AddToCart"] = ("أضف للسلة", "Add to Cart"),
        ["Previous"] = ("السابق", "Previous"),
        ["Next"] = ("التالي", "Next"),
        ["Currency"] = ("ج.م", "EGP"),
        ["CardsWord"] = ("بطاقة", "Cards"),

        // Product Detail
        ["DetailHeaderTitle"] = ("تفاصيل الباقة", "Pack Details"),
        ["TeacherVoiceTitle"] = ("بصوت ميس خلود النادي", "Miss Kholoud's Voice Note"),
        ["TeacherVoiceSubtitle"] = ("شرح ونطق صوتي حصري مسجّل لهذه الباقة", "Exclusive recorded pronunciation & guidance"),
        ["AboutPack"] = ("عن الباقة التعليمية", "About This Pack"),
        ["AudioStudioTitle"] = ("أستوديو النطق الصوتي التفاعلي", "Interactive Pronunciation Studio"),
        ["AudioStudioSubtitle"] = ("اضغطي على أي كلمة للنطق الصحيح", "Tap any word to hear pronunciation"),
        ["PlayAllWords"] = ("▶ استمع لكل الكلمات بالترتيب", "▶ Play All Words in Sequence"),
        ["ReviewsTitle"] = ("آراء أولياء الأمور والمعلمات", "Parents & Teachers Reviews"),
        ["AddReviewTitle"] = ("أضيفي تقييمك", "Write a Review"),
        ["NamePlaceholder"] = ("الاسم", "Your Name"),
        ["RolePlaceholder"] = ("الصفة (ولي أمر / معلمة / مدرسة)", "Role (Parent / Teacher / School)"),
        ["CommentPlaceholder"] = ("اكتبي رأيك وتجربتك (5 أحرف على الأقل)", "Share your experience (min 5 chars)"),
        ["SubmitReview"] = ("إرسال التقييم", "Submit Review"),

        // Wishlist & Cart
        ["WishlistHeader"] = ("قائمة المفضلة", "My Wishlist"),
        ["WishlistSub"] = ("الباقات المحفوظة للعودة لاحقاً", "Saved flashcard packs"),
        ["WishlistEmpty"] = ("لم تضيفي أي باقة للمفضلة بعد", "Your wishlist is empty"),
        ["WishlistEmptyHint"] = ("اضغطي على علامة القلب في أي باقة لحفظها هنا.", "Tap the heart icon on any pack to save it here."),
        ["ViewBtn"] = ("عرض", "View"),
        ["ToCartBtn"] = ("للسلة", "To Cart"),
        ["RemoveBtn"] = ("إزالة", "Remove"),
        ["CartHeader"] = ("سلة التسوق", "Shopping Cart"),
        ["CartSub"] = ("مراجعة الباقات قبل إتمام الطلب", "Review your packs before checkout"),
        ["CartEmpty"] = ("سلتك فارغة. ابدئي باكتشاف البطاقات التعليمية.", "Your cart is empty. Start exploring flashcards!"),
        ["BrowseStore"] = ("تصفحي المتجر", "Browse Store"),
        ["SubtotalLabel"] = ("الإجمالي قبل الشحن", "Subtotal (before shipping)"),
        ["ProceedCheckout"] = ("إتمام الشراء", "Proceed to Checkout"),
        ["ClearCart"] = ("إفراغ السلة", "Clear Cart"),
        ["DeleteBtn"] = ("حذف", "Delete"),

        // Checkout & Payment & Track
        ["CheckoutHeader"] = ("إتمام الشراء", "Checkout"),
        ["CheckoutSub"] = ("الشحن والدفع الآمن داخل مصر", "Fast shipping & secure payment in Egypt"),
        ["ConfirmOrderBtn"] = ("تأكيد الطلب الآن", "Confirm Order Now"),
        ["PaymentHeader"] = ("تعليمات الدفع", "Payment Instructions"),
        ["PaymentSub"] = ("تحويل المحافظ الإلكترونية وإنستاباي", "Mobile Wallets & InstaPay"),
        ["CopyBtn"] = ("نسخ", "Copy"),
        ["PickReceipt"] = ("اختيار صورة الإيصال", "Choose Receipt Image"),
        ["SendReceipt"] = ("إرسال الإيصال وتأكيد الحجز", "Upload Receipt & Confirm"),
        ["TrackHeader"] = ("تتبع طلبك", "Track Your Order"),
        ["TrackSub"] = ("متابعة حالة الشحنة والدفع خطوة بخطوة", "Follow your order & payment status"),
        ["OrderNumLabel"] = ("رقم الطلب", "Order Number"),
        ["PhoneLabel"] = ("رقم الهاتف", "Phone Number"),
        ["SearchOrderBtn"] = ("ابحث عن الطلب", "Track Order"),

        // Account / Login / Register / Onboarding
        ["AccountHeader"] = ("حسابي والإعدادات", "Account & Settings"),
        ["AccountSub"] = ("إدارة حسابك وتفضيلات التطبيق", "Manage your account & app preferences"),
        ["WelcomeBack"] = ("أهلاً بعودتك", "Welcome Back"),
        ["LoginSub"] = ("سجّلي الدخول لمتابعة طلباتك وبطاقات طفلك", "Sign in to manage orders & your child's packs"),
        ["EmailLabel"] = ("البريد الإلكتروني", "Email Address"),
        ["PasswordLabel"] = ("كلمة المرور", "Password"),
        ["LoginBtn"] = ("—   تسجيل الدخول   —", "—   Sign In   —"),
        ["ForgotPassword"] = ("نسيت كلمة المرور؟", "Forgot Password?"),
        ["OrText"] = ("أو", "OR"),
        ["WhatsAppBtn"] = ("تواصلي معنا عبر واتساب", "Chat on WhatsApp"),
        ["NoAccount"] = ("ليس لديك حساب؟", "Don't have an account?"),
        ["CreateAccount"] = ("إنشاء حساب جديد", "Create New Account"),
        ["QuickAccess"] = ("الوصول السريع", "Quick Access"),
        ["HelpSupport"] = ("المساعدة والإعدادات", "Help & Settings"),
        ["SwitchLanguageRow"] = ("تغيير اللغة (العربية / English)", "Switch Language (English / العربية)"),
        ["ReplayOnboarding"] = ("إعادة عرض الجولة التعريفية", "Replay App Walkthrough"),
        ["WhatsAppDirect"] = ("تواصل مباشر مع ميس خلود (واتساب)", "Direct WhatsApp Support"),
        ["LogoutBtn"] = ("تسجيل الخروج", "Sign Out"),
        ["SkipBtn"] = ("تخطي", "Skip")
    };
}
