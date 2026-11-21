namespace FlashcardsStore.Mobile.Models.Ui;

public sealed class SortOptionItem
{
    public string Key { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;

    public override string ToString() => Title;
}
