using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Authentication.Cookies;
using FlashcardsStore.Application;
using FlashcardsStore.Infrastructure;
using FlashcardsStore.Infrastructure.Persistence;
using FlashcardsStore.Infrastructure.Persistence.Seed;
using FlashcardsStore.Web.Common;

var builder = WebApplication.CreateBuilder(args);

// Configure UTF-8 HtmlEncoder so Arabic characters are not entity-encoded
builder.Services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(UnicodeRanges.All));

// Add MVC Controllers with Views
builder.Services.AddControllersWithViews()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

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

// Configure Enterprise Cookie Authentication for Admin Area
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.LogoutPath = "/Admin/Logout";
        options.AccessDeniedPath = "/Admin/Login";
        options.Cookie.Name = ".FlashcardsStore.AdminAuth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// Configure Enterprise Antiforgery Protection (Header support for HTMX & AJAX)
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
    options.Cookie.Name = ".FlashcardsStore.Antiforgery";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
