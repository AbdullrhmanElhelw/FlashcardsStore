using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashcardsStore.PhonicsGame.Data;
using FlashcardsStore.PhonicsGame.Models;
using FlashcardsStore.PhonicsGame.Services.Abstractions;

namespace FlashcardsStore.PhonicsGame.ViewModels;

/// <summary>Manages Activation and Child Profile modals cleanly.</summary>
public sealed partial class ModalsViewModel : BaseViewModel
{
    private readonly IActivationService _activation;
    private readonly IChildProfileService _profileService;
    private readonly ICurriculumRepository _curriculum;
    private readonly IGameProgressService _progress;
    private readonly ILocalizationService _loc;
    private readonly IJsBridgeService _js;

    [ObservableProperty] private bool _showActivationModal;
    [ObservableProperty] private string _activationCodeInput = string.Empty;
    [ObservableProperty] private string _activationFeedback = string.Empty;
    [ObservableProperty] private bool _activationSuccess;
    [ObservableProperty] private bool _isActivating;

    [ObservableProperty] private bool _showProfileModal;
    [ObservableProperty] private string _profileNameInput = string.Empty;
    [ObservableProperty] private string _profileAvatarInput = "lion";
    [ObservableProperty] private ChildProfile _profile;

    public ModalsViewModel(
        IActivationService activation,
        IChildProfileService profileService,
        ICurriculumRepository curriculum,
        IGameProgressService progress,
        ILocalizationService loc,
        IJsBridgeService js)
    {
        _activation = activation;
        _profileService = profileService;
        _curriculum = curriculum;
        _progress = progress;
        _loc = loc;
        _js = js;
        _profile = _profileService.GetProfile();
        _profileNameInput = _profile.Name;
        _profileAvatarInput = _profile.AvatarKey;
    }

    [RelayCommand]
    public void OpenActivationModal()
    {
        ActivationFeedback = string.Empty;
        ActivationSuccess = false;
        ShowActivationModal = true;
    }

    [RelayCommand]
    public void CloseActivationModal() => ShowActivationModal = false;

    [RelayCommand]
    public async Task SubmitActivationAsync()
    {
        IsActivating = true;
        ActivationFeedback = string.Empty;

        var result = await _activation.ActivateAsync(ActivationCodeInput);
        ActivationSuccess = result.Success;
        ActivationFeedback = _loc.IsArabic ? result.MessageAr : result.MessageEn;
        IsActivating = false;

        if (result.Success)
        {
            await _js.InvokeAsync("phonicsAudio.success");
            await _js.InvokeAsync("phonicsAudio.confetti");
            await Task.Delay(900);
            ShowActivationModal = false;
        }
    }

    [RelayCommand]
    public void OpenProfileModal()
    {
        ProfileNameInput = Profile.Name;
        ProfileAvatarInput = Profile.AvatarKey;
        ShowProfileModal = true;
    }

    [RelayCommand]
    public void CloseProfileModal() => ShowProfileModal = false;

    [RelayCommand]
    public void SaveProfile()
    {
        _profileService.SaveProfile(ProfileNameInput, ProfileAvatarInput, Profile.DailyGoalMinutes);
        Profile = _profileService.GetProfile();
        ShowProfileModal = false;
    }

    public void RefreshProfile() => Profile = _profileService.GetProfile();

    public IReadOnlyList<(string Key, string LabelAr, string LabelEn)> GetAvailableAvatars()
    {
        var list = AvatarArtwork.Avatars.ToList();
        var shopItems = _curriculum.GetShopItems();
        foreach (var sa in AvatarArtwork.ShopAvatars)
        {
            var shopItem = shopItems.FirstOrDefault(i => i.Kind == "avatar" && i.Key == sa.Key);
            if (shopItem is not null && _progress.IsShopItemUnlocked(shopItem.Id))
                list.Add(sa);
        }
        return list;
    }
}
