using FluentValidation;
using FlashcardsStore.Application.DTOs.Orders;

namespace FlashcardsStore.Application.Validators;

public class CheckoutDtoValidator : AbstractValidator<CheckoutDto>
{
    public CheckoutDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("الاسم بالكامل مطلوب")
            .MinimumLength(3).WithMessage("الاسم يجب أن يكون 3 أحرف على الأقل");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("رقم الهاتف مطلوب")
            .Matches(@"^(010|011|012|015)[0-9]{8}$").WithMessage("يرجى إدخال رقم هاتف مصري صحيح (مثال: 01012345678)");

        RuleFor(x => x.ShippingAddress)
            .NotEmpty().WithMessage("عنوان التوصيل مطلوب")
            .MinimumLength(10).WithMessage("يرجى إدخال عنوان تفصيلي للتوصيل");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("المدينة / المركز مطلوب");

        RuleFor(x => x.Governorate)
            .NotEmpty().WithMessage("المحافظة مطلوبة");

        When(x => !string.IsNullOrWhiteSpace(x.Email), () =>
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("البريد الإلكتروني غير صحيح");
        });
    }
}
