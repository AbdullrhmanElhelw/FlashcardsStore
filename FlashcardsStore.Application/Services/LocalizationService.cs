using System.Globalization;
using FlashcardsStore.Application.Interfaces.Services;

namespace FlashcardsStore.Application.Services;

/// <summary>
/// Provides localized dictionary entries for Arabic and English.
/// Follows Single Responsibility Principle for UI string translations.
/// </summary>
public class LocalizationService : ILocalizationService
{
    private static readonly Dictionary<string, (string Ar, string En)> Translations = new(StringComparer.OrdinalIgnoreCase)
    {
        // Brand & Header
        { "BrandName", ("أكاديمية خلود النادي", "Kholoud Elnady Academy") },
        { "BrandSubtitle", ("English Academy • بطاقات تعليمية", "English Academy • Educational Flashcards") },
        { "NavHome", ("الرئيسية", "Home") },
        { "NavCatalog", ("المتجر والكتالوج", "Store & Catalog") },
        { "NavShopNow", ("تسوق الآن", "Shop Now") },
        { "NavPhonics", ("صوتيات", "Phonics") },
        { "NavGrammar", ("قواعد", "Grammar") },
        { "NavVocabulary", ("مفردات", "Vocabulary") },
        { "NavSightWords", ("كلمات بصرية", "Sight Words") },
        { "NavTrackOrder", ("تتبع طلبك", "Track Order") },
        { "NavCart", ("السلة", "Cart") },
        { "NavAdmin", ("لوحة التحكم", "Admin Panel") },

        // Home Hero Section
        { "HeroEyebrow", ("البطاقات الأصلية المعتمدة • إشراف وتصميم ميس خلود النادي", "Official Certified Flashcards • Supervised by Miss Kholoud Elnady") },
        { "HeroTitle1", ("تعليم إنجليزي حسي وممتع", "Sensory & Engaging English Learning") },
        { "HeroTitle2", ("يثبت في ذاكرة طفلك للأبد", "That Truly Sticks in Your Child's Mind") },
        { "HeroSubtitle", ("طريقة تعليمية تفاعلية تجمع بين متعة اللمس باليد، جاذبية الصور بالألوان، والاستماع لنطق المتحدثين الأصليين. صُممت بعناية لمناهج المدارس الرسمية واللغات ومناهج Connect و Connect Plus لتمنح طفلك تفوقاً حقيقياً بثقة وسهولة.", "An interactive learning approach combining tactile cards, vibrant illustrations, and native speaker audio. Carefully aligned with official and language school curricula (Connect & Connect Plus) to build lasting confidence.") },
        { "HeroCtaExplore", ("تصفح وشراء الباقات من المتجر", "Explore & Buy Flashcards") },
        { "HeroCtaWhatsApp", ("استشارة وطلب عبر واتساب", "WhatsApp Advice & Orders") },
        { "MetricFamilies", ("+5,000 طفل وأسرة مستفيدة", "+5,000 Happy Families") },
        { "MetricSchools", ("150+ مدرسة وحضانة معتمدة", "150+ Partner Schools & Nurseries") },
        { "MetricPaper", ("350 جم كوشيه بسلوفان حراري", "350gsm Thermal Laminated Cardstock") },
        { "MetricWaterproof", ("100% مقاوم للماء والتلف", "100% Waterproof & Durable") },

        // Home Featured Section
        { "FeaturedSectionTitle", ("باقات فلاش كاردز مختارة للتأسيس", "Featured Foundation Flashcard Packs") },
        { "FeaturedSectionSubtitle", ("أكثر الباقات طلباً وتقييماً من أولياء الأمور والمعلمات لإتقان مهارات القراءة والكتابة والتحدث", "Our most requested and top-rated packs by parents and teachers for English mastery") },
        { "ViewAllStorePacks", ("عرض جميع باقات المتجر بالكامل", "View All Academy Flashcards") },
        { "CalloutTitle", ("هل تبحث عن مهارة أو مرحلة معينة؟", "Looking for a specific skill or grade level?") },
        { "CalloutSubtitle", ("استخدم صفحة المتجر المستقلة للفلترة المباشرة بأسلوب أمازون بالسن، المادة، والسعر مع إمكانية البحث الفوري.", "Use our dedicated store page to filter by grade, skill, and price with instant search.") },
        { "CalloutCta", ("انتقل لصفحة المتجر والتسوق", "Go to Store & Catalog") },

        // Catalog & Filters
        { "CatalogHeaderBadge", ("البطاقات الأصلية المعتمدة • تصميم وإشراف ميس خلود النادي", "Official Certified Flashcards • Supervised by Miss Kholoud Elnady") },
        { "CatalogHeaderTitle", ("بطاقات فلاش كاردز تفاعلية لتأسيس اللغة الإنجليزية وإتقانها", "Interactive English Flashcards for Kids & Young Learners") },
        { "CatalogHeaderSubtitle", ("كوشيه مقوى 350 جم مغلف بسلوفان حراري ضد الماء • شحن سريع لجميع محافظات مصر • دفع فودافون كاش وإنستاباي", "Premium 350gsm coated cardstock with waterproof thermal lamination • Fast delivery across Egypt") },
        { "WhatsAppInquire", ("استفسر عبر واتساب", "Inquire via WhatsApp") },
        { "BrowsePacks", ("تصفح الباقات", "Browse Packs") },
        { "CatalogTitle", ("الكتالوج", "Catalog") },
        { "PacksCount", ("{0} باقة", "{0} packs") },
        { "SearchPlaceholder", ("ابحث عن باقة أو مهارة...", "Search packs or skills...") },
        { "SortBy", ("الترتيب حسب:", "Sort by:") },
        { "SortNewest", ("الأحدث", "Newest") },
        { "SortPriceAsc", ("السعر: الأقل أولاً", "Price: Low to High") },
        { "SortPriceDesc", ("السعر: الأعلى أولاً", "Price: High to Low") },
        { "SortName", ("الاسم: أ→ي", "Name: A to Z") },
        { "GradeLevel", ("المرحلة الدراسية", "Grade Level") },
        { "SkillCategory", ("المهارة اللغوية", "Language Skill") },
        { "AllGrades", ("جميع المراحل", "All Grades") },
        { "GradeKg", ("روضة وتأسيس (KG)", "Kindergarten (KG)") },
        { "GradePrimary13", ("ابتدائي (الصفوف 1-3)", "Primary (Grades 1-3)") },
        { "GradePrimary46", ("ابتدائي (الصفوف 4-6)", "Primary (Grades 4-6)") },
        { "GradePrep", ("إعدادي (Prep)", "Prep & Beyond") },
        { "AllSkills", ("جميع المهارات", "All Skills") },
        { "NoProductsFound", ("لا توجد باقات تطابق معايير البحث", "No packs found matching your criteria") },
        { "NoProductsFoundDesc", ("جرّب تعديل الفلتر أو مسح كلمة البحث للاطلاع على جميع الباقات المتاحة.", "Try changing filters or clearing search to view all available packs.") },
        { "ViewAllPacks", ("عرض جميع الباقات", "View All Packs") },

        // Product Cards
        { "AddToCart", ("أضف للسلة", "Add to Cart") },
        { "Details", ("التفاصيل", "Details") },
        { "InStock", ("متوفر", "In Stock") },
        { "OutOfStock", ("نفد", "Out of Stock") },
        { "FlipCard", ("اقلب الكارت", "Flip Card") },
        { "CardContent", ("محتوى الكارت", "Card Content") },
        { "FrontFace", ("الوجه الأمامي ↺", "Front ↺") },
        { "Currency", ("ج.م", "EGP") },
        { "CardsCountLabel", ("{0} كارت", "{0} cards") },
        { "PriceLabel", ("السعر:", "Price:") },

        // Product Details Page
        { "BreadcrumbHome", ("الرئيسية", "Home") },
        { "PackPrice", ("سعر الباقة للطلب الفردي أو الجملة:", "Pack Price (Single or Bulk):") },
        { "EgyptianPound", ("جنيه مصري", "Egyptian Pounds (EGP)") },
        { "ReadyToShip", ("متوفر وجاهز للشحن الفوري", "In Stock & Ready to Ship") },
        { "AddToCartFull", ("أضف إلى سلة المشتريات", "Add to Shopping Cart") },
        { "OrderViaWhatsApp", ("طلب سريع ومباشر عبر واتساب", "Direct Order via WhatsApp") },
        { "TechnicalSpecs", ("المواصفات والخصائص الفنية للباقة", "Technical Specifications & Materials") },
        { "PackDescription", ("الوصف والتفاصيل الكاملة للباقة", "Full Pack Description & Details") },
        { "FeatureCardsCount", ("عدد البطاقات:", "Card Count:") },
        { "FeatureGrade", ("المرحلة المستهدفة:", "Target Stage:") },
        { "FeatureAudience", ("الفئة المستفيدة:", "Target Audience:") },
        { "FeatureCategory", ("القسم التعليمي:", "Category:") },
        { "FeaturePaper", ("الخامة:", "Material:") },
        { "FeaturePaperVal", ("ورق كوشيه فاخر 350 جم مغلف سلوفان حراري مزدوج", "Premium 350gsm cardstock with double-sided thermal lamination") },
        { "FeatureCorners", ("الأطراف:", "Corners:") },
        { "FeatureCornersVal", ("زوايا دائرية آمنة لحماية أصابع الأطفال", "Child-safe rounded corners") },
        { "FeatureAudio", ("النطق الصوتي:", "Pronunciation:") },
        { "FeatureAudioVal", ("كود QR للاستماع لنطق المتحدثين الأصليين", "QR code for native speaker audio pronunciation") },
        { "FlipCardTip", ("يمكنك تجربة قلب الكارت بالماوس أو الضغط عليه لمشاهدة الوجهين", "Click or hover over the card to flip and view both sides") },

        // Cart & Drawer
        { "CartTitle", ("سلة المشتريات", "Shopping Cart") },
        { "CartSubtitle", ("راجع البطاقات المختارة قبل الانتقال لصفحة إتمام الشراء والدفع", "Review your selected items before proceeding to checkout and payment") },
        { "ContinueShopping", ("متابعة التسوق في الكتالوج", "Continue Shopping") },
        { "SelectedPacks", ("{0} باقة مختارة", "{0} packs selected") },
        { "CartEmpty", ("سلة المشتريات فارغة", "Your shopping cart is empty") },
        { "CartEmptyDesc", ("تصفح بطاقات الأكاديمية واختر الباقة المناسبة لطفلك أو طلابك", "Browse our academy flashcards and pick the best pack for your child or students") },
        { "SubTotal", ("المجموع الفرعي:", "Subtotal:") },
        { "ShippingNote", ("الشحن يحسب بدقة في الخطوة التالية حسب المحافظة", "Shipping is calculated in the next step based on your governorate") },
        { "CheckoutButton", ("إتمام الشراء والطلب", "Proceed to Checkout") },
        { "ViewFullCart", ("عرض تفاصيل السلة الكاملة", "View Full Cart Details") },
        { "PricePerPack", ("للباقة", "per pack") },

        // Checkout Page
        { "CheckoutTitle", ("إتمام الطلب وتحديد الشحن", "Checkout & Shipping") },
        { "CheckoutSubtitle", ("يرجى ملء بيانات التوصيل بدقة لضمان سرعة تسليم باقاتك لباب منزلك", "Please enter your delivery details accurately for fast doorstep delivery") },
        { "CustomerInfo", ("بيانات المشتري والمستلم", "Customer & Delivery Information") },
        { "CustomerType", ("نوع المشتري:", "Customer Type:") },
        { "Parent", ("ولي أمر", "Parent") },
        { "ParentDesc", ("لتعليم الأطفال بالمنزل", "Home learning") },
        { "Teacher", ("معلم / أكاديمية", "Teacher / School") },
        { "TeacherDesc", ("للحضانات والمدارس والدروس", "Nurseries & tutoring") },
        { "Student", ("طالب / متعلم", "Student") },
        { "StudentDesc", ("للتعلم والتطوير الذاتي", "Self-study") },
        { "FullName", ("الاسم بالكامل *", "Full Name *") },
        { "Phone", ("رقم الهاتف (للتواصل والتسليم) *", "Phone Number *") },
        { "AltPhone", ("رقم هاتف بديل (اختياري)", "Alternative Phone (Optional)") },
        { "Governorate", ("المحافظة *", "Governorate *") },
        { "SelectGovernorate", ("-- اختر المحافظة لحساب الشحن بدقة --", "-- Select Governorate --") },
        { "Address", ("العنوان بالتفصيل (الشارع، رقم العقار، علامة مميزة) *", "Detailed Address (Street, Building, Landmark) *") },
        { "Notes", ("ملاحظات إضافية للتوصيل (اختياري)", "Delivery Notes (Optional)") },
        { "PaymentMethod", ("طريقة الدفع المناسبة لك", "Payment Method") },
        { "VodafoneCashTitle", ("فودافون كاش ومحافظ المحمول", "Vodafone Cash & Mobile Wallets") },
        { "VodafoneCashDesc", ("تحويل سهل من أي محفظة إلكترونية بمصر", "Easy transfer from any Egyptian wallet") },
        { "InstaPayTitle", ("إنستاباي (InstaPay)", "InstaPay Transfer") },
        { "InstaPayDesc", ("تحويل بنكي لحظي عبر تطبيق إنستاباي", "Instant bank transfer via InstaPay app") },
        { "OrderSummary", ("ملخص الطلب", "Order Summary") },
        { "Shipping", ("تكلفة الشحن:", "Shipping Fee:") },
        { "TotalAmount", ("الإجمالي النهائي:", "Total Amount:") },
        { "ConfirmOrder", ("تأكيد الطلب وحجز المنتجات", "Confirm Order & Reserve Items") },

        // Order Tracking
        { "TrackOrderTitle", ("تتبع حالة طلبك", "Track Your Order") },
        { "TrackOrderSubtitle", ("أدخل رقم الطلب ورقم الهاتف المسجل لمعرفة الموقف الحالي وإمكانية استكمال رفع إيصال التحويل", "Enter your order number and registered phone to view order progress and upload payment receipt") },
        { "OrderNumberLabel", ("رقم الطلب (Order Number)", "Order Number") },
        { "RegisteredPhoneLabel", ("رقم الهاتف المسجل بالطلب", "Registered Phone Number") },
        { "SearchOrder", ("بحث وتتبع الطلب", "Search & Track Order") },

        // Pagination
        { "PaginationShowing", ("عرض {0}–{1} من أصل {2} نتيجة", "Showing {0}–{1} of {2} results") },
        { "Previous", ("السابق", "Previous") },
        { "Next", ("التالي", "Next") },

        // Footer
        { "FooterDescription", ("منصة متخصصة في بطاقات الفلاش كاردز التعليمية الفاخرة لتأسيس اللغة الإنجليزية للأطفال وطلاب المدارس والحضانات، بإشراف المعلمة خلود النادي.", "Specialized platform for premium educational English flashcards for children, school, and nursery students, under the supervision of teacher Kholoud Elnady.") },
        { "CertifiedLanguages", ("تأسيس لغات معتمد", "Certified Language Foundation") },
        { "CurriculumAligned", ("Curriculum Aligned", "Curriculum Aligned") },
        { "CategoriesTitle", ("أقسام البطاقات", "Card Categories") },
        { "QuickLinksTitle", ("روابط سريعة", "Quick Links") },
        { "ContactTitle", ("طلب مباشر ودعم الأكاديمية", "Direct Orders & Support") },
        { "WhatsAppSupport", ("تواصل واتساب مع المعلمة", "WhatsApp with Teacher") },
        { "LocationCairo", ("القاهرة، جمهورية مصر العربية", "Cairo, Arab Republic of Egypt") },
        { "AllRightsReserved", ("جميع الحقوق محفوظة.", "All rights reserved.") },
        { "BackToTop", ("العودة للأعلى", "Back to Top") },

        // Language Switcher
        { "SwitchLanguage", ("English", "العربية") },
        { "CurrentLanguageName", ("العربية", "English") },

        // Home Section 1 - Card preview
        { "HeroSoundPreviewTitle", ("Short Vowel Sound", "Short Vowel Sound") },
        { "HeroPhonicsMastery", ("PHONICS MASTERY", "PHONICS MASTERY") },
        { "HeroCardPill", ("52 كارت • كوشيه 350 جم", "52 Cards • 350gsm Coated") },
        { "HeroCardLamination", ("مغلف بسلوفان حراري ضد الماء", "Waterproof Thermal Laminated") },

        // Home Section 2 - Methodology
        { "MethodologyEyebrow", ("المنهجية العلمية والتربوية للأكاديمية", "Scientific & Educational Methodology") },
        { "MethodologyTitle", ("فكرة المتجر: لماذا يتفوق الأطفال ببطاقات الفلاش كاردز؟", "Why Flashcards? How Children Excel") },
        { "MethodologySubtitle", ("تعتمد فلسفة ميس خلود النادي على تحويل تعلم اللغة الإنجليزية من حفظ تقليدي ممل إلى تجربة بصرية وحركية تفاعلية تُثبت المعلومة في الذاكرة الدائمة للطفل.", "Miss Kholoud Elnady's philosophy transforms English learning from dull rote memorization into an engaging sensory journey that cements knowledge in long-term memory.") },
        { "PillarActiveRecallTitle", ("التذكر النشط (Active Recall)", "Active Recall") },
        { "PillarActiveRecallDesc", ("بدلاً من قراءة الكلمة بشكل سلبي، يستثير الكارت ذهن الطفل للربط الفوري بين الصورة والصوت ومعنى الكلمة، وهو ما أثبتت الدراسات التربوية أنه يرفع استيعاب الطفل بنسبة تصل إلى 80%.", "Instead of passive reading, flashcards prompt immediate mental connections between image, phonics, and meaning—proven to boost retention by up to 80%.") },
        { "PillarSpacedRepetitionTitle", ("التكرار المتباعد الممتع (Spaced Repetition)", "Spaced Repetition") },
        { "PillarSpacedRepetitionDesc", ("تقسيم البطاقات إلى حزم صغيرة يتيح للأم والمعلمة تنظيم مراجعات سريعة وخفيفة على فترات متباعدة دون أي ضغط نفسي، مما يمنع نسيان الكلمات والقواعد.", "Bite-sized packs enable parents and teachers to conduct brief, regular reviews at spaced intervals without stress, locking vocabulary and grammar into memory.") },
        { "PillarSensoryAudioTitle", ("الربط الحسي والصوتي الشامل", "Sensory & Native Audio Connection") },
        { "PillarSensoryAudioDesc", ("بطاقة ملموسة بالأيدي، رسومات جذابة بالأعين، وباركود صوتي ذكي (QR Code) على كل باقة يمكن مسحه بالموبايل للاستماع إلى النطق الصحيح بأصوات متحدثين أصليين (Native Speakers).", "Tactile physical cards, vibrant visuals, and smart QR codes for instant mobile scanning to hear crystal-clear native speaker audio pronunciation.") },

        // Home Section 3 - Quality & Specifications
        { "SpecsEyebrow", ("مواصفات خامات وتصنيع فائقة الجودة", "PREMIUM PHYSICAL SPECIFICATIONS") },
        { "SpecsTitle", ("مواصفات وتصنيع صُمم ليدوم بين أيدي أطفالك", "Built to Last in Little Hands") },
        { "SpecsSubtitle", ("نعلم أن الأطفال يتعاملون مع البطاقات بحماس وشغف، لذلك لم نتنازل عن أعلى معايير الطباعة والمتانة في مصر.", "We know kids explore with excitement, so we never compromised on print quality, thickness, and child-safe durability.") },
        { "SpecPaperTitle", ("كوشيه مقوى 350 جم", "350gsm Heavyweight Cardstock") },
        { "SpecPaperDesc", ("ورق سميك شديد الصلابة لا ينثني بسهولة ويتحمل الاستخدام المكثف في الفصل والمنزل.", "Rigid, heavyweight paper that resists bending and withstands everyday classroom and home play.") },
        { "SpecWaterproofTitle", ("سلوفان حراري مزدوج (Waterproof)", "Double-Sided Thermal Lamination") },
        { "SpecWaterproofDesc", ("تغليف عازل للماء والسوائل والعصائر من الوجهين، وقابل للمسح والتنظيف الفوري بمنديل.", "100% waterproof sealed coating on both sides, easily wiped clean from spills, milk, or juice.") },
        { "SpecCornersTitle", ("زوايا دائرية ناعمة وآمنة", "Safe Rounded Corners") },
        { "SpecCornersDesc", ("قص دائري مخصص لضمان أمان تام لأيدي الأطفال وعيونهم دون أي حواف حادة.", "Precision die-cut rounded corners to ensure complete safety for toddlers and young children.") },
        { "SpecQrTitle", ("باركود النطق الذكي (QR Code)", "Smart Audio QR Code") },
        { "SpecQrDesc", ("امسح الباركود بكاميرا هاتفك في ثوانٍ للاستماع للنطق الصوتي الإنجليزي السليم.", "Instant camera scan to stream authentic native speaker pronunciation anytime, anywhere.") },

        // Home Section 4 - Academic Pathway
        { "PathwayEyebrow", ("تدرج تعليمي مدروس خطوة بخطوة", "Step-by-Step Academic Pathway") },
        { "PathwayTitle", ("خارطة السنوات والمراحل الدراسية (من أول KG وصاعداً)", "Academic Pathway: From KG to Prep") },
        { "PathwaySubtitle", ("باقات مخصصة لكل سن ومرحلة دراسية لضمان انتقال الطفل بسلاسة من مرحلة إلى أخرى دون أي فجوات.", "Tailored packs for every age and grade, ensuring a smooth transition without learning gaps.") },
        { "PathwayKgTitle", ("مرحلة التأسيس والروضة (KG)", "Foundation & Kindergarten (KG)") },
        { "PathwayKgAge", ("سن 4 إلى 6 سنوات", "Ages 4 to 6") },
        { "PathwayKgItem1", ("الحركات القصيرة (Short Vowels)", "Short Vowels & Phonics") },
        { "PathwayKgItem2", ("الكلمات الثلاثية الأولى (CVC Words)", "First CVC 3-Letter Words") },
        { "PathwayKgItem3", ("أول 500 كلمة مصورة للأطفال", "First 500 Illustrated Words") },
        { "PathwayKgCta", ("عرض باقات KG بالمتجر", "View KG Packs") },
        { "PathwayPrimary13Title", ("الصفوف الأولى (Primary 1 - 3)", "Early Primary (Grades 1 - 3)") },
        { "PathwayPrimary13Age", ("سن 6 إلى 9 سنوات", "Ages 6 to 9") },
        { "PathwayPrimary13Item1", ("الحروف المركبة (Blends & Digraphs)", "Blends & Consonant Digraphs") },
        { "PathwayPrimary13Item2", ("الكلمات البصرية الأساسية (Sight Words)", "Essential Sight Words (Dolch/Fry)") },
        { "PathwayPrimary13Item3", ("كلمات المدرسة والفصل الدراسي", "School & Classroom Vocabulary") },
        { "PathwayPrimary13Cta", ("عرض باقات الصفوف الأولى بالمتجر", "View Primary 1-3 Packs") },
        { "PathwayPrimary46Title", ("الصفوف العليا (Primary 4 - 6)", "Upper Primary (Grades 4 - 6)") },
        { "PathwayPrimary46Age", ("سن 9 إلى 12 سنة", "Ages 9 to 12") },
        { "PathwayPrimary46Item1", ("تكوين الجمل وعلامات الترقيم", "Sentence Structure & Punctuation") },
        { "PathwayPrimary46Item2", ("أزمنة الأفعال الأساسية", "Core Verb Tenses (Past/Present/Future)") },
        { "PathwayPrimary46Item3", ("كلمات متقدمة وتطبيقات Connect Plus", "Advanced Connect & Connect Plus Vocabulary") },
        { "PathwayPrimary46Cta", ("عرض باقات الصفوف العليا بالمتجر", "View Primary 4-6 Packs") },
        { "PathwayPrepTitle", ("المرحلة الإعدادية (Prep & Beyond)", "Preparatory & Beyond (Prep)") },
        { "PathwayPrepAge", ("سن 12 سنة وما فوق", "Ages 12 and above") },
        { "PathwayPrepItem1", ("أقسام الكلام (Parts of Speech)", "Parts of Speech (Nouns, Verbs, Adjectives)") },
        { "PathwayPrepItem2", ("الأفعال الشاذة (Irregular Verbs)", "Comprehensive Irregular Verbs") },
        { "PathwayPrepItem3", ("صياغة البراجراف والفقرات", "Paragraph Writing & Idioms") },
        { "PathwayPrepCta", ("عرض باقات الإعدادي بالمتجر", "View Prep Packs") },

        // Home Section 6 - Audience
        { "AudienceTitle", ("لمن صُممت بطاقات أكاديمية خلود النادي؟", "Who Are These Flashcards For?") },
        { "AudienceSubtitle", ("حلول مصممة بعناية لتخدم كل أطراف العملية التعليمية بنتائج سريعة وملموسة.", "Carefully crafted solutions designed to empower parents, teachers, and schools.") },
        { "AudienceParentsTitle", ("أولياء الأمور والأمهات", "Parents & Mothers") },
        { "AudienceParentsDesc", ("وداعاً لصعوبات المذاكرة ومعاناة الحفظ! تمنحك البطاقات وسيلة ممتعة لقضاء 15 دقيقة يومياً من اللعب والمراجعة مع طفلك دون أي ضغط أو إحباط، مع ضمان نطق سليم عبر الباركود الصوتي.", "Say goodbye to study battles! Spend 15 enjoyable minutes a day playing and reviewing with your child stress-free, with guaranteed correct pronunciation via QR audio.") },
        { "AudienceTeachersTitle", ("المعلمات ومراكز اللغات", "Teachers & Language Centers") },
        { "AudienceTeachersDesc", ("أداة صفية سحرية لإشعال الحماس والمنافسة بين الطلاب في الحصص والمجموعات. بطاقات مقواة تتحمل الاستخدام اليومي المستمر وتختصر وقت تحضير الوسائل التعليمية.", "A classroom powerhouse for engaging activities and lively competitions. Heavyweight cards built for daily student handling that save hours of lesson preparation.") },
        { "AudienceSchoolsTitle", ("المدارس والحضانات", "Schools & Nurseries") },
        { "AudienceSchoolsDesc", ("نوفر باقات وكميات خاصة لمدارس اللغات والحضانات مع عروض حصرية للطلبات المجمعة وتوافق تام مع الخطط الدراسية والمناهج المعتمدة من وزارة التربية والتعليم.", "Special bulk packages for language schools and nurseries, with exclusive institutional pricing and 100% curriculum alignment.") },

        // Home Section 7 - FAQs
        { "FaqEyebrow", ("FREQUENTLY ASKED QUESTIONS", "FREQUENTLY ASKED QUESTIONS") },
        { "FaqTitle", ("الأسئلة الأكثر شيوعاً", "Frequently Asked Questions") },
        { "FaqSubtitle", ("إجابات واضحة على كل ما يهمك معرفته حول بطاقات الأكاديمية والشحن والدفع", "Clear answers to everything you need to know about our cards, shipping, and payment") },
        { "FaqQ1", ("ما هي خامات البطاقات وهل هي قابلة للغسيل والمسح؟", "What materials are used, and are the cards washable?") },
        { "FaqA1", ("نعم بالتأكيد! جميع البطاقات مطبوعة على ورق كوشيه مقوى بوزن 350 جم ومغلفة بسلوفان حراري مزدوج (طبقة حماية بلاستيكية عازلة على الوجهين)، مما يجعلها مقاومة 100% للماء والعصائر وسهلة التنظيف بمنديل مبلل دون أن تتلف أو تنثني.", "Yes, absolutely! All cards are printed on heavy 350gsm cardstock with double-sided thermal lamination, making them 100% water- and juice-resistant and effortlessly cleanable with a damp cloth.") },
        { "FaqQ2", ("كيف استمع للنطق الصوتي الصحيح للبطاقات؟", "How do I listen to the correct audio pronunciation?") },
        { "FaqA2", ("كل باقة مزودة بباركود ذكي (QR Code) مطبوع على غلاف الباقة وخلفية البطاقات. بمجرد توجيه كاميرا الموبايل نحو الباركود، يفتح لك رابط مباشر يحتوي على ملفات صوتية واضحة بنطق متحدثين أصليين (Native Speakers) لتكرار النطق والتدريب عليه.", "Each pack features a smart QR code. Point your smartphone camera at the code to instantly open native speaker audio files for authentic listening and repeating practice.") },
        { "FaqQ3", ("ما هي طرق الدفع المتاحة داخل مصر؟", "What payment methods are available in Egypt?") },
        { "FaqA3", ("نوفر خيارات دفع مريحة وسريعة تشمل: التحويل عبر محافظ الهاتف المحمول (فودافون كاش، أورنج كاش، اتصالات كاش، وي باي)، التحويل الفوري عبر تطبيق إنستاباي (InstaPay)، وكذلك إمكانية تأكيد الطلب مباشرة عبر محادثة واتساب مع المعلمة خلود النادي.", "We offer fast and convenient payment methods: Mobile Wallets (Vodafone Cash, Orange Cash, Etisalat Cash, WE Pay), Instant bank transfer via InstaPay, or direct order confirmation via WhatsApp.") },
        { "FaqQ4", ("كم تستغرق مدة الشحن وكيف يتم التسليم؟", "How long does shipping take and how is delivery handled?") },
        { "FaqA4", ("يتم شحن وتوصيل الطلبات حتى باب منزلك في جميع محافظات مصر. يستغرق الشحن لمحافظات القاهرة والجيزة من 24 إلى 48 ساعة فقط، ومن 2 إلى 4 أيام عمل لباقي محافظات الوجه البحري والقبلي والمحافظات الحدودية.", "Doorstep delivery is available to all Egyptian governorates. Delivery to Cairo and Giza takes 24–48 hours, and 2–4 business days for Delta, Upper Egypt, and border governorates.") },
        { "FaqQ5", ("هل توجد أسعار خاصة للحضانات والمدارس والمعلمات؟", "Are there special discounts for schools, nurseries, and teachers?") },
        { "FaqA5", ("نعم بكل تأكيد! نقدم خصومات مميزة وعروض جملة خاصة للمدارس الخاصة والتجريبية، الحضانات، ومراكز التدريب واللغات. يمكنك التواصل معنا مباشرة عبر واتساب لتحديد الكميات وتجهيز الباقات المطلوبة.", "Yes, certainly! We offer attractive bulk pricing and wholesale discounts for private and language schools, nurseries, and educational centers. Contact us via WhatsApp for bulk quotes.") },

        // Home Section 8 - Final CTA
        { "FinalCtaEyebrow", ("ابدأ الآن وأحدث فارقاً حقيقياً في مستوى طفلك", "Start Today & Elevate Your Child's English") },
        { "FinalCtaTitle", ("استثمر في تأسيس طفلك مع بطاقات الأكاديمية الأصلية", "Invest in Your Child's Future with Official Academy Flashcards") },
        { "FinalCtaSubtitle", ("شحن سريع لباب منزلك، دعم مباشر من المعلمة، وخامات فاخرة تعيش مع طفلك وتصنع فارقاً حقيقياً في إتقانه للغة الإنجليزية.", "Fast doorstep delivery, direct teacher guidance, and premium durable cards that create lasting mastery.") },
        { "FinalCtaWhatsApp", ("تواصل واتساب مع المعلمة", "WhatsApp the Teacher Directly") },

        // Product Details Extra
        { "DetailsOverview", ("وصف المحتوى وأهداف التعلم", "Educational Overview & Learning Goals") },
        { "DetailsSpecsTitle", ("المواصفات الفنية وجودة الطباعة:", "Technical Specifications & Print Quality:") },
        { "DetailsOrderHeader", ("اختر طريقة الطلب المفضلة لديك:", "Choose Your Preferred Order Method:") },
        { "DetailsAddToCartAction", ("أضف للسلة ومتابعة الشراء", "Add to Cart & Checkout") },
        { "DetailsOutOfStockAction", ("غير متوفر في المخزن", "Currently Out of Stock") },
        { "DetailsWhatsAppDirect", ("طلب مباشر عبر واتساب", "Direct WhatsApp Order") },
        { "DetailsSafePaymentNote", ("دفع آمن عبر فودافون كاش، إنستاباي، أو التنسيق المباشر مع المعلمة للشحن", "Secure payment via Vodafone Cash, InstaPay, or direct WhatsApp coordination") },
        { "DetailsAvailableUnits", ("المتاح: {0} باقة", "Available: {0} packs") },
        { "DetailsLevelLabel", ("المستوى: {0}", "Level: {0}") },

        // Cart Page Extra
        { "CartPageTitle", ("سلة مشترياتك", "Your Shopping Cart") },
        { "CartEmptyTitle", ("سلة مشترياتك فارغة حالياً", "Your shopping cart is currently empty") },
        { "CartEmptySub", ("لم تقم بإضافة أي باقات فلاش كاردز حتى الآن. استكشف باقات الفونكس والقواعد والكلمات المصورة وأضف ما يناسبك.", "You haven't added any flashcard packs yet. Explore phonics, grammar, and vocabulary packs to get started.") },
        { "CartBrowsePacksNow", ("تصفح البطاقات التعليمية الآن", "Browse Educational Flashcards Now") },
        { "CartSelectedPacksHeader", ("البطاقات المحددة ({0} باقة)", "Selected Flashcards ({0} packs)") },
        { "CartPricesCurrencyNote", ("الأسعار بالجنيه المصري", "Prices in Egyptian Pounds (EGP)") },
        { "CartItemPriceLabel", ("سعر الباقة:", "Pack Price:") },
        { "CartTotalLabel", ("الإجمالي:", "Subtotal:") },
        { "CartDecreaseQty", ("تقليل الكمية", "Decrease quantity") },
        { "CartIncreaseQty", ("زيادة الكمية", "Increase quantity") },
        { "CartRemoveItem", ("حذف من السلة", "Remove from cart") },
        { "CartSummaryTitle", ("ملخص سلة الشراء", "Cart Summary") },
        { "CartPacksCountLabel", ("عدد الباكات:", "Packs Count:") },
        { "CartProductsValue", ("قيمة المنتجات:", "Products Total:") },
        { "CartEstimatedShipping", ("مصاريف الشحن التقديرية:", "Estimated Shipping:") },
        { "CartCheckoutNow", ("إتمام الطلب الآن", "Proceed to Checkout") },

        // Checkout Page Extra
        { "CheckoutTrustBanner", ("شحن سريع لجميع محافظات مصر • دفع آمن ومباشر", "Fast Shipping Across Egypt • Direct & Secure Payment") },
        { "CheckoutValidationHeader", ("يرجى تصحيح الأخطاء التالية لمتابعة الطلب:", "Please correct the following errors to continue:") },
        { "CustomerTypeSchool", ("مدرسة / حضانة", "School / Nursery") },
        { "CustomerTypeSchoolDesc", ("للأنشطة والكميات", "For bulk & activities") },
        { "EgyptianPhone", ("رقم الهاتف المصري *", "Egyptian Phone Number *") },
        { "WhatsAppNumberOptional", ("رقم الواتساب (للمتابعة والتأكيد)", "WhatsApp Number (For follow-up)") },
        { "WhatsAppNumberPlaceholder", ("إذا كان مختلفاً عن رقم الهاتف", "If different from phone number") },
        { "EmailOptional", ("البريد الإلكتروني (اختياري)", "Email Address (Optional)") },
        { "ShippingAddressSection", ("عنوان التوصيل في مصر", "Shipping Address in Egypt") },
        { "CityLabel", ("المدينة / الحي أو المركز *", "City / District *") },
        { "CityPlaceholder", ("مثال: مدينة نصر، المعادي، الدقي...", "e.g. New Cairo, Maadi, Dokki...") },
        { "DetailedAddressLabel", ("العنوان بالتفصيل (اسم الشارع، رقم العمارة، الشقة أو علامة مميزة) *", "Detailed Address (Street name, building, apartment, landmark) *") },
        { "DetailedAddressPlaceholder", ("أدخل العنوان المفصل لسهولة وسرعة وصول مندوب الشحن لبابك", "Enter full address for fast doorstep courier delivery") },
        { "NotesLabel", ("ملاحظات إضافية على الطلب (اختياري)", "Additional Delivery Notes (Optional)") },
        { "NotesPlaceholder", ("أي تعليمات للمندوب أو ميعاد تسليم مفضل", "Any courier instructions or preferred delivery time") },
        { "PaymentMethodSection", ("طريقة الدفع والتأكيد", "Payment & Confirmation Method") },
        { "WalletMethodTitle", ("محافظ إلكترونية / إنستاباي", "Mobile Wallets / InstaPay") },
        { "WalletMethodSub", ("فودافون كاش، أورنج موني، InstaPay", "Vodafone Cash, Orange Money, InstaPay") },
        { "WalletMethodBadge", ("حجز مؤقت للمخزون لمدة 24 ساعة", "Temporary 24h Stock Reservation") },
        { "WhatsAppMethodTitle", ("تأكيد مباشر عبر واتساب", "Direct WhatsApp Confirmation") },
        { "WhatsAppMethodSub", ("محادثة فورية مع المعلمة برقم الطلب", "Chat directly with Miss Kholoud with Order #") },
        { "WhatsAppMethodBadge", ("مناسب للمدارس والكميات", "Ideal for schools & bulk orders") },
        { "WalletStepsTitle", ("خطوات الدفع بالمحفظة الإلكترونية أو إنستاباي:", "Payment Steps via Wallet or InstaPay:") },
        { "WalletStepsDesc", ("بعد الضغط على \"تأكيد الطلب\"، ستظهر لك صفحة تحتوي على رقم فودافون كاش وعنوان إنستاباي، ونموذج سريع لرفع صورة الإيصال ليتم اعتماد شحن طلبك فوراً.", "After clicking \"Confirm Order\", you will see our Vodafone Cash number and InstaPay handle, with an instant receipt upload form to approve your shipment immediately.") },
        { "WhatsAppStepsTitle", ("خطوات التأكيد عبر واتساب:", "WhatsApp Confirmation Steps:") },
        { "WhatsAppStepsDesc", ("سيتم إنشاء رقم طلب لحفظ بياناتك، وتوجيهك مباشرة بضغطة زر إلى محادثة واتساب مع المعلمة خلود النادي برسالة جاهزة تحتوي على تفاصيل باقاتك وعنوان التوصيل.", "An order number will be generated to save your details, redirecting you directly to a WhatsApp chat with teacher Kholoud Elnady with prefilled order details.") },
        { "OrderCardsHeader", ("البطاقات في طلبك", "Items in Your Order") },
        { "SubtotalCardsLabel", ("مجموع البطاقات:", "Items Subtotal:") },
        { "ShippingGovernorateLabel", ("الشحن (حسب المحافظة):", "Shipping (by Governorate):") },
        { "TotalPayableLabel", ("المبلغ الإجمالي المطلوب:", "Total Payable Amount:") },
        { "ConfirmAndProceed", ("تأكيد الطلب والمتابعة", "Confirm Order & Continue") },
        { "TrustWaterproof", ("سلوفان حراري ضد الماء", "Waterproof Thermal Lamination") },
        { "TrustDoorstep", ("توصيل لباب منزلك", "Doorstep Delivery") },
        { "CheckoutDisclaimer", ("بالضغط على تأكيد الطلب، فإنك تؤكد رغبتك في شراء البطاقات المطبوعة المحددة من أكاديمية خلود النادي.", "By confirming this order, you agree to purchase the selected printed flashcard packs from Kholoud Elnady Academy.") },

        // Tracking Page Extra
        { "SearchingOrder", ("جارٍ البحث والتحقق من بيانات الطلب...", "Searching and verifying order details...") },
        { "OrderNotFound", ("لم يتم العثور على الطلب", "Order Not Found") },
        { "OrderRecordData", ("بيانات الطلب المسجل:", "Recorded Order Information:") },
        { "OrderDateLabel", ("تاريخ الطلب: {0}", "Order Date: {0}") },
        { "StepPlaced", ("تسجيل الطلب", "Order Placed") },
        { "StepReview", ("مراجعة التحويل", "Payment Review") },
        { "StepConfirmed", ("التأكيد والتجهيز", "Confirmed & Packed") },
        { "StepWithCourier", ("مع المندوب", "Out for Delivery") },
        { "StepDelivered", ("تم التسليم", "Delivered") },
        { "CurrentOrderStatus", ("الموقف الحالي للطلب:", "Current Order Status:") },
        { "NoProofNotice", ("لم يتم رفع إيصال التحويل بعد:", "Payment receipt not uploaded yet:") },
        { "NoProofDesc", ("يرجى رفع لقطة الشاشة للتحويل ليتم اعتماد طلبك قبل انتهاء مهلة الـ 24 ساعة.", "Please upload your transfer screenshot to approve your order before the 24-hour window expires.") },
        { "UploadProofNow", ("رفع الإيصال الآن", "Upload Receipt Now") },
        { "RecipientName", ("اسم المستلم:", "Recipient Name:") },
        { "OrderPacksCount", ("عدد الباكات:", "Packs Count:") },
        { "RequiredAmount", ("المبلغ المطلوب:", "Required Amount:") },

        // Order Confirmation Page
        { "OrderConfirmationTitle", ("تأكيد استلام الطلب - {0}", "Order Confirmation - {0}") },
        { "OrderConfirmedTitle", ("تم تسجيل طلبك بنجاح!", "Order Received Successfully!") },
        { "OrderConfirmedSub", ("شكراً لاختيارك أكاديمية خلود النادي لتعليم اللغة الإنجليزية", "Thank you for choosing Kholoud Elnady Academy") },
        { "OrderNumberSaved", ("تم حفظ بيانات الطلب في نظامنا برقم: {0}", "Your order has been recorded with reference number: {0}") },
        { "WhatsAppConfirmPrompt", ("تأكيد الميعاد مع الأكاديمية مباشرة عبر واتساب", "Direct WhatsApp Confirmation with Academy") },
        { "WhatsAppConfirmDesc", ("اضغط على الزر أدناه لفتح محادثة واتساب مجهزة مسبقاً بكافة تفاصيل طلبك وعنوانك للتنسيق الفوري مع ميس خلود النادي:", "Click the button below to open a prefilled WhatsApp chat with your order details to coordinate directly with Miss Kholoud:") },
        { "WhatsAppSendNow", ("إرسال تفاصيل الطلب لميس خلود على واتساب الآن", "Send Order Details to Miss Kholoud Now") },
        { "OrderDetailsHeader", ("تفاصيل الطلب", "Order Details") },
        { "CustomerNameLabel", ("اسم العميل:", "Customer Name:") },
        { "CustomerPhoneLabel", ("رقم الهاتف:", "Phone Number:") },
        { "DeliveryAddressLabel", ("عنوان التوصيل:", "Delivery Address:") },
        { "PaymentMethodLabel", ("طريقة الدفع:", "Payment Method:") },
        { "TrackThisOrder", ("تتبع حالة هذا الطلب", "Track This Order") },
        { "ReturnToCatalog", ("العودة لكتالوج البطاقات", "Return to Store & Catalog") },
        { "TableCardHeader", ("البطاقة", "Flashcard Pack") },
        { "TableQtyHeader", ("الكمية", "Quantity") },
        { "TablePriceHeader", ("السعر", "Price") },
        { "ShippingFeeLabel", ("مصاريف الشحن:", "Shipping Fee:") },
        { "GrandTotalLabel", ("الإجمالي النهائي:", "Grand Total:") },

        // Payment Instructions Page
        { "PaymentInstructionsTitle", ("تعليمات الدفع ورفع إيصال التحويل - طلب {0}", "Payment Instructions & Receipt Upload - Order {0}") },
        { "PaymentStepBanner", ("الخطوة الأخيرة لإتمام الطلب", "Final Step to Complete Your Order") },
        { "PaymentPendingTitle", ("تم تسجيل طلبك بنجاح! في انتظار تحويل المبلغ", "Order Recorded! Awaiting Payment Transfer") },
        { "YourOrderNumber", ("رقم طلبك: {0}", "Order Reference: {0}") },
        { "TotalWithShipping", ("المبلغ الإجمالي شامل الشحن:", "Total Amount (incl. shipping):") },
        { "Reservation24hTitle", ("تم حجز باقات الفلاش كاردز الخاصة بك مؤقتاً لمدة 24 ساعة", "Your Flashcard Packs Are Reserved for 24 Hours") },
        { "Reservation24hDesc", ("تم تأمين الكمية المطلوبة من المخزن لك لمدة 24 ساعة. يرجى تحويل المبلغ ورفع لقطة الشاشة في النموذج أدناه ليتم اعتماد طلبك للشحن فوراً.", "Your items are reserved for 24 hours. Please transfer the amount and upload your screenshot below to approve shipment immediately.") },
        { "EgyptPaymentMethods", ("طرق التحويل المتاحة في مصر", "Available Payment Methods in Egypt") },
        { "CopyNumber", ("نسخ الرقم", "Copy Number") },
        { "CopyAccount", ("نسخ الحساب", "Copy Account") },
        { "WalletTransferHint", ("يمكنك التحويل من أي محفظة كاش إلكترونية تابعة لأي شبكة محمول في جمهورية مصر العربية", "You can transfer from any mobile cash wallet across Egypt") },
        { "InstaPayReceiverHint", ("اسم المستلم: Kholoud Elnady English Academy (أكاديمية خلود النادي)", "Recipient Name: Kholoud Elnady English Academy") },
        { "PaymentNoteTitle", ("تنويه هام عند التحويل:", "Important Transfer Note:") },
        { "PaymentNoteDesc", ("يرجى كتابة رقم الطلب {0} في خانة الملاحظات داخل تطبيق المحفظة أو إنستاباي لتسريع التحقق التلقائي.", "Please mention Order #{0} in transfer notes/reference for instant verification.") },
        { "UploadReceiptHeader", ("رفع لقطة الشاشة أو إيصال التحويل", "Upload Transfer Receipt or Screenshot") },
        { "ReceiptFileLabel", ("صورة الإيصال / لقطة الشاشة *", "Receipt / Screenshot Image *") },
        { "SupportedFilesHint", ("الملفات المدعومة: JPG, PNG, WEBP, PDF (بحد أقصى 10 ميجابايت)", "Supported files: JPG, PNG, WEBP, PDF (max 10MB)") },
        { "ReceiptPreviewTitle", ("معاينة الصورة المحددة:", "Selected Image Preview:") },
        { "PaymentMethodUsed", ("وسيلة الدفع المستخدمة", "Payment Method Used") },
        { "SenderPhoneLabel", ("رقم المحفظة التي قمت بالتحويل منها (للتأكد)", "Sender Phone / Wallet Number (for verification)") },
        { "TrxRefLabel", ("رقم العملية المرجعي / كود التحويل (اختياري)", "Transaction Reference / ID (Optional)") },
        { "SubmitReceipt", ("إرسال الإيصال وتأكيد حجز الطلب", "Submit Receipt & Confirm Order") },
        { "OrSendViaWhatsApp", ("أو إرسال صورة الإيصال مباشرة عبر واتساب الأكاديمية", "Or send receipt directly via Academy WhatsApp") },

        // Wishlist
        { "NavWishlist", ("المفضلة", "Wishlist") },
        { "AddToWishlist", ("حفظ في المفضلة", "Save to Wishlist") },
        { "RemoveFromWishlist", ("إزالة من المفضلة", "Remove from Wishlist") },
        { "InWishlist", ("محفوظ بالمفضلة", "Saved in Wishlist") },
        { "WishlistTitle", ("قائمة البطاقات المفضلة", "Saved Flashcards Wishlist") },
        { "WishlistSubtitle", ("الكروت التي قمت بحفظها للرجوع إليها وشرائها لاحقاً", "Flashcard packs you saved to review or purchase later") },
        { "WishlistEmptyTitle", ("قائمة المفضلة فارغة حالياً", "Your Wishlist is Empty") },
        { "WishlistEmptyDesc", ("لم تقم بحفظ أي باقات بعد. استكشف مجموعات الفلاش كاردز واحفظ ما يناسب طفلك أو طلابك بالضغط على علامة القلب.", "You haven't saved any packs yet. Explore our flashcards collection and save packs by tapping the heart icon.") },
        { "WishlistMoveToCart", ("نقل إلى السلة", "Move to Cart") },
        { "WishlistClearAll", ("مسح كل المفضلة", "Clear Wishlist") },
        { "WishlistTotalItems", ("عدد الباقات المحفوظة: {0}", "{0} saved packs") },
        { "WishlistAddedToast", ("تمت إضافة الباقة للمفضلة بنجاح", "Pack added to wishlist") },
        { "WishlistRemovedToast", ("تمت إزالة الباقة من المفضلة", "Pack removed from wishlist") },

        // Reviews & Ratings
        { "ReviewsSectionTitle", ("آراء وتجارب أولياء الأمور والمعلمين", "Parent & Teacher Reviews & Ratings") },
        { "ReviewsSectionSubtitle", ("تجارب حقيقية موثقة من أمهات ومعلمات استخدمن كروت أكاديمية خلود النادي", "Verified testimonials from parents and educators who used our flashcards") },
        { "ReviewsRatingOutOf", ("من 5 نجوم", "out of 5 stars") },
        { "ReviewsTotalCount", ("بناءً على {0} تقييم موثق", "Based on {0} verified reviews") },
        { "WriteReviewButton", ("أضف تقييمك وتجربتك", "Write a Review") },
        { "ReviewFormTitle", ("شاركنا تجربتك مع هذه الباقة", "Share Your Experience with this Pack") },
        { "ReviewRatingLabel", ("تقييمك بالنجوم *", "Your Rating *") },
        { "ReviewNameLabel", ("الاسم بالكامل *", "Full Name *") },
        { "ReviewNamePlaceholder", ("مثال: أ. منى السيد (ولية أمر)", "e.g. Mrs. Mona El-Sayed (Parent)") },
        { "ReviewRoleLabel", ("صفتك التعليمية *", "Your Role *") },
        { "ReviewRoleParent", ("ولي أمر (أم / أب)", "Parent (Mother / Father)") },
        { "ReviewRoleTeacher", ("معلمة لغة إنجليزية", "English Teacher") },
        { "ReviewRoleSchool", ("إدارة حضانة / مدرسة", "School / Nursery") },
        { "ReviewRoleSpecialist", ("أخصائي تخاطب وتنمية مهارات", "Speech & Skills Specialist") },
        { "ReviewCommentLabel", ("رأيك وتجربتك بالتفصيل *", "Your Detailed Review *") },
        { "ReviewCommentPlaceholder", ("كيف ساعدت هذه الكروت طفلك أو طلابك في تعلم النطق أو القراءة؟ ما رأيك في خامة الورق وسرعة الاستفادة؟", "How did these cards help your child or students? What do you think of durability and progress?") },
        { "ReviewSubmitButton", ("إرسال التقييم الآن", "Submit Review Now") },
        { "ReviewSuccessMessage", ("شكراً لك! تم تسجيل تقييمك بنجاح وسعداء جداً برأيك.", "Thank you! Your review has been submitted successfully.") },
        { "VerifiedBuyerBadge", ("مشتري موثق", "Verified Purchase") },
        { "ReviewsNoReviewsYet", ("لا توجد تقييمات مسجلة لهذه الباقة بعد. كن أول من يشاركنا رأيه!", "No reviews for this pack yet. Be the first to share your feedback!") }
    };

    public bool IsRtl
    {
        get
        {
            var culture = CultureInfo.CurrentUICulture;
            return culture.TextInfo.IsRightToLeft;
        }
    }

    public string CurrentCultureName => CultureInfo.CurrentUICulture.Name;

    public string CurrentLanguageCode => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

    public string Get(string key)
    {
        if (Translations.TryGetValue(key, out var entry))
        {
            return IsRtl ? entry.Ar : entry.En;
        }

        return key;
    }

    public string Get(string key, params object[] args)
    {
        var template = Get(key);
        try
        {
            return string.Format(template, args);
        }
        catch
        {
            return template;
        }
    }

    public string GetLocalized(string arText, string enText)
    {
        if (!IsRtl && !string.IsNullOrWhiteSpace(enText))
        {
            return enText;
        }

        return string.IsNullOrWhiteSpace(arText) ? enText : arText;
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
}
