namespace FlashcardsStore.Mobile.Models.Ui;

public sealed class PickerOptionItem
{
    public int Index { get; init; }
    public string Label { get; init; } = string.Empty;
    public object? Value { get; init; }
    public bool IsSelected { get; init; }
}
