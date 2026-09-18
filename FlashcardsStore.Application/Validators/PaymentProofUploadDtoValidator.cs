using FluentValidation;
using FlashcardsStore.Application.DTOs.PaymentProof;

namespace FlashcardsStore.Application.Validators;

public class PaymentProofUploadDtoValidator : AbstractValidator<PaymentProofUploadDto>
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".pdf" };

    public PaymentProofUploadDtoValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0).WithMessage("رقم الطلب غير صحيح");

        RuleFor(x => x.FileLength)
            .GreaterThan(0).WithMessage("يرجى اختيار ملف الإيصال أو لقطة الشاشة")
            .LessThanOrEqualTo(10 * 1024 * 1024).WithMessage("أقصى حجم مسموح به للملف هو 10 ميجابايت");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("اسم الملف مطلوب")
            .Must(fileName =>
            {
                var ext = Path.GetExtension(fileName).ToLowerInvariant();
                return AllowedExtensions.Contains(ext);
            }).WithMessage("صيغة الملف غير مدعومة. الصيغ المسموحة: JPG, PNG, WEBP, PDF");

        When(x => !string.IsNullOrWhiteSpace(x.SenderPhoneNumber), () =>
        {
            RuleFor(x => x.SenderPhoneNumber)
                .Matches(@"^(010|011|012|015)[0-9]{8}$").WithMessage("رقم محفظة المحول غير صحيح");
        });
    }
}
