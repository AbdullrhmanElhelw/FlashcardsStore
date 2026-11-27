using Microsoft.AspNetCore.Components;
using FlashcardsStore.PhonicsGame.Components.Shared;
using FlashcardsStore.PhonicsGame.ViewModels;

namespace FlashcardsStore.PhonicsGame.Components.Pages;

public partial class HomeView : PhonicsComponentBase<HomeViewModel>
{
    [Inject]
    public ModalsViewModel Modals { get; set; } = default!;
}
