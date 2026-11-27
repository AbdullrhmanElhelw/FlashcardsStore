using Microsoft.AspNetCore.Components;
using FlashcardsStore.PhonicsGame.Components.Shared;
using FlashcardsStore.PhonicsGame.ViewModels;

namespace FlashcardsStore.PhonicsGame.Components.Pages;

public partial class GameArenaView : PhonicsComponentBase<GameArenaViewModel>
{
    [Inject]
    public AppShellViewModel Shell { get; set; } = default!;
}
