using Microsoft.AspNetCore.Components;
using FlashcardsStore.PhonicsGame.Components.Shared;
using FlashcardsStore.PhonicsGame.ViewModels;

namespace FlashcardsStore.PhonicsGame.Components.Pages;

public partial class StoryPlayerView : PhonicsComponentBase<StoryPlayerViewModel>
{
    [Inject]
    public AppShellViewModel Shell { get; set; } = default!;
}
