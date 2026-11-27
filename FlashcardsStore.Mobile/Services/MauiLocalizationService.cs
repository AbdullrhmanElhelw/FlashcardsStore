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

    public string Format(string key, params object?[] arguments)
        => string.Format(System.Globalization.CultureInfo.InvariantCulture, Get(key), arguments);

    public string GetLocalized(string? arabic, string? english)
    {
        if (IsRtl)
            return !string.IsNullOrWhiteSpace(arabic) ? arabic : (english ?? string.Empty);
        return !string.IsNullOrWhiteSpace(english) ? english : (arabic ?? string.Empty);
    }

    public string TranslateTargetAudience(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
        var text = raw.Trim();
        if (IsRtl) return text;

        return text
            .Replace("ابتدائي (الصفوف 1-3)", "Primary (Grades 1-3)")
            .Replace("ابتدائي (الصفوف 4-6)", "Primary (Grades 4-6)")
            .Replace("رياض الأطفال (KG)", "Kindergarten (KG)")
            .Replace("تأسيس (KG)", "Foundation (KG)")
            .Replace("إعدادي", "Preparatory")
            .Replace("الأطفال من سن 4 إلى 7 سنوات", "Ages 4 to 7")
            .Replace("سن 6 إلى 10 سنوات", "Ages 6 to 10")
            .Replace("سن 7 إلى 13 سنة", "Ages 7 to 13")
            .Replace("أولياء الأمور والمعلمين", "Parents & Teachers")
            .Replace("أولياء الأمور", "Parents")
            .Replace("المعلمين والمعلمات", "Teachers")
            .Replace("المعلمين", "Teachers")
            .Replace("الصفوف", "Grades");
    }

    public string TranslateGradeLevel(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
        var key = raw.Trim().ToLowerInvariant();
        return key switch
        {
            "kg" or "kg1-kg2" or "kg-1-2" => IsRtl ? "رياض الأطفال (KG1 - KG2)" : "Kindergarten (KG1 - KG2)",
            "primary-1" => IsRtl ? "الأول الابتدائي" : "Primary 1",
            "primary-2" => IsRtl ? "الثاني الابتدائي" : "Primary 2",
            "primary-3" => IsRtl ? "الثالث الابتدائي" : "Primary 3",
            "primary-1-3" => IsRtl ? "الصفوف 1 - 3 ابتدائي" : "Primary 1 - 3",
            "primary-4-6" => IsRtl ? "الصفوف 4 - 6 ابتدائي" : "Primary 4 - 6",
            "prep" or "prep-1-3" => IsRtl ? "المرحلة الإعدادية" : "Preparatory",
            "all" or "all-levels" => IsRtl ? "جميع المستويات" : "All Levels",
            _ => raw.Trim()
        };
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
        ["FiltersBtn"] = ("الفلاتر", "Filters"),
        ["FreeShippingRemaining"] = ("متبقي {0:0} ج.م للحصول على شحن مجاني 🚚", "Add {0:0} EGP more for FREE shipping 🚚"),
        ["FreeShippingUnlocked"] = ("مبروك! طلبك مؤهل لشحن مجاني ✓", "Congrats! You unlocked FREE shipping ✓"),
        ["WhatsAppOrderBtn"] = ("طلب عبر واتساب", "Order via WhatsApp"),
        ["SharePackBtn"] = ("مشاركة", "Share"),
        ["NoProductsFound"] = ("لا توجد باقات تطابق البحث", "No matching packs found"),
        ["ShowAllPacks"] = ("عرض كل الباقات", "Show All Packs"),
        ["TapToZoom"] = ("اضغط للتكبير", "Tap to zoom"),
        ["MyOrdersTitle"] = ("طلباتي السابقة", "My Previous Orders"),
        ["TapOrderToTrack"] = ("اضغط على أي طلب لعرض حالته فوراً", "Tap any order to track it immediately"),
        ["KeyPointsTitle"] = ("المواصفات والنقاط المميزة:", "Technical Specifications & Key Points:"),
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
        ["SkipBtn"] = ("تخطي", "Skip"),
        ["OnboardingDiscoverTitle"] = ("اكتشفي بطاقات الفلاش كاردز", "Explore Flashcards"),
        ["OnboardingDiscoverDescription"] = ("مجموعات تعليمية مصممة بعناية مع ميس خلود النادي لتأسيس طفلك في الفونكس والقواعد والمفردات.", "Thoughtfully designed learning packs from Miss Kholoud Elnady to build your child's phonics, grammar and vocabulary."),
        ["OnboardingChooseTitle"] = ("اختاري المناسب لعمر طفلك", "Choose the Right Level"),
        ["OnboardingChooseDescription"] = ("تصفحي حسب المرحلة الدراسية مع معاينة الكلمات، التقييمات، وتسجيلات المعلمة الصوتية.", "Browse by grade, preview words and reviews, and listen to the teacher's audio recordings."),
        ["OnboardingPracticeTitle"] = ("تعلّم تفاعلي وطلب سهل", "Learn and Order with Ease"),
        ["OnboardingPracticeDescription"] = ("استمعي للنطق الصحيح بالسرعة البطيئة للأطفال واطلبي الباقات بسهولة حتى باب البيت.", "Practice pronunciation at a child-friendly speed and have flashcard packs delivered to your door."),
        ["OnboardingNext"] = ("التالي", "Next"),
        ["OnboardingStart"] = ("ابدئي التسوق الآن", "Start Shopping"),

        // Shared form labels and actions
        ["RegisterHeader"] = ("إنشاء حساب جديد", "Create an Account"),
        ["RegisterSub"] = ("أنشئي حسابك لمتابعة طلباتك وبطاقات طفلك بسهولة", "Create an account to manage your orders and your child's flashcards."),
        ["FullNamePlaceholder"] = ("الاسم بالكامل", "Full name"),
        ["PhonePlaceholder"] = ("رقم الهاتف", "Phone number"),
        ["PasswordMinPlaceholder"] = ("كلمة المرور (8 أحرف على الأقل)", "Password (at least 8 characters)"),
        ["ConfirmPasswordPlaceholder"] = ("تأكيد كلمة المرور", "Confirm password"),
        ["TermsNotice"] = ("بإنشاء الحساب، أوافق على الشروط وسياسة الخصوصية.", "By creating an account, I agree to the Terms and Privacy Policy."),
        ["HaveAccount"] = ("لديك حساب بالفعل؟", "Already have an account?"),
        ["SignIn"] = ("تسجيل الدخول", "Sign in"),
        ["LoginEmailPlaceholder"] = ("أدخلي بريدك الإلكتروني", "Enter your email address"),
        ["LoginPasswordPlaceholder"] = ("أدخلي كلمة المرور", "Enter your password"),
        ["AcademyMember"] = ("عضو أكاديمية خلود النادي", "Kholoud Elnady Academy member"),
        ["QuickOrders"] = ("تتبع طلباتي", "Track my orders"),
        ["QuickWishlist"] = ("المفضلة", "Wishlist"),
        ["QuickCart"] = ("السلة", "Cart"),
        ["HelpHeading"] = ("المساعدة والدعم", "Help & Support"),
        ["WhatsAppSupport"] = ("تواصل مباشر مع ميس خلود (واتساب)", "Contact Miss Kholoud on WhatsApp"),
        ["ReviewTeacherVoice"] = ("بصوت ميس خلود النادي", "Narrated by Miss Kholoud Elnady"),
        ["ReviewTeacherVoiceSub"] = ("شرح ونطق صوتي حصري مسجّل لهذه الباقة", "Exclusive recorded explanation and pronunciation for this pack"),
        ["CustomerReviews"] = ("آراء أولياء الأمور والمعلمات", "Parents' and teachers' reviews"),
        ["GradeTarget"] = ("الصف الدراسي: {0}", "Grade level: {0}"),
        ["AudienceTarget"] = ("الفئة المستهدفة: {0}", "Target audience: {0}"),
        ["RatingTitle"] = ("التقييم", "Rating"),
        ["RoleReviewPlaceholder"] = ("الصفة (ولي أمر / معلمة / مدرسة)", "Role (parent / teacher / school)"),
        ["ReviewCommentPlaceholder"] = ("اكتبي رأيك وتجربتك (5 أحرف على الأقل)", "Share your experience (at least 5 characters)"),
        ["CartRemoveItem"] = ("حذف", "Remove"),
        ["CartSubtotal"] = ("الإجمالي قبل الشحن", "Subtotal (before shipping)"),
        ["CartItemsCount"] = ("{0} قطعة مختارة", "{0} items selected"),
        ["CheckoutCustomerSection"] = ("1. بيانات العميل والتواصل", "1. Customer information"),
        ["CheckoutContactIntro"] = ("أدخلي بيانات التواصل لتنسيق التوصيل بدقة.", "Enter your contact details to arrange delivery."),
        ["CustomerTypeLabel"] = ("نوع العميل", "Customer type"),
        ["CustomerNameRequired"] = ("الاسم بالكامل *", "Full name *"),
        ["PhoneRequired"] = ("رقم الهاتف *", "Phone number *"),
        ["WhatsAppOptional"] = ("رقم واتساب (اختياري)", "WhatsApp number (optional)"),
        ["EmailOptional"] = ("البريد الإلكتروني (اختياري)", "Email (optional)"),
        ["CheckoutAddressSection"] = ("2. عنوان التوصيل والمحافظة", "2. Delivery address"),
        ["GovernorateRequired"] = ("المحافظة *", "Governorate *"),
        ["SelectGovernorate"] = ("اختر المحافظة", "Select a governorate"),
        ["CityRequired"] = ("المدينة / المنطقة *", "City / area *"),
        ["CityPlaceholder"] = ("المدينة", "City"),
        ["AddressRequired"] = ("العنوان بالتفصيل *", "Detailed address *"),
        ["AddressPlaceholder"] = ("الشارع، رقم المبنى، الدور...", "Street, building and floor..."),
        ["NotesOptional"] = ("ملاحظات (اختياري)", "Notes (optional)"),
        ["NotesPlaceholder"] = ("ملاحظات إضافية", "Additional notes"),
        ["CheckoutPaymentSection"] = ("3. طريقة الدفع", "3. Payment method"),
        ["SelectPaymentMethod"] = ("اختر طريقة الدفع", "Select a payment method"),
        ["OrderTrackIntro"] = ("أدخلي رقم الطلب ورقم الهاتف الذي سجلته عند الشراء.", "Enter the order number and the phone number used at checkout."),
        ["OrderNumberExample"] = ("مثال: EGY-20261120-1234", "Example: EGY-20261120-1234"),
        ["OrderPhonePlaceholder"] = ("رقم الهاتف المسجل بالطلب", "Phone number used for the order"),
        ["OrderFormat"] = ("الطلب {0}", "Order {0}"),
        ["CustomerFormat"] = ("العميل: {0}", "Customer: {0}"),
        ["StatusFormat"] = ("الحالة: {0}", "Status: {0}"),
        ["ProductCountFormat"] = ("عدد المنتجات: {0}", "Items: {0}"),
        ["OrderTotalFormat"] = ("الإجمالي شامل الشحن: {0:0.##} ج.م", "Total including shipping: {0:0.##} EGP"),
        ["OrderNumberFormat"] = ("رقم الطلب: {0}", "Order number: {0}"),
        ["OrderPlacedSuccess"] = ("تم تسجيل طلبك بنجاح ✓", "Your order was placed successfully ✓"),
        ["PaymentTransferInstructions"] = ("حوّلي المبلغ إلى إحدى وسائل الدفع التالية ثم ارفعي صورة الإيصال.", "Transfer the amount using one of the methods below, then upload the receipt."),
        ["AvailableTransferDetails"] = ("بيانات التحويل المتاحة", "Available transfer details"),
        ["VodafoneCashLabel"] = ("فودافون كاش / المحافظ الذكية", "Vodafone Cash / mobile wallets"),
        ["RecipientNameFormat"] = ("اسم المستلم: {0}", "Recipient: {0}"),
        ["PaymentInstructionsHeading"] = ("تعليمات الدفع", "Payment instructions"),
        ["UploadTransferReceipt"] = ("رفع إيصال التحويل", "Upload transfer receipt"),
        ["WalletOrBank"] = ("المحفظة أو البنك", "Wallet or bank"),
        ["SelectWallet"] = ("اختر المحفظة", "Select a wallet"),
        ["SenderPhone"] = ("رقم الهاتف المحول منه", "Sender phone number"),
        ["TransactionReferenceOptional"] = ("رقم العملية (اختياري)", "Transaction reference (optional)"),
        ["TransactionReference"] = ("رقم العملية", "Transaction reference"),
        ["ReceiptSelected"] = ("اختيار صورة الإيصال", "Choose receipt image"),
        ["ReceiptNotSelected"] = ("لم يتم اختيار صورة إيصال", "No receipt image selected"),
        ["PickTransferReceipt"] = ("اختر صورة إيصال التحويل", "Choose a transfer receipt image"),
        ["PaymentInstructionsAr"] = ("تعليمات الدفع", "Payment instructions"),
        ["PaymentSuccess"] = ("تم الإرسال ✓", "Sent ✓"),
        ["ReceiptUploaded"] = ("تم رفع إيصال التحويل بنجاح وجارٍ التحقق منه من قبل الأكاديمية.", "The receipt was uploaded successfully and is awaiting verification."),
        ["CopyVodafoneFormat"] = ("تم نسخ رقم فودافون كاش: {0}", "Copied Vodafone Cash number: {0}"),
        ["CopyInstaPayFormat"] = ("تم نسخ عنوان InstaPay: {0}", "Copied InstaPay address: {0}"),
        ["NoReviewsYet"] = ("لا توجد تقييمات بعد", "No reviews yet"),
        ["NoReviewsBeFirst"] = ("لا توجد تقييمات بعد — كوني أول من يقيّم الباقة", "No reviews yet — be the first to review this pack"),
        ["ReviewSummaryFormat"] = ("⭐ {0:0.0} من 5  ·  {1} تقييم", "⭐ {0:0.0} out of 5 · {1} reviews"),
        ["AvailableQuantityFormat"] = ("متوفر — الكمية المتاحة: {0}", "In stock — {0} available"),
        ["Unavailable"] = ("غير متوفر حالياً", "Currently unavailable"),
        ["Available"] = ("متوفر", "Available"),
        ["SavedFavorite"] = ("في المفضلة ♥", "Saved ♥"),
        ["AddFavorite"] = ("أضف للمفضلة ♡", "Add to wishlist ♡"),
        ["KidSlow"] = ("بطيء للأطفال (0.75x)", "Kid slow (0.75x)"),
        ["NormalSpeed"] = ("سرعة طبيعية (1.0x)", "Normal speed (1.0x)"),
        ["Stop"] = ("إيقاف", "Stop"),
        ["Play"] = ("تشغيل", "Play"),
        ["ValidationRequired"] = ("يرجى استكمال جميع البيانات.", "Please complete all fields."),
        ["ValidationEmail"] = ("أدخلي بريداً إلكترونياً صحيحاً.", "Enter a valid email address."),
        ["ValidationPhone"] = ("أدخلي رقم هاتف صحيحاً.", "Enter a valid phone number."),
        ["ValidationPasswordLength"] = ("يجب ألا تقل كلمة المرور عن 8 أحرف.", "Password must be at least 8 characters long."),
        ["ValidationPasswordMatch"] = ("كلمتا المرور غير متطابقتين.", "Passwords do not match."),
        ["ValidationLoginRequired"] = ("يرجى إدخال البريد الإلكتروني وكلمة المرور.", "Enter your email address and password."),
        ["ValidationOrderRequired"] = ("أدخل رقم الطلب ورقم الهاتف.", "Enter the order number and phone number."),
        ["ValidationCartEmpty"] = ("السلة فارغة. أضيفي منتجاً قبل إتمام الشراء.", "Your cart is empty. Add an item before checkout."),
        ["ValidationCheckoutRequired"] = ("أدخلي الاسم، الهاتف، المدينة، والعنوان بالتفصيل.", "Enter your name, phone, city and full address."),
        ["ValidationEgyptPhone"] = ("أدخلي رقم هاتف مصري صحيح.", "Enter a valid Egyptian phone number."),
        ["AlertTitle"] = ("تنبيه", "Notice"),
        ["ErrorTitle"] = ("خطأ", "Error"),
        ["Okay"] = ("حسناً", "OK"),
        ["Yes"] = ("نعم", "Yes"),
        ["No"] = ("لا", "No"),
        ["CurrencyAmount"] = ("{0:0.##} ج.م", "{0:0.##} EGP"),
        ["Rating5Stars"] = ("5 نجوم ★★★★★", "5 stars ★★★★★"),
        ["Rating4Stars"] = ("4 نجوم ★★★★", "4 stars ★★★★"),
        ["Rating3Stars"] = ("3 نجوم ★★★", "3 stars ★★★"),
        ["Rating2Stars"] = ("2 نجمتان ★★", "2 stars ★★"),
        ["Rating1Star"] = ("1 نجمة ★", "1 star ★"),
        ["ByTeacher"] = ("بصوت المعلمة", "Teacher narration"),
        ["WriteReview"] = ("أضيفي تقييمك", "Write a Review"),
        ["StartOnboarding"] = ("مرحباً بكِ", "Welcome"),
        ["ProductDetailTitle"] = ("تفاصيل المنتج", "Product Details"),
        ["ImageCountFormat"] = ("الصورة {0} من {1}", "Image {0} of {1}"),
        ["ProductCardCountFormat"] = ("{0} بطاقة", "{0} cards"),
        ["CartTitle"] = ("السلة", "Cart"),
        ["WishlistTitle"] = ("المفضلة", "Wishlist"),
        ["CheckoutTitle"] = ("إتمام الشراء", "Checkout"),
        ["TrackOrderTitle"] = ("تتبع الطلب", "Track Order"),
        ["PaymentTitle"] = ("تعليمات الدفع", "Payment Instructions"),
        ["AccountTitle"] = ("حسابي", "My Account"),
        ["LoginWhatsAppForgot"] = ("مرحباً مس خلود، نسيت كلمة المرور الخاصة بحسابي في أكاديمية خلود النادي.", "Hello Miss Kholoud, I forgot the password for my Kholoud Elnady Academy account."),
        ["LoginWhatsAppInquiry"] = ("مرحباً مس خلود، لدي استفسار بخصوص بطاقات الفلاش كاردز.", "Hello Miss Kholoud, I have a question about the flashcards."),
        ["EmailInvalid"] = ("أدخلي بريداً إلكترونياً صحيحاً.", "Enter a valid email address."),
        ["ContactPhoneInvalid"] = ("أدخلي رقم هاتف صحيحاً.", "Enter a valid phone number."),
        ["ThankYou"] = ("شكراً لكِ ✓", "Thank you ✓"),
        ["ReviewSubmitted"] = ("تم إرسال تقييمك للمراجعة وسيظهر بعد الاعتماد.", "Your review was submitted and will appear after approval."),
        ["ReviewRequired"] = ("أكمل الاسم والصفة واكتب تعليقاً من 5 أحرف على الأقل.", "Enter your name and role, and write a comment of at least 5 characters."),
        ["AddToCartToast"] = ("تمت إضافة «{0}» إلى السلة", "Added “{0}” to your cart"),
        ["LoginRequiredCheckout"] = ("يرجى تسجيل الدخول أو إنشاء حساب جديد لإتمام عملية الشراء.", "Sign in or create an account to complete checkout."),
        ["LogoutConfirmTitle"] = ("تسجيل الخروج", "Sign out"),
        ["LogoutConfirmMessage"] = ("هل تريدين تسجيل الخروج من حسابك؟", "Are you sure you want to sign out?"),
        ["LogoutSuccess"] = ("تم تسجيل الخروج بنجاح", "You have signed out successfully"),
        ["WelcomeCustomer"] = ("أهلاً بعودتك يا {0}", "Welcome back, {0}"),
        ["RegisterSuccess"] = ("تم إنشاء الحساب بنجاح، أهلاً بكِ", "Your account is ready. Welcome!"),
        ["OrderPlacedToast"] = ("تم تسجيل الطلب ✓", "Order placed ✓"),
        ["EmptyCartAlert"] = ("السلة فارغة", "Your cart is empty"),
        ["EmptyCartConfirm"] = ("هل تريدين إزالة جميع المنتجات من السلة؟", "Remove all items from your cart?"),
        ["ReceiptRequired"] = ("اختر صورة الإيصال أولاً", "Choose a receipt image first"),
        ["WalletTransferOption"] = ("تحويل محفظة إلكترونية / InstaPay", "Wallet transfer / InstaPay"),
        ["WhatsAppPaymentOption"] = ("التواصل عبر واتساب", "Contact via WhatsApp"),
        ["ParentOption"] = ("ولي أمر", "Parent"),
        ["TeacherOption"] = ("معلمة", "Teacher"),
        ["SchoolOption"] = ("مدرسة", "School"),
        ["Governorate.Cairo"] = ("القاهرة", "Cairo"),
        ["Governorate.Giza"] = ("الجيزة", "Giza"),
        ["Governorate.Alexandria"] = ("الإسكندرية", "Alexandria"),
        ["Governorate.Qalyubia"] = ("القليوبية", "Qalyubia"),
        ["Governorate.Sharqia"] = ("الشرقية", "Sharqia"),
        ["Governorate.Dakahlia"] = ("الدقهلية", "Dakahlia"),
        ["Governorate.Gharbia"] = ("الغربية", "Gharbia"),
        ["Governorate.Monufia"] = ("المنوفية", "Monufia"),
        ["Governorate.Beheira"] = ("البحيرة", "Beheira"),
        ["Governorate.Damietta"] = ("دمياط", "Damietta"),
        ["Governorate.KafrElSheikh"] = ("كفر الشيخ", "Kafr El Sheikh"),
        ["Governorate.PortSaid"] = ("بورسعيد", "Port Said"),
        ["Governorate.Ismailia"] = ("الإسماعيلية", "Ismailia"),
        ["Governorate.Suez"] = ("السويس", "Suez"),
        ["Governorate.Fayoum"] = ("الفيوم", "Fayoum"),
        ["Governorate.BeniSuef"] = ("بني سويف", "Beni Suef"),
        ["Governorate.Minya"] = ("المنيا", "Minya"),
        ["Governorate.Assiut"] = ("أسيوط", "Assiut"),
        ["Governorate.Sohag"] = ("سوهاج", "Sohag"),
        ["Governorate.Qena"] = ("قنا", "Qena"),
        ["Governorate.Luxor"] = ("الأقصر", "Luxor"),
        ["Governorate.Aswan"] = ("أسوان", "Aswan"),
        ["Governorate.RedSea"] = ("البحر الأحمر", "Red Sea"),
        ["Governorate.Matrouh"] = ("مطروح", "Matrouh"),
        ["OrderPaymentReceived"] = ("تم استلام إيصال الدفع وجارٍ التحقق منه من قبل الأكاديمية ✓", "Payment receipt received and awaiting verification ✓"),
        ["OrderPaymentMissing"] = ("يرجى رفع إيصال الدفع من صفحة تعليمات الدفع لتأكيد الحجز.", "Upload your payment receipt from the payment instructions page to confirm your order."),
        ["OrderNoProofRequired"] = ("لا توجد مستندات دفع مطلوبة لهذا الطلب.", "No payment documents are required for this order."),
        ["ShippingFree"] = ("مجاني 🎉", "Free 🎉"),
        ["ShippingSummary"] = ("المجموع: {0:0.##} ج.م   ·   الشحن: {1}\nالإجمالي النهائي: {2:0.##} ج.م", "Subtotal: {0:0.##} EGP · Shipping: {1}\nTotal: {2:0.##} EGP"),
        ["SubtotalBeforeShipping"] = ("المجموع قبل الشحن: {0:0.##} ج.م", "Subtotal before shipping: {0:0.##} EGP"),
        ["OrderTotalToast"] = ("رقم الطلب: {0}\nالإجمالي: {1:0.##} ج.م", "Order number: {0}\nTotal: {1:0.##} EGP"),
        ["PaymentRequiredStatus"] = ("يرجى رفع إيصال الدفع من صفحة تعليمات الدفع لتأكيد الحجز.", "Upload the payment receipt from the payment instructions page to confirm the order."),
        ["Copied"] = ("تم النسخ", "Copied"),
        ["ClearCartTitle"] = ("السلة", "Cart"),
        ["ServerAddressError"] = ("أدخل عنوان خادم صحيحاً يبدأ بـ http:// أو https://", "Enter a valid server address starting with http:// or https://"),
        ["ReceiptTypesError"] = ("الصيغ المسموحة للإيصال هي JPG أو PNG أو WEBP", "Receipt must be a JPG, PNG or WEBP image"),
        ["ServerIncomplete"] = ("استجاب الخادم ببيانات غير مكتملة", "The server returned incomplete data"),
        ["ServerReadError"] = ("تعذر قراءة استجابة الخادم: {0}", "Could not read the server response: {0}"),
        ["ServerConnectionError"] = ("تعذر الاتصال بالخادم: {0}", "Could not connect to the server: {0}"),
        ["ServerUnauthorized"] = ("يرجى تسجيل الدخول للمتابعة", "Please sign in to continue"),
        ["ServerNotFound"] = ("لم يتم العثور على البيانات المطلوبة", "The requested data could not be found"),
        ["ServerRequestFailed"] = ("فشل الطلب ({0})", "Request failed ({0})"),
        ["Cancel"] = ("إلغاء", "Cancel"),
        ["SelectFromList"] = ("اختاري من القائمة", "Choose an option"),
        ["CartProductUnavailable"] = ("هذا المنتج غير متوفر حالياً", "This product is currently unavailable"),
        ["CartRestoreWarning"] = ("تعذر استعادة بيانات محفوظة تالفة؛ تم حذفها ويمكنك متابعة استخدام المتجر.", "Some saved data was corrupted and has been cleared. You can continue shopping."),
        ["EditProfileRow"] = ("تعديل بيانات الحساب", "Edit Profile"),
        ["EditProfileNamePlaceholder"] = ("الاسم بالكامل", "Full name"),
        ["EditProfilePhonePlaceholder"] = ("رقم الهاتف", "Phone number"),
        ["EditProfileAddressPlaceholder"] = ("عنوان التوصيل (اختياري)", "Shipping address (optional)"),
        ["EditProfileCityPlaceholder"] = ("المدينة / المنطقة (اختياري)", "City / Area (optional)"),
        ["EditProfileGovernoratePlaceholder"] = ("المحافظة (اختياري)", "Governorate (optional)"),
        ["SaveProfileBtn"] = ("حفظ البيانات", "Save Changes"),
        ["ProfileUpdatedToast"] = ("تم تحديث بيانات حسابك بنجاح ✓", "Profile updated successfully ✓"),
        ["ProfileValidationRequired"] = ("الاسم ورقم الهاتف مطلوبان", "Full name and phone number are required"),
        ["ChangePasswordRow"] = ("تغيير كلمة المرور", "Change Password"),
        ["CurrentPasswordPlaceholder"] = ("كلمة المرور الحالية", "Current password"),
        ["NewPasswordPlaceholder"] = ("كلمة المرور الجديدة (6 أحرف على الأقل)", "New password (min 6 chars)"),
        ["ConfirmNewPasswordPlaceholder"] = ("تأكيد كلمة المرور الجديدة", "Confirm new password"),
        ["SavePasswordBtn"] = ("تحديث كلمة المرور", "Update Password"),
        ["PasswordChangedToast"] = ("تم تغيير كلمة المرور بنجاح ✓", "Password changed successfully ✓"),
        ["PasswordChangeRequired"] = ("أدخلي كلمة المرور الحالية والجديدة", "Enter current and new password"),
        ["PasswordMinLength"] = ("كلمة المرور الجديدة يجب أن تكون 6 أحرف على الأقل", "New password must be at least 6 characters"),
        ["PasswordMismatch"] = ("كلمتا المرور غير متطابقتين", "New passwords do not match"),
        ["DeleteAccountBtn"] = ("حذف الحساب نهائياً", "Delete Account"),
        ["DeleteAccountConfirmTitle"] = ("حذف الحساب", "Delete Account"),
        ["DeleteAccountConfirmMessage"] = ("هل أنتِ متأكدة من حذف حسابك نهائياً؟ سيتم مسح بياناتك الشخصية ولن تتمكني من تسجيل الدخول بهذا الحساب مرة أخرى.", "Are you sure you want to permanently delete your account? Your personal data will be erased and you will no longer be able to sign in with it."),
        ["AccountDeletedToast"] = ("تم حذف الحساب بنجاح", "Account deleted successfully")
    };
}
