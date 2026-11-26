using FlashcardsStore.Domain.Common;

namespace FlashcardsStore.Domain.Entities;

/// <summary>Tracks an individual device bound to a Phonics Game activation code.</summary>
public class PhonicsDeviceActivation : BaseEntity
{
    public int PhonicsActivationCodeId { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public string? DeviceName { get; set; }
    public DateTime ActivatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime LastVerifiedAtUtc { get; set; } = DateTime.UtcNow;

    public PhonicsActivationCode ActivationCode { get; set; } = null!;
}
