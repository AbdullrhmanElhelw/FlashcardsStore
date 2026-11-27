using System.ComponentModel;
using FlashcardsStore.PhonicsGame.Services.Abstractions;
using Microsoft.AspNetCore.Components;

namespace FlashcardsStore.PhonicsGame.Components.Shared;

/// <summary>Base Blazor component that automatically re-renders when app language changes.</summary>
public abstract class PhonicsComponentBase : ComponentBase, IDisposable
{
    [Inject] protected ILocalizationService Loc { get; set; } = default!;

    protected override void OnInitialized()
    {
        Loc.Changed += HandleChanged;
    }

    protected void HandleChanged() => _ = InvokeAsync(StateHasChanged);

    public virtual void Dispose()
    {
        Loc.Changed -= HandleChanged;
    }
}

/// <summary>Typed MVVM Blazor base component binding to a CommunityToolkit.Mvvm ObservableObject ViewModel.</summary>
public abstract class PhonicsComponentBase<TViewModel> : PhonicsComponentBase
    where TViewModel : class, INotifyPropertyChanged
{
    [Inject] protected TViewModel Vm { get; set; } = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Vm.PropertyChanged += OnVmPropertyChanged;
    }

    private void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
        => HandleChanged();

    public override void Dispose()
    {
        Vm.PropertyChanged -= OnVmPropertyChanged;
        base.Dispose();
    }
}
