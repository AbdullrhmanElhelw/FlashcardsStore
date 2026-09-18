using FlashcardsStore.Application.Interfaces.Persistence;
using FlashcardsStore.Application.Interfaces.Services;

namespace FlashcardsStore.Application.Services;

public class StockReservationService : IStockReservationService
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StockReservationService(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> ReserveStockAsync(int productId, int quantity, CancellationToken cancellationToken = default)
    {
        if (quantity <= 0) return false;

        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null) return false;

        if (product.AvailableStockQuantity < quantity)
        {
            return false;
        }

        product.ReservedStockQuantity += quantity;
        await _productRepository.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ReleaseStockAsync(int productId, int quantity, CancellationToken cancellationToken = default)
    {
        if (quantity <= 0) return false;

        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null) return false;

        product.ReservedStockQuantity = Math.Max(0, product.ReservedStockQuantity - quantity);
        await _productRepository.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> CommitStockAsync(int productId, int quantity, CancellationToken cancellationToken = default)
    {
        if (quantity <= 0) return false;

        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null) return false;

        product.StockQuantity = Math.Max(0, product.StockQuantity - quantity);
        product.ReservedStockQuantity = Math.Max(0, product.ReservedStockQuantity - quantity);
        await _productRepository.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
