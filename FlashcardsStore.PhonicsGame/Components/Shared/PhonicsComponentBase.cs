using FlashcardsStore.PhonicsGame.Services.Abstractions;
using FlashcardsStore.PhonicsGame.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FlashcardsStore.PhonicsGame.Components.Shared;

/// <summary>Base Blazor component that automatically re-renders whenever ViewModel state or app language changes.</summary>
public abstract class PhonicsComponentBase : ComponentBase, IDisposable
{
    [Inject] protected PhonicsGameViewModel Vm { get; set; } = default!;
    [Inject] protected ILocalizationService Loc { get; set; } = default!;

    protected override void OnInitialized()
    {
        Vm.StateChanged += HandleChanged;
        Loc.Changed += HandleChanged;
    }

    private void HandleChanged() => _ = InvokeAsync(StateHasChanged);

    public virtual void Dispose()
    {
        Vm.StateChanged -= HandleChanged;
        Loc.Changed -= HandleChanged;
    }
}
