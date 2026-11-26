using FlashcardsStore.Domain.Common;

namespace FlashcardsStore.Domain.Entities;

/// <summary>Represents a redeemable activation code that unlocks full Phonics Game stages.</summary>
public class PhonicsActivationCode : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string? BundleName { get; set; }
    public string? Notes { get; set; }
    public int MaxDevices { get; set; } = 2;
    public bool IsActive { get; set; } = true;
    public DateTime? ExpiresAtUtc { get; set; }

    public ICollection<PhonicsDeviceActivation> Devices { get; set; } = new List<PhonicsDeviceActivation>();
}
