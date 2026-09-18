using FlashcardsStore.Domain.Common;
using FlashcardsStore.Domain.Enums;

namespace FlashcardsStore.Domain.Entities;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal GrandTotal => TotalAmount + ShippingFee;
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.WalletTransfer;
    public int? PaymentProofId { get; set; }
    public PaymentProof? PaymentProof { get; set; }
    public bool WhatsAppConfirmationSent { get; set; }
    public string? CustomerNotes { get; set; }
    public string? AdminNotes { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
