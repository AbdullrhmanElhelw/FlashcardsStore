using FlashcardsStore.Application.Common;
using FlashcardsStore.Application.DTOs.Admin;
using FlashcardsStore.Application.DTOs.Cart;
using FlashcardsStore.Application.DTOs.Orders;
using FlashcardsStore.Application.DTOs.PaymentProof;
using FlashcardsStore.Application.Interfaces.Persistence;
using FlashcardsStore.Application.Interfaces.Services;
using FlashcardsStore.Domain.Entities;
using FlashcardsStore.Domain.Enums;

namespace FlashcardsStore.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly IStockReservationService _stockReservationService;
    private readonly IWhatsAppLinkBuilder _whatsAppLinkBuilder;
    private readonly IWalletMessageGenerator _walletMessageGenerator;
    private readonly IStoreSettingsService _storeSettingsService;
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        IStockReservationService stockReservationService,
        IWhatsAppLinkBuilder whatsAppLinkBuilder,
        IWalletMessageGenerator walletMessageGenerator,
        IStoreSettingsService storeSettingsService,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _stockReservationService = stockReservationService;
        _whatsAppLinkBuilder = whatsAppLinkBuilder;
        _walletMessageGenerator = walletMessageGenerator;
        _storeSettingsService = storeSettingsService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrderDetailDto>> CreateOrderAsync(
        CheckoutDto checkout,
        List<CartItemDto> items,
        CancellationToken cancellationToken = default)
    {
        if (items == null || items.Count == 0)
        {
            return Result<OrderDetailDto>.Failure("سلة الشراء فارغة");
        }

        // Validate stock availability
        foreach (var item in items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product == null || product.AvailableStockQuantity < item.Quantity)
            {
                var name = product?.NameAr ?? item.ProductNameAr;
                return Result<OrderDetailDto>.Failure($"عفواً، الكمية المطلوبة غير متوفرة حالياً من: {name}");
            }
        }

        // Customer resolution
        var customer = await _customerRepository.GetByPhoneAsync(checkout.PhoneNumber, cancellationToken);
        if (customer == null)
        {
            customer = new Customer
            {
                FullName = checkout.FullName,
                PhoneNumber = checkout.PhoneNumber,
                WhatsAppNumber = checkout.WhatsAppNumber ?? checkout.PhoneNumber,
                Email = checkout.Email,
                ShippingAddress = checkout.ShippingAddress,
                City = checkout.City,
                Governorate = checkout.Governorate,
                CustomerType = checkout.CustomerType
            };
            await _customerRepository.AddAsync(customer, cancellationToken);
        }
        else
        {
            customer.FullName = checkout.FullName;
            customer.WhatsAppNumber = checkout.WhatsAppNumber ?? checkout.PhoneNumber;
            customer.ShippingAddress = checkout.ShippingAddress;
            customer.City = checkout.City;
            customer.Governorate = checkout.Governorate;
            customer.CustomerType = checkout.CustomerType;
            if (!string.IsNullOrWhiteSpace(checkout.Email))
                customer.Email = checkout.Email;

            await _customerRepository.UpdateAsync(customer, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Generate Order Number: EGY-YYYYMMDD-XXXX
        var randomSuffix = Random.Shared.Next(1000, 9999);
        var orderNumber = $"EGY-{DateTime.UtcNow:yyyyMMdd}-{randomSuffix}";

        // Calculate shipping fee according to Egyptian governorate & store settings
        var totalAmount = items.Sum(i => i.SubTotal);
        var shippingFee = _storeSettingsService.CalculateShippingFee(checkout.Governorate, totalAmount);

        var order = new Order
        {
            OrderNumber = orderNumber,
            CustomerId = customer.Id,
            Customer = customer,
            TotalAmount = totalAmount,
            ShippingFee = shippingFee,
            PaymentMethod = checkout.PaymentMethod,
            CustomerNotes = checkout.Notes,
            OrderDate = DateTime.UtcNow,
            Status = checkout.PaymentMethod == PaymentMethod.WalletTransfer
                ? OrderStatus.PendingVerification
                : OrderStatus.WhatsAppPending
        };

        foreach (var item in items)
        {
            order.Items.Add(new OrderItem
            {
                ProductId = item.ProductId,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity
            });
        }

        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Soft reserve stock only if WalletTransfer
        if (checkout.PaymentMethod == PaymentMethod.WalletTransfer)
        {
            foreach (var item in items)
            {
                await _stockReservationService.ReserveStockAsync(item.ProductId, item.Quantity, cancellationToken);
            }
        }

        var orderDetail = MapToDetailDto(order);
        orderDetail.WhatsAppDirectLink = _whatsAppLinkBuilder.BuildOrderLink(orderDetail);
        orderDetail.WalletPaymentInstructions = _walletMessageGenerator.GenerateTransferInstructions(orderDetail);

        return Result<OrderDetailDto>.Success(orderDetail);
    }

    public async Task<OrderDetailDto?> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetWithDetailsByIdAsync(id, cancellationToken);
        if (order == null) return null;

        var dto = MapToDetailDto(order);
        dto.WhatsAppDirectLink = _whatsAppLinkBuilder.BuildOrderLink(dto);
        dto.WalletPaymentInstructions = _walletMessageGenerator.GenerateTransferInstructions(dto);
        return dto;
    }

    public async Task<OrderDetailDto?> GetOrderByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetWithDetailsByOrderNumberAsync(orderNumber, cancellationToken);
        if (order == null) return null;

        var dto = MapToDetailDto(order);
        dto.WhatsAppDirectLink = _whatsAppLinkBuilder.BuildOrderLink(dto);
        dto.WalletPaymentInstructions = _walletMessageGenerator.GenerateTransferInstructions(dto);
        return dto;
    }

    public async Task<OrderTrackingDto?> TrackOrderAsync(string orderNumber, string phoneNumber, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetWithDetailsByOrderNumberAsync(orderNumber.Trim(), cancellationToken);
        if (order == null) return null;

        var cleanPhone = phoneNumber.Trim().Replace(" ", "").Replace("+2", "");
        var customerPhone = order.Customer?.PhoneNumber?.Trim().Replace(" ", "").Replace("+2", "") ?? "";

        if (!customerPhone.EndsWith(cleanPhone) && !cleanPhone.EndsWith(customerPhone))
        {
            return null;
        }

        return new OrderTrackingDto
        {
            OrderNumber = order.OrderNumber,
            CustomerName = order.Customer?.FullName ?? "",
            OrderDate = order.OrderDate,
            Status = order.Status,
            PaymentMethod = order.PaymentMethod,
            GrandTotal = order.GrandTotal,
            ItemCount = order.Items.Sum(i => i.Quantity),
            HasUploadedProof = order.PaymentProofId.HasValue,
            StatusDescriptionAr = GetStatusDescriptionAr(order.Status),
            StatusDescription = GetStatusDescriptionEn(order.Status)
        };
    }

    public async Task<IReadOnlyList<OrderSummaryDto>> GetOrdersByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetOrdersByStatusAsync(status, cancellationToken);
        return orders.Select(MapToSummaryDto).ToList();
    }

    public async Task<IReadOnlyList<OrderSummaryDto>> GetRecentOrdersAsync(int count = 20, CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetRecentOrdersAsync(count, cancellationToken);
        return orders.Select(MapToSummaryDto).ToList();
    }

    public async Task<Result<bool>> ApproveOrderAsync(int orderId, string adminUsername, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetWithDetailsByIdAsync(orderId, cancellationToken);
        if (order == null)
            return Result<bool>.Failure("الطلب غير موجود");

        if (order.Status == OrderStatus.Approved || order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Delivered)
            return Result<bool>.Failure("الطلب معتمد بالفعل");

        // Commit stock
        foreach (var item in order.Items)
        {
            if (order.Status == OrderStatus.PendingVerification)
            {
                await _stockReservationService.CommitStockAsync(item.ProductId, item.Quantity, cancellationToken);
            }
            else
            {
                // If approving WhatsAppPending order, deduct stock directly
                var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
                if (product != null)
                {
                    product.StockQuantity = Math.Max(0, product.StockQuantity - item.Quantity);
                    await _productRepository.UpdateAsync(product, cancellationToken);
                }
            }
        }

        order.Status = OrderStatus.Approved;
        order.UpdatedBy = adminUsername;
        order.UpdatedAt = DateTime.UtcNow;

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RejectOrderAsync(int orderId, string adminUsername, string? reason, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetWithDetailsByIdAsync(orderId, cancellationToken);
        if (order == null)
            return Result<bool>.Failure("الطلب غير موجود");

        if (order.Status == OrderStatus.PendingVerification)
        {
            // Release soft-reserved stock
            foreach (var item in order.Items)
            {
                await _stockReservationService.ReleaseStockAsync(item.ProductId, item.Quantity, cancellationToken);
            }
        }

        order.Status = OrderStatus.Rejected;
        order.AdminNotes = reason;
        order.UpdatedBy = adminUsername;
        order.UpdatedAt = DateTime.UtcNow;

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, string adminUsername, string? notes, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetWithDetailsByIdAsync(orderId, cancellationToken);
        if (order == null)
            return Result<bool>.Failure("الطلب غير موجود");

        if (newStatus == OrderStatus.Cancelled && order.Status == OrderStatus.PendingVerification)
        {
            foreach (var item in order.Items)
            {
                await _stockReservationService.ReleaseStockAsync(item.ProductId, item.Quantity, cancellationToken);
            }
        }

        order.Status = newStatus;
        if (!string.IsNullOrWhiteSpace(notes))
            order.AdminNotes = notes;

        order.UpdatedBy = adminUsername;
        order.UpdatedAt = DateTime.UtcNow;

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<int> ReleaseExpiredStockReservationsAsync(CancellationToken cancellationToken = default)
    {
        var cutoff = DateTime.UtcNow.AddHours(-24);
        var expiredOrders = await _orderRepository.GetExpiredPendingVerificationOrdersAsync(cutoff, cancellationToken);

        int releasedCount = 0;
        foreach (var order in expiredOrders)
        {
            foreach (var item in order.Items)
            {
                await _stockReservationService.ReleaseStockAsync(item.ProductId, item.Quantity, cancellationToken);
            }

            order.Status = OrderStatus.Cancelled;
            order.AdminNotes = "تم إلغاء الطلب تلقائياً: انتهت مهلة الـ 24 ساعة دون إرفاق وتأكيد إيصال التحويل";
            order.UpdatedAt = DateTime.UtcNow;
            order.UpdatedBy = "System:24hStockCleanup";

            await _orderRepository.UpdateAsync(order, cancellationToken);
            releasedCount++;
        }

        if (releasedCount > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return releasedCount;
    }

    public async Task<AdminDashboardDto> GetAdminDashboardMetricsAsync(CancellationToken cancellationToken = default)
    {
        var totalOrders = await _orderRepository.CountAsync(null, cancellationToken);
        var pendingVerif = await _orderRepository.CountByStatusAsync(OrderStatus.PendingVerification, cancellationToken);
        var whatsappPending = await _orderRepository.CountByStatusAsync(OrderStatus.WhatsAppPending, cancellationToken);
        var approved = await _orderRepository.CountByStatusAsync(OrderStatus.Approved, cancellationToken);
        var recentOrders = await _orderRepository.GetRecentOrdersAsync(10, cancellationToken);

        var allProducts = await _productRepository.GetAllAsync(cancellationToken);
        var lowStockCount = allProducts.Count(p => p.AvailableStockQuantity <= 5);

        var approvedOrders = await _orderRepository.GetOrdersByStatusAsync(OrderStatus.Approved, cancellationToken);
        var totalRevenue = approvedOrders.Sum(o => o.TotalAmount);

        return new AdminDashboardDto
        {
            TotalOrders = totalOrders,
            PendingVerificationOrders = pendingVerif,
            WhatsAppPendingOrders = whatsappPending,
            ApprovedOrders = approved,
            LowStockProductsCount = lowStockCount,
            TotalRevenue = totalRevenue,
            RecentOrders = recentOrders.Select(MapToSummaryDto).ToList()
        };
    }

    private decimal CalculateShippingFee(string governorate, decimal subtotal = 0m) =>
        _storeSettingsService.CalculateShippingFee(governorate, subtotal);

    private static OrderDetailDto MapToDetailDto(Order order) => new()
    {
        Id = order.Id,
        OrderNumber = order.OrderNumber,
        OrderDate = order.OrderDate,
        Status = order.Status,
        PaymentMethod = order.PaymentMethod,
        TotalAmount = order.TotalAmount,
        ShippingFee = order.ShippingFee,
        CustomerNotes = order.CustomerNotes,
        AdminNotes = order.AdminNotes,
        CustomerName = order.Customer?.FullName ?? "",
        CustomerPhone = order.Customer?.PhoneNumber ?? "",
        CustomerWhatsApp = order.Customer?.WhatsAppNumber,
        CustomerEmail = order.Customer?.Email,
        ShippingAddress = order.Customer?.ShippingAddress ?? "",
        City = order.Customer?.City ?? "",
        Governorate = order.Customer?.Governorate ?? "",
        CustomerType = order.Customer?.CustomerType ?? CustomerType.Parent,
        Items = order.Items.Select(i => new OrderItemDetailDto
        {
            ProductId = i.ProductId,
            ProductName = i.Product?.Name ?? "",
            ProductNameAr = i.Product?.NameAr ?? "",
            ImageUrl = i.Product?.ImageUrl ?? "",
            UnitPrice = i.UnitPrice,
            Quantity = i.Quantity
        }).ToList(),
        PaymentProof = order.PaymentProof == null ? null : new PaymentProofDto
        {
            Id = order.PaymentProof.Id,
            OrderId = order.PaymentProof.OrderId,
            FilePath = order.PaymentProof.FilePath,
            OriginalFileName = order.PaymentProof.OriginalFileName,
            UploadedAt = order.PaymentProof.UploadedAt,
            BankOrWalletName = order.PaymentProof.BankOrWalletName,
            SenderPhoneNumber = order.PaymentProof.SenderPhoneNumber,
            TransactionReference = order.PaymentProof.TransactionReference,
            IsVerified = order.PaymentProof.IsVerified,
            VerifiedAt = order.PaymentProof.VerifiedAt,
            VerificationNotes = order.PaymentProof.VerificationNotes
        }
    };

    private static OrderSummaryDto MapToSummaryDto(Order o) => new()
    {
        Id = o.Id,
        OrderNumber = o.OrderNumber,
        CustomerName = o.Customer?.FullName ?? "",
        CustomerPhone = o.Customer?.PhoneNumber ?? "",
        Governorate = o.Customer?.Governorate ?? "",
        CustomerType = o.Customer?.CustomerType ?? CustomerType.Parent,
        GrandTotal = o.GrandTotal,
        Status = o.Status,
        PaymentMethod = o.PaymentMethod,
        HasPaymentProof = o.PaymentProofId.HasValue,
        OrderDate = o.OrderDate,
        TotalItems = o.Items?.Sum(i => i.Quantity) ?? 0
    };

    private static string GetStatusDescriptionAr(OrderStatus status) => status switch
    {
        OrderStatus.Draft => "مسودة طلب جديدة",
        OrderStatus.WhatsAppPending => "في انتظار المراسلة عبر واتساب والتأكيد",
        OrderStatus.PendingVerification => "تم حجز المنتجات مؤقتاً - في انتظار مراجعة إيصال التحويل",
        OrderStatus.Approved => "تم التأكيد واعتماد الطلب وجارٍ التجهيز للشحن",
        OrderStatus.Shipped => "تم شحن الطلب وهو في طريقه إليك مع شركة الشحن",
        OrderStatus.Delivered => "تم تسليم الطلب بنجاح",
        OrderStatus.Rejected => "تم رفض الطلب لعدم صحة بيانات التحويل",
        OrderStatus.Cancelled => "تم إلغاء الطلب",
        _ => "غير محدد"
    };

    private static string GetStatusDescriptionEn(OrderStatus status) => status switch
    {
        OrderStatus.Draft => "New Order Draft",
        OrderStatus.WhatsAppPending => "Awaiting WhatsApp confirmation and coordination",
        OrderStatus.PendingVerification => "Stock reserved temporarily - Awaiting payment receipt verification",
        OrderStatus.Approved => "Order confirmed & approved - Preparing for shipping",
        OrderStatus.Shipped => "Order shipped and on its way with the courier",
        OrderStatus.Delivered => "Order successfully delivered",
        OrderStatus.Rejected => "Order rejected due to invalid payment verification",
        OrderStatus.Cancelled => "Order cancelled",
        _ => "Unknown"
    };
}
