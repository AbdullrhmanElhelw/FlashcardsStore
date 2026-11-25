using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FlashcardsStore.Application.DTOs.Admin;
using FlashcardsStore.Application.DTOs.Categories;
using FlashcardsStore.Application.DTOs.Products;
using FlashcardsStore.Application.DTOs.Settings;
using FlashcardsStore.Application.Interfaces.Persistence;
using FlashcardsStore.Application.Interfaces.Services;
using FlashcardsStore.Domain.Enums;
using FlashcardsStore.Infrastructure.Persistence.Seed;

namespace FlashcardsStore.Web.Controllers;

[Authorize(Roles = "Administrator")]
public class AdminController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IPaymentProofService _paymentProofService;
    private readonly IAdminUserRepository _adminUserRepository;
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IReviewService _reviewService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IStoreSettingsService _storeSettingsService;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdminController(
        IOrderService orderService,
        IPaymentProofService paymentProofService,
        IAdminUserRepository adminUserRepository,
        IProductService productService,
        ICategoryService categoryService,
        IFileStorageService fileStorageService,
        IReviewService reviewService,
        IPasswordHasher passwordHasher,
        IStoreSettingsService storeSettingsService,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _orderService = orderService;
        _paymentProofService = paymentProofService;
        _adminUserRepository = adminUserRepository;
        _productService = productService;
        _categoryService = categoryService;
        _fileStorageService = fileStorageService;
        _reviewService = reviewService;
        _passwordHasher = passwordHasher;
        _storeSettingsService = storeSettingsService;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    private async Task<string> GenerateUniqueSlugAsync(string? name, string? nameAr, int? currentProductId = null, CancellationToken cancellationToken = default)
    {
        var raw = !string.IsNullOrWhiteSpace(name) ? name : nameAr ?? "pack";
        var clean = System.Text.RegularExpressions.Regex.Replace(raw, @"[^a-zA-Z0-9\s-]", "").Trim();
        clean = System.Text.RegularExpressions.Regex.Replace(clean, @"\s+", "-").ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(clean))
        {
            clean = "pack";
        }

        var candidate = clean;
        int counter = 1;
        while (true)
        {
            var existing = await _productService.GetProductDetailBySlugAsync(candidate, cancellationToken);
            if (existing == null || (currentProductId.HasValue && existing.Id == currentProductId.Value))
            {
                return candidate;
            }
            candidate = $"{clean}-{counter}";
            counter++;
        }
    }

    private string CurrentAdminUser => User.Identity?.Name ?? "admin";

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Dashboard");
        }

        ViewBag.ReturnUrl = returnUrl;
        return View(new AdminLoginDto());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginSubmit(AdminLoginDto dto, string? returnUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
        {
            ModelState.AddModelError(string.Empty, "يرجى إدخال اسم المستخدم وكلمة المرور");
            return View("Login", dto);
        }

        var user = await _adminUserRepository.GetByUsernameAsync(dto.Username.Trim(), cancellationToken);
        if (user == null || !_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "بيانات الدخول غير صحيحة");
            return View("Login", dto);
        }

        // Auto-upgrade legacy password hash to salted PBKDF2 if needed
        if (!user.PasswordHash.StartsWith("$pbkdf2-sha256$"))
        {
            user.PasswordHash = _passwordHasher.HashPassword(dto.Password);
        }
        user.LastLoginAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.GivenName, user.FullName ?? user.Username),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Role, user.Role)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Index() => RedirectToAction(nameof(Dashboard));

    [HttpGet]
    public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
    {
        var metrics = await _orderService.GetAdminDashboardMetricsAsync(cancellationToken);
        return View(metrics);
    }

    private static FlashcardsStore.Application.Common.PagedResult<T> BuildPagedResult<T>(
        IReadOnlyCollection<T> allItems,
        int page,
        int pageSize,
        out List<T> pagedItems)
    {
        var safeSize = Math.Clamp(pageSize, 5, 100);
        var safePage = Math.Max(1, page);
        pagedItems = allItems.Skip((safePage - 1) * safeSize).Take(safeSize).ToList();
        return new FlashcardsStore.Application.Common.PagedResult<T>
        {
            Items = pagedItems,
            TotalCount = allItems.Count,
            PageIndex = safePage,
            PageSize = safeSize
        };
    }

    [HttpGet]
    public async Task<IActionResult> Orders(
        OrderStatus? status,
        string? search,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<FlashcardsStore.Application.DTOs.Orders.OrderSummaryDto> orders = status.HasValue
            ? await _orderService.GetOrdersByStatusAsync(status.Value, cancellationToken)
            : await _orderService.GetRecentOrdersAsync(500, cancellationToken);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            orders = orders.Where(o =>
                (!string.IsNullOrEmpty(o.OrderNumber) && o.OrderNumber.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(o.CustomerName) && o.CustomerName.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(o.CustomerPhone) && o.CustomerPhone.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(o.Governorate) && o.Governorate.Contains(term, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        var pagedResult = BuildPagedResult(orders, page, pageSize, out var pagedOrders);

        ViewBag.CurrentStatus = status;
        ViewBag.SearchTerm = search;
        ViewBag.PaginationBaseUrl = $"/Admin/Orders?status={status}&search={Uri.EscapeDataString(search ?? "")}";
        ViewBag.PagedInfo = pagedResult;

        return View(pagedOrders);
    }

    [HttpGet]
    public async Task<IActionResult> OrderDetails(int id, CancellationToken cancellationToken)
    {

        var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddOrderNote(int id, string notes, OrderStatus? newStatus, CancellationToken cancellationToken)
    {

        var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);
        if (order == null) return NotFound();

        var statusToSet = newStatus ?? order.Status;
        var result = await _orderService.UpdateOrderStatusAsync(id, statusToSet, CurrentAdminUser, notes, cancellationToken);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "تم تحديث حالة الطلب وحفظ الملاحظات بنجاح ✓";
        }
        else
        {
            TempData["ErrorMessage"] = result.Error ?? "تعذر تحديث الطلب";
        }

        return RedirectToAction("OrderDetails", new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Approve(int id, CancellationToken cancellationToken)
    {

        var result = await _orderService.ApproveOrderAsync(id, CurrentAdminUser, cancellationToken);
        if (!result.Succeeded)
        {
            Response.Headers.Append("HX-Toast-Message", Uri.EscapeDataString(result.Error ?? "تعذر اعتماد الطلب"));
            Response.Headers.Append("HX-Toast-Type", "error");
        }
        else
        {
            Response.Headers.Append("HX-Toast-Message", Uri.EscapeDataString("تم اعتماد الطلب وخصم الكمية المحجوزة من المخزن بنجاح ✓"));
        }

        var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);
        return PartialView("_OrderStatusBadge", order?.Status ?? OrderStatus.Approved);
    }

    [HttpPost]
    public async Task<IActionResult> Reject(int id, string? reason, CancellationToken cancellationToken)
    {

        var result = await _orderService.RejectOrderAsync(id, CurrentAdminUser, reason, cancellationToken);
        if (!result.Succeeded)
        {
            Response.Headers.Append("HX-Toast-Message", Uri.EscapeDataString(result.Error ?? "تعذر رفض الطلب"));
            Response.Headers.Append("HX-Toast-Type", "error");
        }
        else
        {
            Response.Headers.Append("HX-Toast-Message", Uri.EscapeDataString("تم رفض الطلب وإعادة الكمية المحجوزة للمخزن"));
        }

        var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);
        return PartialView("_OrderStatusBadge", order?.Status ?? OrderStatus.Rejected);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, OrderStatus newStatus, string? notes, CancellationToken cancellationToken)
    {

        var result = await _orderService.UpdateOrderStatusAsync(id, newStatus, CurrentAdminUser, notes, cancellationToken);
        if (result.Succeeded)
        {
            Response.Headers.Append("HX-Toast-Message", Uri.EscapeDataString($"تم تحديث حالة الطلب إلى {newStatus}"));
        }

        var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);
        return PartialView("_OrderStatusBadge", order?.Status ?? newStatus);
    }

    // ==========================================
    // Products (Flashcards) CMS Management
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> Products(
        int? categoryId,
        string? status,
        string? search,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var allProducts = await _productService.GetAllProductsAsync(cancellationToken);
        var query = allProducts.AsEnumerable();

        if (categoryId.HasValue && categoryId.Value > 0)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        var normalizedStatus = string.IsNullOrWhiteSpace(status) ? "all" : status.Trim().ToLowerInvariant();
        query = normalizedStatus switch
        {
            "active" => query.Where(p => p.IsActive),
            "inactive" => query.Where(p => !p.IsActive),
            "lowstock" => query.Where(p => p.StockQuantity < 5),
            _ => query
        };

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p =>
                (!string.IsNullOrEmpty(p.NameAr) && p.NameAr.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(p.Name) && p.Name.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(p.GradeLevel) && p.GradeLevel.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(p.TargetAudience) && p.TargetAudience.Contains(term, StringComparison.OrdinalIgnoreCase))
            );
        }

        var filtered = query.ToList();
        var pagedResult = BuildPagedResult(filtered, page, pageSize, out var pagedProducts);

        var categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
        ViewBag.Categories = categories;
        ViewBag.SelectedCategoryId = categoryId;
        ViewBag.SelectedStatus = normalizedStatus;
        ViewBag.SearchTerm = search;
        ViewBag.PaginationBaseUrl = $"/Admin/Products?categoryId={categoryId}&status={normalizedStatus}&search={Uri.EscapeDataString(search ?? "")}";
        ViewBag.PagedInfo = pagedResult;

        return View(pagedProducts);
    }

    [HttpGet]
    public async Task<IActionResult> CreateProduct(CancellationToken cancellationToken)
    {

        ViewBag.Categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
        return View(new CreateProductDto
        {
            IsActive = true,
            Price = 195.00m,
            StockQuantity = 50,
            CardCount = 60,
            GradeLevel = "KG & Primary",
            TargetAudience = "KG1 - Primary 3",
            ImageUrl = string.Empty
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProduct(CreateProductDto dto, List<IFormFile>? imageFiles, IFormFile? audioFile, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Slug))
        {
            dto.Slug = await GenerateUniqueSlugAsync(dto.Name, dto.NameAr, null, cancellationToken);
            ModelState.Remove(nameof(dto.Slug));
        }

        var uploadedPaths = new List<string>();
        if (imageFiles != null && imageFiles.Count > 0)
        {
            foreach (var file in imageFiles)
            {
                if (file.Length > 0 && file.ContentType.StartsWith("image/"))
                {
                    using var stream = file.OpenReadStream();
                    var savedPath = await _fileStorageService.SaveFileAsync(stream, file.FileName, "products", cancellationToken);
                    uploadedPaths.Add(savedPath);
                }
            }
        }

        if (uploadedPaths.Count > 0)
        {
            dto.ImageUrl = string.Join(",", uploadedPaths);
        }
        else if (string.IsNullOrWhiteSpace(dto.ImageUrl))
        {
            dto.ImageUrl = "/images/cards/card-phonics-1.svg";
        }
        ModelState.Remove(nameof(dto.ImageUrl));

        // Handle Audio Voice Note Upload / In-Browser Recording
        if (audioFile != null && audioFile.Length > 0)
        {
            var ext = Path.GetExtension(audioFile.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext)) ext = ".webm";
            var fileName = audioFile.FileName.Contains('.') ? audioFile.FileName : $"{Guid.NewGuid():N}{ext}";
            using var audioStream = audioFile.OpenReadStream();
            var savedAudioPath = await _fileStorageService.SaveFileAsync(audioStream, fileName, "audio", cancellationToken);
            dto.AudioUrl = savedAudioPath;
        }
        ModelState.Remove(nameof(dto.AudioUrl));

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
            return View(dto);
        }

        var result = await _productService.CreateProductAsync(dto, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "تعذر إضافة الباقة");
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
            return View(dto);
        }

        TempData["SuccessMessage"] = $"تمت إضافة باقة «{dto.NameAr}» بنجاح ✓";
        return RedirectToAction("Products");
    }

    [HttpGet]
    public async Task<IActionResult> EditProduct(int id, CancellationToken cancellationToken)
    {

        var product = await _productService.GetByIdAsync(id, cancellationToken);
        if (product == null) return NotFound();

        ViewBag.Categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
        var updateDto = new UpdateProductDto
        {
            Id = product.Id,
            Name = product.Name,
            NameAr = product.NameAr,
            Slug = product.Slug,
            Description = product.Description,
            DescriptionAr = product.DescriptionAr,
            TargetAudience = product.TargetAudience,
            GradeLevel = product.GradeLevel,
            CardCount = product.CardCount,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            ImageUrl = product.ImageUrl,
            AudioUrl = product.AudioUrl,
            BackPreviewText = product.BackPreviewText,
            BackKeyPoints = product.BackKeyPoints,
            CategoryId = product.CategoryId,
            IsActive = product.IsActive,
            IsFeatured = product.IsFeatured
        };

        return View(updateDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(UpdateProductDto dto, List<IFormFile>? imageFiles, string? existingImages, IFormFile? audioFile, string? existingAudio, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Slug))
        {
            dto.Slug = await GenerateUniqueSlugAsync(dto.Name, dto.NameAr, dto.Id, cancellationToken);
            ModelState.Remove(nameof(dto.Slug));
        }

        var imagesList = (existingImages ?? "")
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrEmpty(s))
            .ToList();

        if (imageFiles != null && imageFiles.Count > 0)
        {
            foreach (var file in imageFiles)
            {
                if (file.Length > 0 && file.ContentType.StartsWith("image/"))
                {
                    using var stream = file.OpenReadStream();
                    var savedPath = await _fileStorageService.SaveFileAsync(stream, file.FileName, "products", cancellationToken);
                    imagesList.Add(savedPath);
                }
            }
        }

        if (imagesList.Count > 0)
        {
            dto.ImageUrl = string.Join(",", imagesList);
        }
        else if (string.IsNullOrWhiteSpace(dto.ImageUrl))
        {
            dto.ImageUrl = "/images/cards/card-phonics-1.svg";
        }
        ModelState.Remove(nameof(dto.ImageUrl));

        // Handle Audio Voice Note Upload / In-Browser Recording
        if (audioFile != null && audioFile.Length > 0)
        {
            var ext = Path.GetExtension(audioFile.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext)) ext = ".webm";
            var fileName = audioFile.FileName.Contains('.') ? audioFile.FileName : $"{Guid.NewGuid():N}{ext}";
            using var audioStream = audioFile.OpenReadStream();
            var savedAudioPath = await _fileStorageService.SaveFileAsync(audioStream, fileName, "audio", cancellationToken);
            dto.AudioUrl = savedAudioPath;
        }
        else if (!string.IsNullOrWhiteSpace(existingAudio))
        {
            dto.AudioUrl = existingAudio;
        }
        ModelState.Remove(nameof(dto.AudioUrl));

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
            return View(dto);
        }

        var result = await _productService.UpdateProductAsync(dto, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "تعذر تحديث الباقة");
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
            return View(dto);
        }

        TempData["SuccessMessage"] = $"تم تحديث باقة «{dto.NameAr}» بنجاح ✓";
        return RedirectToAction("Products");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProduct(int id, CancellationToken cancellationToken)
    {

        var result = await _productService.DeleteProductAsync(id, cancellationToken);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Error ?? "تعذر حذف الباقة";
        }
        else
        {
            TempData["SuccessMessage"] = "تم حذف باقة الفلاش كاردز بنجاح ✓";
        }

        return RedirectToAction("Products");
    }

    [HttpPost]
    public async Task<IActionResult> ToggleProductStatus(int id, CancellationToken cancellationToken)
    {

        var product = await _productService.GetByIdAsync(id, cancellationToken);
        if (product == null) return NotFound();

        var updateDto = new UpdateProductDto
        {
            Id = product.Id,
            Name = product.Name,
            NameAr = product.NameAr,
            Slug = product.Slug,
            Description = product.Description,
            DescriptionAr = product.DescriptionAr,
            TargetAudience = product.TargetAudience,
            GradeLevel = product.GradeLevel,
            CardCount = product.CardCount,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            ImageUrl = product.ImageUrl,
            BackPreviewText = product.BackPreviewText,
            BackKeyPoints = product.BackKeyPoints,
            CategoryId = product.CategoryId,
            IsActive = !product.IsActive,
            IsFeatured = product.IsFeatured
        };

        var result = await _productService.UpdateProductAsync(updateDto, cancellationToken);
        if (result.Succeeded)
        {
            Response.Headers.Append("HX-Toast-Message", Uri.EscapeDataString(updateDto.IsActive ? "تم تفعيل الباقة بالمتجر ✓" : "تم إيقاف عرض الباقة بالمتجر"));
        }

        return PartialView("_ProductStatusBadge", updateDto.IsActive);
    }

    // ==========================================
    // Categories CMS Management
    // ==========================================

    [HttpGet]
    public async Task<IActionResult> Categories(
        string? search,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var categories = (await _categoryService.GetAllCategoriesAsync(cancellationToken)).AsEnumerable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            categories = categories.Where(c =>
                (!string.IsNullOrEmpty(c.NameAr) && c.NameAr.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(c.Name) && c.Name.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(c.Slug) && c.Slug.Contains(term, StringComparison.OrdinalIgnoreCase)));
        }

        var filtered = categories.ToList();
        var pagedResult = BuildPagedResult(filtered, page, pageSize, out var pagedCategories);

        ViewBag.SearchTerm = search;
        ViewBag.PaginationBaseUrl = $"/Admin/Categories?search={Uri.EscapeDataString(search ?? "")}";
        ViewBag.PagedInfo = pagedResult;

        return View(pagedCategories);
    }

    [HttpGet]
    public IActionResult CreateCategory()
    {

        return View(new CreateCategoryDto { DisplayOrder = 10 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(CreateCategoryDto dto, CancellationToken cancellationToken)
    {

        if (string.IsNullOrWhiteSpace(dto.Slug))
        {
            dto.Slug = dto.Name.ToLowerInvariant().Replace(" ", "-");
        }

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var result = await _categoryService.CreateCategoryAsync(dto, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "تعذر إضافة القسم");
            return View(dto);
        }

        TempData["SuccessMessage"] = $"تم إنشاء قسم «{dto.NameAr}» بنجاح ✓";
        return RedirectToAction("Categories");
    }

    [HttpGet]
    public async Task<IActionResult> EditCategory(int id, CancellationToken cancellationToken)
    {

        var category = await _categoryService.GetByIdAsync(id, cancellationToken);
        if (category == null) return NotFound();

        return View(new UpdateCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            NameAr = category.NameAr,
            Slug = category.Slug,
            Description = category.Description,
            DisplayOrder = category.DisplayOrder
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCategory(UpdateCategoryDto dto, CancellationToken cancellationToken)
    {

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var result = await _categoryService.UpdateCategoryAsync(dto, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "تعذر تحديث القسم");
            return View(dto);
        }

        TempData["SuccessMessage"] = $"تم تحديث قسم «{dto.NameAr}» بنجاح ✓";
        return RedirectToAction("Categories");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken)
    {

        var result = await _categoryService.DeleteCategoryAsync(id, cancellationToken);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Error ?? "تعذر حذف القسم";
        }
        else
        {
            TempData["SuccessMessage"] = "تم حذف القسم بنجاح ✓";
        }

        return RedirectToAction("Categories");
    }

    [HttpGet]
    public async Task<IActionResult> Reviews(
        string? status = "all",
        string? search = null,
        int page = 1,
        int pageSize = 10)
    {
        var allReviews = await _reviewService.GetAllReviewsAsync();
        ViewBag.TotalReviewsCount = allReviews.Count;
        ViewBag.ApprovedReviewsCount = allReviews.Count(r => r.IsApproved);
        ViewBag.PendingReviewsCount = allReviews.Count(r => !r.IsApproved);
        ViewBag.AverageScore = allReviews.Count > 0 ? allReviews.Average(r => r.Rating).ToString("F1") : "5.0";

        var normalizedStatus = string.IsNullOrWhiteSpace(status) ? "all" : status.Trim().ToLowerInvariant();
        var query = allReviews.AsEnumerable();

        if (normalizedStatus == "approved") query = query.Where(r => r.IsApproved);
        else if (normalizedStatus == "pending") query = query.Where(r => !r.IsApproved);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(r =>
                (!string.IsNullOrEmpty(r.CustomerName) && r.CustomerName.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(r.ProductName) && r.ProductName.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(r.Comment) && r.Comment.Contains(term, StringComparison.OrdinalIgnoreCase)));
        }

        var filtered = query.ToList();
        var pagedResult = BuildPagedResult(filtered, page, pageSize, out var pagedReviews);

        ViewBag.SelectedStatus = normalizedStatus;
        ViewBag.SearchTerm = search;
        ViewBag.PaginationBaseUrl = $"/Admin/Reviews?status={normalizedStatus}&search={Uri.EscapeDataString(search ?? "")}";
        ViewBag.PagedInfo = pagedResult;

        return View(pagedReviews);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleReviewApproval(int id)
    {

        var result = await _reviewService.ToggleApprovalAsync(id);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error;
        }
        else
        {
            TempData["SuccessMessage"] = "تم تعديل حالة اعتماد التقييم بنجاح ✓";
        }

        return RedirectToAction("Reviews");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteReview(int id)
    {

        var result = await _reviewService.DeleteReviewAsync(id);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error;
        }
        else
        {
            TempData["SuccessMessage"] = "تم حذف التقييم بنجاح ✓";
        }

        return RedirectToAction("Reviews");
    }

    [HttpGet]
    public async Task<IActionResult> Settings(CancellationToken cancellationToken)
    {
        var settings = await _storeSettingsService.GetSettingsAsync(cancellationToken);
        return View(settings);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Settings(StoreSettingsDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "يرجى مراجعة الحقول المطلوبة والتأكد من صحة البيانات.";
            return View(dto);
        }

        var adminUser = User.Identity?.Name ?? "المشرف";
        var result = await _storeSettingsService.UpdateSettingsAsync(dto, adminUser, cancellationToken);
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Error ?? "حدث خطأ أثناء حفظ الإعدادات.";
            return View(dto);
        }

        TempData["SuccessMessage"] = "تم حفظ وتحديث إعدادات المتجر وبيانات التواصل والدفع بنجاح! ✓";
        return RedirectToAction(nameof(Settings));
    }

    [HttpGet]
    public async Task<IActionResult> Customers(
        string? search,
        string? filter = "all",
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        var summary = await _customerRepository.GetAdminCustomersSummaryAsync(search, filter, cancellationToken);
        var pagedResult = BuildPagedResult(summary.Customers, page, pageSize, out var pagedCustomers);
        summary.Customers = pagedCustomers;

        ViewBag.PaginationBaseUrl = $"/Admin/Customers?filter={summary.FilterType}&search={Uri.EscapeDataString(search ?? "")}";
        ViewBag.PagedInfo = pagedResult;

        return View(summary);
    }
}
