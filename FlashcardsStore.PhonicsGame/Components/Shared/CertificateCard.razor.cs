using FlashcardsStore.PhonicsGame.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FlashcardsStore.PhonicsGame.Components.Shared;

public partial class CertificateCard : PhonicsComponentBase<GameArenaViewModel>
{
    [Inject] private ModalsViewModel Modals { get; set; } = default!;
    [Inject] private AppShellViewModel Shell { get; set; } = default!;

    private string HeroName => string.IsNullOrWhiteSpace(Modals.Profile.Name)
        ? Loc.T("DefaultHeroName")
        : Modals.Profile.Name;

    private string StageName => Vm.ActiveStage is null
        ? Loc.T("AppTitle")
        : (Loc.IsArabic ? Vm.ActiveStage.TitleAr : Vm.ActiveStage.TitleEn);
}
