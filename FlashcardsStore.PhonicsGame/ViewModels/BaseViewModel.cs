using CommunityToolkit.Mvvm.ComponentModel;

namespace FlashcardsStore.PhonicsGame.ViewModels;

/// <summary>Base MVVM ViewModel powered by CommunityToolkit.Mvvm ObservableObject.</summary>
public abstract class BaseViewModel : ObservableObject
{
    protected void NotifyAll() => OnPropertyChanged(string.Empty);
}
