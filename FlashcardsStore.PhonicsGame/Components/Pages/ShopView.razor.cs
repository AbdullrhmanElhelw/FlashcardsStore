using Microsoft.AspNetCore.Components;
using FlashcardsStore.PhonicsGame.Components.Shared;
using FlashcardsStore.PhonicsGame.ViewModels;

namespace FlashcardsStore.PhonicsGame.Components.Pages;

public partial class ShopView : PhonicsComponentBase<ShopViewModel>
{
    [Inject]
    public AppShellViewModel Shell { get; set; } = default!;
}
