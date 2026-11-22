using System.Collections;
using FlashcardsStore.Mobile.Models.Ui;
using FlashcardsStore.Mobile.Pages;

namespace FlashcardsStore.Mobile.Controls;

public partial class BrandDropdown : ContentView
{
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title), typeof(string), typeof(BrandDropdown), "اختر");

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource), typeof(IEnumerable), typeof(BrandDropdown), null,
        propertyChanged: (b, _, _) => ((BrandDropdown)b).RefreshDisplay());

    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
        nameof(SelectedItem), typeof(object), typeof(BrandDropdown), null, BindingMode.TwoWay,
        propertyChanged: (b, _, _) => ((BrandDropdown)b).SyncIndexFromItem());

    public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(
        nameof(SelectedIndex), typeof(int), typeof(BrandDropdown), -1, BindingMode.TwoWay,
        propertyChanged: (b, _, _) => ((BrandDropdown)b).SyncItemFromIndex());

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public IEnumerable? ItemsSource
    {
        get => (IEnumerable?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    public BrandDropdown()
    {
        InitializeComponent();
        RefreshDisplay();
    }

    private async void OnDropdownTapped(object? sender, TappedEventArgs e)
    {
        if (ItemsSource is null) return;
        var items = ItemsSource.Cast<object>().ToList();
        if (items.Count == 0) return;

        var options = items.Select((item, idx) => new PickerOptionItem
        {
            Index = idx,
            Label = item?.ToString() ?? string.Empty,
            Value = item,
            IsSelected = idx == SelectedIndex || (SelectedItem is not null && Equals(item, SelectedItem))
        }).ToList();

        var nav = Application.Current?.Windows.FirstOrDefault()?.Page?.Navigation;
        if (nav is null) return;

        var modal = new BrandPickerPage(Title, options);
        await nav.PushModalAsync(modal, false);
        var chosen = await modal.WaitForSelectionAsync();
        if (chosen is not null)
        {
            SelectedIndex = chosen.Index;
            SelectedItem = chosen.Value;
            RefreshDisplay();
        }
    }

    private void SyncIndexFromItem()
    {
        if (ItemsSource is null || SelectedItem is null)
        {
            RefreshDisplay();
            return;
        }
        var items = ItemsSource.Cast<object>().ToList();
        var idx = items.FindIndex(x => Equals(x, SelectedItem));
        if (idx >= 0 && SelectedIndex != idx) SelectedIndex = idx;
        RefreshDisplay();
    }

    private void SyncItemFromIndex()
    {
        if (ItemsSource is null)
        {
            RefreshDisplay();
            return;
        }
        var items = ItemsSource.Cast<object>().ToList();
        if (SelectedIndex >= 0 && SelectedIndex < items.Count)
        {
            var item = items[SelectedIndex];
            if (!Equals(SelectedItem, item)) SelectedItem = item;
        }
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        if (SelectedItem is not null)
        {
            DisplayLabel.Text = SelectedItem.ToString();
            return;
        }
        if (ItemsSource is not null && SelectedIndex >= 0)
        {
            var items = ItemsSource.Cast<object>().ToList();
            if (SelectedIndex < items.Count)
            {
                DisplayLabel.Text = items[SelectedIndex]?.ToString() ?? Title;
                return;
            }
        }
        DisplayLabel.Text = Title;
    }
}
