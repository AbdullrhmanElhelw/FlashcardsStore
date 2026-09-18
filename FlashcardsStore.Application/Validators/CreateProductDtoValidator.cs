using FluentValidation;
using FlashcardsStore.Application.DTOs.Products;

namespace FlashcardsStore.Application.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("اسم المنتج بالإنجليزية مطلوب");

        RuleFor(x => x.NameAr)
            .NotEmpty().WithMessage("اسم المنتج بالعربية مطلوب");

        When(x => !string.IsNullOrWhiteSpace(x.Slug), () =>
        {
            RuleFor(x => x.Slug)
                .Matches(@"^[a-z0-9-]+$").WithMessage("الرابط المخصص يجب أن يحتوي على أحرف صغيرة وأرقام وشرطات فقط");
        });

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("السعر يجب أن يكون أكبر من صفر");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("الكمية بالمخزن لا يمكن أن تكون سالبة");

        RuleFor(x => x.CardCount)
            .GreaterThan(0).WithMessage("عدد البطاقات يجب أن يكون أكبر من صفر");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("يجب اختيار تصنيف للمنتج");
    }
}
