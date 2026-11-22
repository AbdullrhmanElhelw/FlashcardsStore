namespace FlashcardsStore.Mobile.Models.Ui;

public sealed class SortOptionItem
{
    public string Key { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string TitleEn { get; init; } = string.Empty;
    public bool UseEnglish { get; set; }

    public override string ToString() => UseEnglish && !string.IsNullOrWhiteSpace(TitleEn) ? TitleEn : Title;
}
