using CommunityToolkit.Mvvm.ComponentModel;

namespace FlashcardsStore.Mobile.Models.Ui;

public sealed partial class GradeChipItem : ObservableObject
{
    public string? Value { get; init; }
    public string Title { get; init; } = string.Empty;

    [ObservableProperty]
    private bool _isSelected;
}
