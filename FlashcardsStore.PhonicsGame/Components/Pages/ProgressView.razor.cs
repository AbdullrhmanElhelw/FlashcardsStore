using Microsoft.AspNetCore.Components;
using FlashcardsStore.PhonicsGame.Components.Shared;
using FlashcardsStore.PhonicsGame.ViewModels;

namespace FlashcardsStore.PhonicsGame.Components.Pages;

public partial class ProgressView : PhonicsComponentBase<HomeViewModel>
{
    [Inject]
    public AppShellViewModel Shell { get; set; } = default!;

    [Inject]
    public ModalsViewModel Modals { get; set; } = default!;
}
