using FlashcardsStore.PhonicsGame.Components.Shared;
using FlashcardsStore.PhonicsGame.ViewModels;

namespace FlashcardsStore.PhonicsGame.Components.Arena;

public partial class TraceWriteArena : PhonicsComponentBase<GameArenaViewModel>
{
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await Vm.InitWordTraceAsync();
    }
}
