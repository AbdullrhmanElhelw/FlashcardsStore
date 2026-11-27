using Microsoft.AspNetCore.Components;
using FlashcardsStore.PhonicsGame.Components.Shared;
using FlashcardsStore.PhonicsGame.ViewModels;

namespace FlashcardsStore.PhonicsGame.Components.Pages;

public partial class SoundboardView : PhonicsComponentBase<SoundboardViewModel>
{
    [Inject]
    public AppShellViewModel Shell { get; set; } = default!;
}
