using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using FlashcardsStore.Domain.Entities;

namespace FlashcardsStore.Infrastructure.Persistence.Seed;

public static class DbInitializer
{
    public static async Task InitializeAsync(FlashcardsStoreDbContext context)
    {
        // Ensure created if migrations not yet applied
        await context.Database.EnsureCreatedAsync();

        // Seed Categories
        if (!await context.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new()
                {
                    Name = "Phonics",
                    NameAr = "فونكس وصوتيات الحروف",
                    Slug = "phonics",
                    Description = "بطاقات تعليم مخارج الحروف وأصوات العلة والحروف المركبة للأطفال والمبتدئين",
                    DisplayOrder = 1
                },
                new()
                {
                    Name = "Grammar",
                    NameAr = "قواعد اللغة الإنجليزية",
                    Slug = "grammar",
                    Description = "بطاقات تبسيط قواعد الإنجليزية وأزمنة الأفعال وأقسام الكلام بأمثلة ملونة وشيقة",
                    DisplayOrder = 2
                },
                new()
                {
                    Name = "Vocabulary",
                    NameAr = "مفردات وكلمات مصورة",
                    Slug = "vocabulary",
                    Description = "بطاقات كلمات مصورة عالية الجودة لتنمية الحصيلة اللغوية للمراحل التأسيسية",
                    DisplayOrder = 3
                },
                new()
                {
                    Name = "Sight Words",
                    NameAr = "الكلمات البصرية والقراءة",
                    Slug = "sight-words",
                    Description = "بطاقات الكلمات البصرية المتكررة لتسريع طلاقة القراءة الذاتية للأطفال",
                    DisplayOrder = 4
                }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        // Seed Products
        if (!await context.Products.AnyAsync())
        {
            var phonics = await context.Categories.FirstAsync(c => c.Slug == "phonics");
            var grammar = await context.Categories.FirstAsync(c => c.Slug == "grammar");
            var vocab = await context.Categories.FirstAsync(c => c.Slug == "vocabulary");
            var sightWords = await context.Categories.FirstAsync(c => c.Slug == "sight-words");

            var products = new List<Product>
            {
                new()
                {
                    Name = "Short Vowels Sound Cards Pack",
                    NameAr = "مجموعة بطاقات الحركات القصيرة (Short Vowels)",
                    Slug = "short-vowels-pack",
                    Description = "A complete set of 52 laminated high-durability flashcards covering all 5 short vowels (a, e, i, o, u) with colorful illustrations, word families, and phonetic pronunciations.",
                    DescriptionAr = "مجموعة مكونة من 52 كارت مطبوع بجودة كوشيه مقوى ومغلف سلوفان لحماية طويلة الأمد. تشمل جميع الحركات القصيرة مع أمثلة بالصور وتطبيق فوري على الكلمات.",
                    TargetAudience = "KG1 - Primary 2 (الأطفال من سن 4 إلى 7 سنوات)",
                    GradeLevel = "KG & Early Primary",
                    CardCount = 52,
                    Price = 185.00m,
                    StockQuantity = 40,
                    ReservedStockQuantity = 0,
                    ImageUrl = "/images/cards/card-phonics-1.svg",
                    BackPreviewText = "Sample Card: Short 'a' - /æ/ as in Cat, Bat, Hat. Includes word family patterns: -at, -an, -ap.",
                    BackKeyPoints = "52 بطاقة وجهين ملونة, ورق كوشيه فاخر 350 جرام, زوايا دائرية آمنة للأطفال, باركود للاستماع للنطق الصحيح",
                    CategoryId = phonics.Id,
                    IsActive = true,
                    IsFeatured = true
                },
                new()
                {
                    Name = "Blends & Digraphs Master Pack",
                    NameAr = "صندوق الحروف المركبة والدمج (Blends & Digraphs)",
                    Slug = "blends-and-digraphs-pack",
                    Description = "80 durable educational cards for mastering beginning and ending blends (bl, cl, st, tr) and digraphs (sh, ch, th, ph, wh).",
                    DescriptionAr = "80 كارت تعليمي متدرج الصعوبة للتدريب على دمج حرفين أو أكثر ونطق الحروف المركبة بدقة واحترافية.",
                    TargetAudience = "Primary 1 - Primary 4 (سن 6 إلى 10 سنوات)",
                    GradeLevel = "Primary 1 - 4",
                    CardCount = 80,
                    Price = 240.00m,
                    StockQuantity = 35,
                    ReservedStockQuantity = 0,
                    ImageUrl = "/images/cards/card-phonics-2.svg",
                    BackPreviewText = "Sample Card: Digraph 'sh' - /ʃ/ as in Ship, Fish, Shell. Includes rhyming sentence practice.",
                    BackKeyPoints = "80 بطاقة مقسمة إلى مجموعتين بألوان مميزة, تمارين قراءة جمل سريعة, مغلفة ضد الماء وسهلة التنظيف",
                    CategoryId = phonics.Id,
                    IsActive = true,
                    IsFeatured = true
                },
                new()
                {
                    Name = "CVC Word Builder Cards",
                    NameAr = "بطاقات تكوين وقراءة الكلمات الثلاثية (CVC Words)",
                    Slug = "cvc-word-builder",
                    Description = "64 hands-on cards designed for segmenting and blending Consonant-Vowel-Consonant words with sound buttons.",
                    DescriptionAr = "64 بطاقة تفاعلية تحتوي على أزرار الصوتيات (Sound Buttons) لتدريب الطفل على تقطيع الكلمة ودمجها للقراءة الأولى.",
                    TargetAudience = "KG2 - Primary 1",
                    GradeLevel = "KG2 - Primary 1",
                    CardCount = 64,
                    Price = 210.00m,
                    StockQuantity = 50,
                    ReservedStockQuantity = 0,
                    ImageUrl = "/images/cards/card-phonics-3.svg",
                    BackPreviewText = "Sample Card: /b/ - /e/ - /d/ = Bed. With touch sound-dots under each letter.",
                    BackKeyPoints = "نقاط صوتية لمسية أسفل الحروف, مثالية لحصص التأسيس والمراكز التعليمية, تأتي في علبة هدايا كرتونية مقواة",
                    CategoryId = phonics.Id,
                    IsActive = true,
                    IsFeatured = false
                },
                new()
                {
                    Name = "Parts of Speech Flashcards",
                    NameAr = "بطاقات أقسام الكلام (Parts of Speech)",
                    Slug = "parts-of-speech-pack",
                    Description = "60 visual flashcards categorizing Nouns, Pronouns, Verbs, Adjectives, Adverbs, and Prepositions with real sentence examples.",
                    DescriptionAr = "60 بطاقة ملونة تساعد الطالب على تمييز الأسماء والضمائر والأفعال والصفات وحروف الجر في الجملة بسهولة.",
                    TargetAudience = "Primary 2 - Prep 1 (سن 7 إلى 13 سنة)",
                    GradeLevel = "Primary & Prep",
                    CardCount = 60,
                    Price = 195.00m,
                    StockQuantity = 30,
                    ReservedStockQuantity = 0,
                    ImageUrl = "/images/cards/card-grammar-1.svg",
                    BackPreviewText = "Sample Card: Adjective - Describes a noun. 'The happy puppy'. Includes fill-in-the-blank mini quiz.",
                    BackKeyPoints = "كود لوني لكل قسم من أقسام الكلام, جمل تطبيقية من مناهج Connect و Connect Plus, أسئلة سريعة خلف الكارت",
                    CategoryId = grammar.Id,
                    IsActive = true,
                    IsFeatured = true
                },
                new()
                {
                    Name = "English Verb Tenses & Irregular Verbs",
                    NameAr = "أزمنة الأفعال وتصريفات الأفعال الشاذة",
                    Slug = "verb-tenses-pack",
                    Description = "75 comprehensive cards showing Past, Present, Future and the top 50 irregular verb forms with context sentences.",
                    DescriptionAr = "75 كارت شامل يعرض المضارع والماضي والتصريف الثالث لأهم 50 فعل غير منتظم مع أمثلة عملية توضيحية.",
                    TargetAudience = "Primary 3 - Prep 3",
                    GradeLevel = "Primary 3 - Prep 3",
                    CardCount = 75,
                    Price = 230.00m,
                    StockQuantity = 25,
                    ReservedStockQuantity = 0,
                    ImageUrl = "/images/cards/card-grammar-2.svg",
                    BackPreviewText = "Sample Card: Go - Went - Gone. Example: She went to the library yesterday.",
                    BackKeyPoints = "جدول التصريفات الثلاثة بوضوح, تدرج زمني ملون, تدريبات لتجنب الأخطاء الشائعة في الامتحانات",
                    CategoryId = grammar.Id,
                    IsActive = true,
                    IsFeatured = false
                },
                new()
                {
                    Name = "Sentence Building & Punctuation",
                    NameAr = "تكوين الجمل وعلامات الترقيم",
                    Slug = "sentence-building-cards",
                    Description = "50 interactive prompt cards teaching subject-verb agreement, punctuation marks, and paragraph starters.",
                    DescriptionAr = "50 كارت تفاعلي لتدريب الطالب على بناء جملة إنجليزية سليمة وتطبيق علامات الترقيم (Capital letter, Full stop, Question mark).",
                    TargetAudience = "Primary 2 - Primary 6",
                    GradeLevel = "Primary 2 - 6",
                    CardCount = 50,
                    Price = 175.00m,
                    StockQuantity = 45,
                    ReservedStockQuantity = 0,
                    ImageUrl = "/images/cards/card-grammar-3.svg",
                    BackPreviewText = "Sample Card: Rule: Start with a capital letter and end with a period. Fix this sentence: 'the cat sleeps'",
                    BackKeyPoints = "بطاقات تصحيح أخطاء تفاعلية, مناسبة للمسابقات الجماعية داخل الفصل, ترقيم ملون وسهل القراءة",
                    CategoryId = grammar.Id,
                    IsActive = true,
                    IsFeatured = false
                },
                new()
                {
                    Name = "500 First English Words for Kids",
                    NameAr = "أول 500 كلمة إنجليزية مصورة للأطفال",
                    Slug = "500-words-kids-pack",
                    Description = "100 double-sided cards featuring 500 essential vocabulary words divided into 20 daily life topics (Family, Food, Animals, Colors).",
                    DescriptionAr = "100 كارت كبير بوجهين يشمل 500 كلمة أساسية مقسمة إلى 20 موضوع من الحياة اليومية بالصور الكرتونية الجذابة.",
                    TargetAudience = "All Ages / Toddlers to Primary 3",
                    GradeLevel = "Pre-K to Primary 3",
                    CardCount = 100,
                    Price = 295.00m,
                    StockQuantity = 60,
                    ReservedStockQuantity = 0,
                    ImageUrl = "/images/cards/card-vocab-1.svg",
                    BackPreviewText = "Topic: Farm Animals - Cow, Horse, Sheep, Duck, Rooster with phonetic spelling.",
                    BackKeyPoints = "مقاس جامبو كبير وواضح, رسومات حصرية بألوان مبهجة, تصنيف بالموضوعات لسهولة الحفظ, ترجمة عربية دقيقة",
                    CategoryId = vocab.Id,
                    IsActive = true,
                    IsFeatured = true
                },
                new()
                {
                    Name = "School & Classroom Vocabulary",
                    NameAr = "كلمات المدرسة والفصل الدراسي المصورة",
                    Slug = "school-vocabulary-pack",
                    Description = "48 cards packed with classroom objects, teacher instructions, school subjects, and friendship phrases.",
                    DescriptionAr = "48 كارت يعلم الطفل كل ما يحيط به في المدرسة والأدوات المدرسية والتعليمات الصفية المعتادة.",
                    TargetAudience = "KG1 - Primary 3",
                    GradeLevel = "KG & Primary",
                    CardCount = 48,
                    Price = 160.00m,
                    StockQuantity = 30,
                    ReservedStockQuantity = 0,
                    ImageUrl = "/images/cards/card-vocab-2.svg",
                    BackPreviewText = "Sample Card: 'Ruler' / 'مسطرة' - Used to draw straight lines. Includes sentence: 'May I borrow your ruler?'",
                    BackKeyPoints = "جمل محادثة صفية جاهزة للاستخدام, حجم مناسب لجيب الطفل والمقلمة, سلوفان لامع لحماية من السوائل",
                    CategoryId = vocab.Id,
                    IsActive = true,
                    IsFeatured = false
                },
                new()
                {
                    Name = "Dolch Sight Words (Pre-Primer to Grade 3)",
                    NameAr = "الكلمات البصرية دولش (Dolch Sight Words)",
                    Slug = "dolch-sight-words-box",
                    Description = "92 cards covering the highest frequency Dolch list words that children must recognize instantly without sounding out.",
                    DescriptionAr = "92 كارت للكلمات البصرية الأكثر شيوعاً في قصص الأطفال والتي يجب على الطفل التعرف عليها بالعين مباشرة لزيادة سرعة القراءة.",
                    TargetAudience = "KG2 - Primary 3",
                    GradeLevel = "KG2 - Primary 3",
                    CardCount = 92,
                    Price = 260.00m,
                    StockQuantity = 40,
                    ReservedStockQuantity = 0,
                    ImageUrl = "/images/cards/card-sight-1.svg",
                    BackPreviewText = "Sample Card: 'the', 'said', 'where', 'come' - Includes sample reading sentence on the back.",
                    BackKeyPoints = "مقسمة بمستويات ألوان دولش المعتمدة عالمياً, جمل تطبيقية قصيرة على كل كلمة, حلقات معدنية لجمع الكروت معاً",
                    CategoryId = sightWords.Id,
                    IsActive = true,
                    IsFeatured = true
                },
                new()
                {
                    Name = "Fry 100 High-Frequency Reading Words",
                    NameAr = "أول 100 كلمة قراءة سريعة (Fry 100 Words)",
                    Slug = "fry-100-words-pack",
                    Description = "100 reading fluency cards representing 50% of all words found in elementary textbooks, with rapid recognition drills.",
                    DescriptionAr = "100 بطاقة قراءة سريعة تمثل أكثر من 50% من الكلمات الموجودة في كتب القراءة الإنجليزية للمراحل الابتدائية.",
                    TargetAudience = "Primary 1 - Primary 4",
                    GradeLevel = "Primary 1 - 4",
                    CardCount = 100,
                    Price = 275.00m,
                    StockQuantity = 35,
                    ReservedStockQuantity = 0,
                    ImageUrl = "/images/cards/card-sight-2.svg",
                    BackPreviewText = "Sample Card: 'there', 'their', 'they're' - Master the difference in meaning and usage easily.",
                    BackKeyPoints = "تمارين سرعة القراءة بالدقيقة (Speed Drills), كروت خفيفة ومتينة, تدريب بصري ممتاز لطلاب اللغات والتجريبي",
                    CategoryId = sightWords.Id,
                    IsActive = true,
                    IsFeatured = false
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }

        // Seed Admin User
        if (!await context.AdminUsers.AnyAsync())
        {
            var admin = new AdminUser
            {
                Username = "admin",
                PasswordHash = HashPassword("Admin@12345"),
                FullName = "Sara El-Sayed (English Teacher)",
                Email = "admin@flashcards-store.eg",
                Role = "Administrator",
                IsActive = true
            };

            await context.AdminUsers.AddAsync(admin);
            await context.SaveChangesAsync();
        }

        // Ensure Reviews Table Exists
        try
        {
            await context.Database.ExecuteSqlRawAsync(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Reviews')
                BEGIN
                    CREATE TABLE [Reviews] (
                        [Id] int NOT NULL IDENTITY,
                        [ProductId] int NOT NULL,
                        [CustomerName] nvarchar(150) NOT NULL,
                        [Role] nvarchar(100) NOT NULL,
                        [Rating] int NOT NULL,
                        [Comment] nvarchar(1000) NOT NULL,
                        [IsApproved] bit NOT NULL,
                        [IsVerifiedBuyer] bit NOT NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        [CreatedBy] nvarchar(max) NULL,
                        [UpdatedAt] datetime2 NULL,
                        [UpdatedBy] nvarchar(max) NULL,
                        [IsDeleted] bit NOT NULL,
                        [DeletedAt] datetime2 NULL,
                        CONSTRAINT [PK_Reviews] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Reviews_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Product] ([Id]) ON DELETE CASCADE
                    );
                END
            ");
        }
        catch
        {
            // Table may already exist or running in non-SQL Server environment
        }

        // Ensure Product.AudioUrl column exists for custom teacher voice notes
        try
        {
            await context.Database.ExecuteSqlRawAsync(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Product') AND name = 'AudioUrl')
                BEGIN
                    ALTER TABLE [Product] ADD [AudioUrl] nvarchar(500) NULL;
                END
            ");
        }
        catch
        {
            // Column may already exist
        }

        // Seed Reviews
        if (await context.Products.AnyAsync() && !await context.Reviews.AnyAsync())
        {
            var firstProduct = await context.Products.FirstOrDefaultAsync(p => p.Slug == "short-vowels-pack");
            var secondProduct = await context.Products.FirstOrDefaultAsync(p => p.Slug == "blends-and-digraphs-pack");
            var thirdProduct = await context.Products.FirstOrDefaultAsync(p => p.Slug == "cvc-word-builder");

            var reviews = new List<Review>();

            if (firstProduct != null)
            {
                reviews.AddRange(new[]
                {
                    new Review
                    {
                        ProductId = firstProduct.Id,
                        CustomerName = "أ. مروة سالم",
                        Role = "أم لطفلين (KG2 & Primary 1)",
                        Rating = 5,
                        Comment = "الكروت ممتازة وخامتها قوية جداً وسلوفان حراري ضد القطع، ابني حبها وبدأ ينطق أصوات الحركات القصيرة لوحده وبسرعة.",
                        IsApproved = true,
                        IsVerifiedBuyer = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-12)
                    },
                    new Review
                    {
                        ProductId = firstProduct.Id,
                        CustomerName = "Miss Yasmin Taha",
                        Role = "English Teacher - KG Stage",
                        Rating = 5,
                        Comment = "Used these cards in my phonics classes. The illustrations are vivid and the font size is ideal for small group learning.",
                        IsApproved = true,
                        IsVerifiedBuyer = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-8)
                    },
                    new Review
                    {
                        ProductId = firstProduct.Id,
                        CustomerName = "أ. دينا خليل",
                        Role = "معلمة تأسيس لغات",
                        Rating = 5,
                        Comment = "باركود النطق الصوتي في ظهر الكارت فكرة عبقرية، ساعدت أولياء الأمور يتابعوا مع أطفالهم النطق الصح في البيت.",
                        IsApproved = true,
                        IsVerifiedBuyer = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-3)
                    }
                });
            }

            if (secondProduct != null)
            {
                reviews.AddRange(new[]
                {
                    new Review
                    {
                        ProductId = secondProduct.Id,
                        CustomerName = "أ. إيمان عبد الرحمن",
                        Role = "ولية أمر (Primary 2)",
                        Rating = 5,
                        Comment = "بنتي كانت بتتلخبط بين sh و ch و th، الصندوق ده فرق معاها جداً وبقت تدمج وتقرأ الجمل السريعة بكل ثقة.",
                        IsApproved = true,
                        IsVerifiedBuyer = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-10)
                    },
                    new Review
                    {
                        ProductId = secondProduct.Id,
                        CustomerName = "Mr. Ahmed Mansour",
                        Role = "Primary English Coordinator",
                        Rating = 5,
                        Comment = "Outstanding cardstock, safe rounded corners, and brilliant color grouping. One of the best resources we acquired.",
                        IsApproved = true,
                        IsVerifiedBuyer = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-5)
                    }
                });
            }

            if (thirdProduct != null)
            {
                reviews.Add(new Review
                {
                    ProductId = thirdProduct.Id,
                    CustomerName = "أ. فاطمة النجار",
                    Role = "أخصائية تنمية مهارات وتخاطب",
                    Rating = 5,
                    Comment = "أزرار الصوتيات التفاعلية في الكارت فكرة ممتازة بتساعد الطفل على تجزئة الكلمة ودمجها للقراءة بسلاسة.",
                    IsApproved = true,
                    IsVerifiedBuyer = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-4)
                });
            }

            if (reviews.Count > 0)
            {
                await context.Reviews.AddRangeAsync(reviews);
                await context.SaveChangesAsync();
            }
        }
    }

    private static readonly FlashcardsStore.Infrastructure.Security.PasswordHasher _hasher = new();

    public static string HashPassword(string password) => _hasher.HashPassword(password);

    public static bool VerifyPassword(string enteredPassword, string storedHash) => _hasher.VerifyPassword(enteredPassword, storedHash);
}
