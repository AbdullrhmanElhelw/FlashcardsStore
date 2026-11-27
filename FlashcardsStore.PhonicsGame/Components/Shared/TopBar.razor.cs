using FlashcardsStore.PhonicsGame.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FlashcardsStore.PhonicsGame.Components.Shared;

public partial class TopBar : PhonicsComponentBase<AppShellViewModel>
{
    [Inject] private ModalsViewModel Modals { get; set; } = default!;
}
