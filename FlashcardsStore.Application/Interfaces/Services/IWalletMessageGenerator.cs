using FlashcardsStore.Application.DTOs.Orders;

namespace FlashcardsStore.Application.Interfaces.Services;

public interface IWalletMessageGenerator
{
    string GenerateTransferInstructions(OrderDetailDto order);
    string GetVodafoneCashNumber();
    string GetInstaPayAddress();
}
