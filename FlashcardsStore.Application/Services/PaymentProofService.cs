using FlashcardsStore.Application.Common;
using FlashcardsStore.Application.DTOs.PaymentProof;
using FlashcardsStore.Application.Interfaces.Persistence;
using FlashcardsStore.Application.Interfaces.Services;
using FlashcardsStore.Domain.Entities;
using FlashcardsStore.Domain.Enums;

namespace FlashcardsStore.Application.Services;

public class PaymentProofService : IPaymentProofService
{
    private readonly IPaymentProofRepository _paymentProofRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IStockReservationService _stockReservationService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentProofService(
        IPaymentProofRepository paymentProofRepository,
        IOrderRepository orderRepository,
        IStockReservationService stockReservationService,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork)
    {
        _paymentProofRepository = paymentProofRepository;
        _orderRepository = orderRepository;
        _stockReservationService = stockReservationService;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaymentProofDto>> UploadProofAsync(PaymentProofUploadDto uploadDto, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetWithDetailsByIdAsync(uploadDto.OrderId, cancellationToken);
        if (order == null)
        {
            return Result<PaymentProofDto>.Failure("الطلب غير موجود");
        }

        if (order.Status == OrderStatus.Approved || order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Delivered)
        {
            return Result<PaymentProofDto>.Failure("تم تأكيد هذا الطلب بالفعل ولا يمكن رفع إيصال جديد");
        }

        var savedPath = await _fileStorageService.SaveFileAsync(
            uploadDto.FileStream,
            uploadDto.FileName,
            "receipts",
            cancellationToken);

        var proof = new PaymentProof
        {
            OrderId = order.Id,
            FilePath = savedPath,
            OriginalFileName = uploadDto.FileName,
            BankOrWalletName = uploadDto.BankOrWalletName,
            SenderPhoneNumber = uploadDto.SenderPhoneNumber,
            TransactionReference = uploadDto.TransactionReference,
            UploadedAt = DateTime.UtcNow,
            IsVerified = false
        };

        await _paymentProofRepository.AddAsync(proof, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        order.PaymentProofId = proof.Id;

        // Transition order status to PendingVerification
        if (order.Status == OrderStatus.Draft || order.Status == OrderStatus.Rejected || order.Status == OrderStatus.WhatsAppPending)
        {
            order.Status = OrderStatus.PendingVerification;

            // Reserve stock upon entering PendingVerification
            foreach (var item in order.Items)
            {
                await _stockReservationService.ReserveStockAsync(item.ProductId, item.Quantity, cancellationToken);
            }
        }

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PaymentProofDto>.Success(new PaymentProofDto
        {
            Id = proof.Id,
            OrderId = proof.OrderId,
            FilePath = proof.FilePath,
            OriginalFileName = proof.OriginalFileName,
            UploadedAt = proof.UploadedAt,
            BankOrWalletName = proof.BankOrWalletName,
            SenderPhoneNumber = proof.SenderPhoneNumber,
            TransactionReference = proof.TransactionReference,
            IsVerified = proof.IsVerified,
            VerifiedAt = proof.VerifiedAt,
            VerificationNotes = proof.VerificationNotes
        });
    }

    public async Task<PaymentProofDto?> GetProofByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var proof = await _paymentProofRepository.GetByOrderIdAsync(orderId, cancellationToken);
        if (proof == null) return null;

        return new PaymentProofDto
        {
            Id = proof.Id,
            OrderId = proof.OrderId,
            FilePath = proof.FilePath,
            OriginalFileName = proof.OriginalFileName,
            UploadedAt = proof.UploadedAt,
            BankOrWalletName = proof.BankOrWalletName,
            SenderPhoneNumber = proof.SenderPhoneNumber,
            TransactionReference = proof.TransactionReference,
            IsVerified = proof.IsVerified,
            VerifiedAt = proof.VerifiedAt,
            VerificationNotes = proof.VerificationNotes
        };
    }

    public async Task<Result<bool>> VerifyProofAsync(int proofId, string adminUsername, string? notes, CancellationToken cancellationToken = default)
    {
        var proof = await _paymentProofRepository.GetByIdAsync(proofId, cancellationToken);
        if (proof == null)
            return Result<bool>.Failure("إيصال الدفع غير موجود");

        proof.IsVerified = true;
        proof.VerifiedAt = DateTime.UtcNow;
        proof.VerificationNotes = notes;
        proof.UpdatedBy = adminUsername;

        await _paymentProofRepository.UpdateAsync(proof, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
