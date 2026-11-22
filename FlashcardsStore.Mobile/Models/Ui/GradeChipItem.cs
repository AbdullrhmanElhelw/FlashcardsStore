using CommunityToolkit.Mvvm.ComponentModel;

namespace FlashcardsStore.Mobile.Models.Ui;

public sealed partial class GradeChipItem : ObservableObject
{
    public string? Value { get; init; }
    public string Title { get; init; } = string.Empty;
    public string TitleEn { get; init; } = string.Empty;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private string _displayTitle = string.Empty;
}
