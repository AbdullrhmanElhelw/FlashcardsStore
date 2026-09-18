using System.Text.Encodings.Web;
using System.Text.Unicode;
using FlashcardsStore.Application;
using FlashcardsStore.Infrastructure;
using FlashcardsStore.Infrastructure.Persistence;
using FlashcardsStore.Infrastructure.Persistence.Seed;
using FlashcardsStore.Web.Common;

var builder = WebApplication.CreateBuilder(args);

// Configure UTF-8 HtmlEncoder so Arabic characters are not entity-encoded
builder.Services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(UnicodeRanges.All));

// Add MVC Controllers with Views
builder.Services.AddControllersWithViews();

// Add HttpContextAccessor and Session Support
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".FlashcardsStore.Session";
});

// Register Cart Session Manager
builder.Services.AddScoped<CartSessionManager>();

// Register Application Services (DTOs, Validation, Business Logic)
builder.Services.AddApplication();

// Register Infrastructure Services (EF Core SQL Server, Repositories, File Storage, 24h Background Cleanup)
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment.WebRootPath);

var app = builder.Build();

// Seed Database on Startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<FlashcardsStoreDbContext>();
        await DbInitializer.InitializeAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

// Configure HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Catalog/Index");
    app.UseHsts();
}

// Configure Request Localization
var supportedCultures = new[]
{
    new System.Globalization.CultureInfo("ar-EG"),
    new System.Globalization.CultureInfo("ar"),
    new System.Globalization.CultureInfo("en-US"),
    new System.Globalization.CultureInfo("en")
};

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("ar-EG"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

// Apply Request Localization
app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
