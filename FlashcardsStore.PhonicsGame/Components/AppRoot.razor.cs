using FlashcardsStore.PhonicsGame.Components.Shared;
using FlashcardsStore.PhonicsGame.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FlashcardsStore.PhonicsGame.Components;

public partial class AppRoot : PhonicsComponentBase<AppShellViewModel>
{
    [Inject] private IJSRuntime Js { get; set; } = default!;
    [Inject] private GameArenaViewModel Arena { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Vm.InitializeAsync(Js);
            await Arena.RegisterDragBridgeAsync();
        }
    }
}
