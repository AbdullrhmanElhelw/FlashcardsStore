using FlashcardsStore.Application.Common;
using FlashcardsStore.Application.DTOs.Reviews;
using FlashcardsStore.Application.Interfaces.Persistence;
using FlashcardsStore.Application.Interfaces.Services;
using FlashcardsStore.Domain.Entities;

namespace FlashcardsStore.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IRepository<Review> _reviewRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReviewService(
        IRepository<Review> reviewRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _reviewRepository = reviewRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<ReviewDto>> GetReviewsByProductIdAsync(int productId, bool approvedOnly = true)
    {
        var reviews = approvedOnly
            ? await _reviewRepository.FindAsync(r => r.ProductId == productId && r.IsApproved)
            : await _reviewRepository.FindAsync(r => r.ProductId == productId);

        return reviews
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                ProductId = r.ProductId,
                CustomerName = r.CustomerName,
                Role = r.Role,
                Rating = r.Rating,
                Comment = r.Comment,
                IsApproved = r.IsApproved,
                IsVerifiedBuyer = r.IsVerifiedBuyer,
                CreatedAt = r.CreatedAt
            })
            .ToList();
    }

    public async Task<ProductReviewSummaryDto> GetProductReviewSummaryAsync(int productId)
    {
        var reviews = await _reviewRepository.FindAsync(r => r.ProductId == productId && r.IsApproved);

        var total = reviews.Count;
        var avg = total > 0 ? Math.Round(reviews.Average(r => r.Rating), 1) : 5.0;

        return new ProductReviewSummaryDto
        {
            ProductId = productId,
            AverageRating = avg,
            TotalReviews = total,
            FiveStarCount = reviews.Count(r => r.Rating == 5),
            FourStarCount = reviews.Count(r => r.Rating == 4),
            ThreeStarCount = reviews.Count(r => r.Rating == 3),
            TwoStarCount = reviews.Count(r => r.Rating == 2),
            OneStarCount = reviews.Count(r => r.Rating == 1)
        };
    }

    public async Task<Result<ReviewDto>> AddReviewAsync(CreateReviewDto dto)
    {
        var product = await _productRepository.GetByIdAsync(dto.ProductId);
        if (product == null)
            return Result<ReviewDto>.Failure("المنتج غير موجود");

        var review = new Review
        {
            ProductId = dto.ProductId,
            CustomerName = dto.CustomerName.Trim(),
            Role = string.IsNullOrWhiteSpace(dto.Role) ? "ولي أمر" : dto.Role.Trim(),
            Rating = Math.Clamp(dto.Rating, 1, 5),
            Comment = dto.Comment.Trim(),
            IsApproved = true,
            IsVerifiedBuyer = true
        };

        await _reviewRepository.AddAsync(review);
        await _unitOfWork.SaveChangesAsync();

        return Result<ReviewDto>.Success(new ReviewDto
        {
            Id = review.Id,
            ProductId = review.ProductId,
            CustomerName = review.CustomerName,
            Role = review.Role,
            Rating = review.Rating,
            Comment = review.Comment,
            IsApproved = review.IsApproved,
            IsVerifiedBuyer = review.IsVerifiedBuyer,
            CreatedAt = review.CreatedAt
        });
    }

    public async Task<IReadOnlyList<ReviewDto>> GetAllReviewsAsync()
    {
        var reviews = await _reviewRepository.GetAllAsync();
        var products = (await _productRepository.GetAllAsync()).ToDictionary(p => p.Id, p => (p.Name, p.NameAr));

        return reviews
            .OrderByDescending(r => r.CreatedAt)
            .Select(r =>
            {
                var hasProduct = products.TryGetValue(r.ProductId, out var names);
                return new ReviewDto
                {
                    Id = r.Id,
                    ProductId = r.ProductId,
                    CustomerName = r.CustomerName,
                    Role = r.Role,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    IsApproved = r.IsApproved,
                    IsVerifiedBuyer = r.IsVerifiedBuyer,
                    CreatedAt = r.CreatedAt,
                    ProductName = hasProduct ? names.Name : "Unknown",
                    ProductNameAr = hasProduct ? names.NameAr : "غير محدد"
                };
            })
            .ToList();
    }

    public async Task<Result> ToggleApprovalAsync(int reviewId)
    {
        var review = await _reviewRepository.GetByIdAsync(reviewId);
        if (review == null)
            return Result.Failure("التقييم غير موجود");

        review.IsApproved = !review.IsApproved;
        await _reviewRepository.UpdateAsync(review);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteReviewAsync(int reviewId)
    {
        var review = await _reviewRepository.GetByIdAsync(reviewId);
        if (review == null)
            return Result.Failure("التقييم غير موجود");

        await _reviewRepository.DeleteAsync(review);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
