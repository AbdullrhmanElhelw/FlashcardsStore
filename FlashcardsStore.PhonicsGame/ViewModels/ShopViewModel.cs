using CommunityToolkit.Mvvm.ComponentModel;
using FlashcardsStore.PhonicsGame.Models;
using FlashcardsStore.PhonicsGame.Services.Abstractions;

namespace FlashcardsStore.PhonicsGame.ViewModels;

/// <summary>Manages the Reward Shop items, purchases, and equipping avatars/themes.</summary>
public sealed partial class ShopViewModel : BaseViewModel
{
    private readonly ICurriculumRepository _curriculum;
    private readonly IGameProgressService _progress;
    private readonly IChildProfileService _profileService;
    private readonly ILocalizationService _loc;
    private readonly IJsBridgeService _js;
    private readonly ModalsViewModel _modals;
    private readonly AppShellViewModel _shell;

    [ObservableProperty] private string _feedback = string.Empty;

    public ShopViewModel(
        ICurriculumRepository curriculum,
        IGameProgressService progress,
        IChildProfileService profileService,
        ILocalizationService loc,
        IJsBridgeService js,
        ModalsViewModel modals,
        AppShellViewModel shell)
    {
        _curriculum = curriculum;
        _progress = progress;
        _profileService = profileService;
        _loc = loc;
        _js = js;
        _modals = modals;
        _shell = shell;
        _curriculum.ContentReloaded += NotifyAll;
    }

    public IReadOnlyList<ShopItem> Items => _curriculum.GetShopItems();
    public int AvailableStars => _progress.AvailableStars;

    public void Reset() => Feedback = string.Empty;

    public bool IsUnlocked(string itemId) => _progress.IsShopItemUnlocked(itemId);

    public bool IsEquipped(ShopItem item) => item.Kind switch
    {
        "avatar" => string.Equals(_modals.Profile.AvatarKey, item.Key, StringComparison.OrdinalIgnoreCase),
        "theme" => string.Equals(_progress.EquippedTheme, item.Key, StringComparison.OrdinalIgnoreCase),
        _ => false
    };

    public async Task BuyOrEquipAsync(ShopItem item)
    {
        Feedback = string.Empty;
        if (!IsUnlocked(item.Id))
        {
            if (!_progress.TryUnlockShopItem(item.Id, item.CostStars))
            {
                Feedback = _loc.T("ShopNotEnough");
                await _js.InvokeAsync("phonicsAudio.wrong");
                return;
            }
            await _js.InvokeAsync("phonicsAudio.success");
            await _js.InvokeAsync("phonicsAudio.confetti");
        }
        else
        {
            await _js.InvokeAsync("phonicsAudio.pop");
        }

        if (item.Kind == "avatar")
        {
            _profileService.SaveProfile(_modals.Profile.Name, item.Key, _modals.Profile.DailyGoalMinutes);
            _modals.RefreshProfile();
        }
        else if (item.Kind == "theme")
        {
            _progress.SetEquippedTheme(item.Key);
        }

        _shell.RefreshHeader();
        NotifyAll();
    }
}
