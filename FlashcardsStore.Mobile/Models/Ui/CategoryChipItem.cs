using CommunityToolkit.Mvvm.ComponentModel;

namespace FlashcardsStore.Mobile.Models.Ui;

public sealed partial class CategoryChipItem : ObservableObject
{
    public int? Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string TitleEn { get; init; } = string.Empty;
    public string IconName { get; init; } = "home_house.png";
    public bool HasIcon => !string.IsNullOrWhiteSpace(IconName);

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private string _displayTitle = string.Empty;
}
