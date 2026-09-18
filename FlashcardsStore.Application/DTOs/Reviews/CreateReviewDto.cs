using System.ComponentModel.DataAnnotations;

namespace FlashcardsStore.Application.DTOs.Reviews;

public class CreateReviewDto
{
    [Required]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "يرجى كتابة الاسم")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "الاسم يجب أن يكون بين حرفين و 100 حرف")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "يرجى اختيار صفتك (ولي أمر، معلمة، ...)")]
    public string Role { get; set; } = string.Empty;

    [Range(1, 5, ErrorMessage = "التقييم يجب أن يكون من 1 إلى 5 نجوم")]
    public int Rating { get; set; } = 5;

    [Required(ErrorMessage = "يرجى كتابة رأيك وتجربتك")]
    [StringLength(1000, MinimumLength = 5, ErrorMessage = "التعليق يجب ألا يقل عن 5 أحرف")]
    public string Comment { get; set; } = string.Empty;
}
