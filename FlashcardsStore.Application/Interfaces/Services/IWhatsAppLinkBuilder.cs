using FlashcardsStore.Application.DTOs.Orders;

namespace FlashcardsStore.Application.Interfaces.Services;

public interface IWhatsAppLinkBuilder
{
    string BuildOrderLink(OrderDetailDto order);
    string BuildSupportLink(string? customMessage = null);
}
