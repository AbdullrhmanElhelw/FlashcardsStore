using System.Net.Http.Json;
using System.Text.Json;
using FlashcardsStore.PhonicsGame.Models;
using FlashcardsStore.PhonicsGame.Services.Abstractions;

namespace FlashcardsStore.PhonicsGame.Services;

/// <summary>Handles online activation codes and local offline activation persistence.</summary>
public sealed class ActivationService : IActivationService
{
    private const string KeyActivated = "phonics_is_activated";
    private const string KeyCode = "phonics_activation_code";
    private const string KeyBundle = "phonics_bundle_name";
    private const string KeyDeviceId = "phonics_device_id";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly HttpClient _http;

    public event Action? ActivationChanged;

    public ActivationService(HttpClient http)
    {
        _http = http;
    }

    public bool IsActivated => Preferences.Default.Get(KeyActivated, false);
    public string ActivatedCode => Preferences.Default.Get(KeyCode, string.Empty);
    public string BundleName => Preferences.Default.Get(KeyBundle, string.Empty);

    public string GetOrCreateDeviceId()
    {
        var id = Preferences.Default.Get(KeyDeviceId, string.Empty);
        if (string.IsNullOrWhiteSpace(id))
        {
            id = Guid.NewGuid().ToString("N");
            Preferences.Default.Set(KeyDeviceId, id);
        }
        return id;
    }

    public async Task<ActivationResult> ActivateAsync(string rawCode, CancellationToken ct = default)
    {
        var code = (rawCode ?? string.Empty).Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(code))
            return new ActivationResult(false, "من فضلك أدخل كود التفعيل أولاً.", "Please enter an activation code.");

        try
        {
            var payload = new
            {
                Code = code,
                DeviceId = GetOrCreateDeviceId(),
                DeviceName = $"{DeviceInfo.Current.Manufacturer} {DeviceInfo.Current.Model}".Trim()
            };

            using var response = await _http.PostAsJsonAsync("api/phonics/activate", payload, ct);
            var body = await response.Content.ReadFromJsonAsync<PhonicsActivateApiResponse>(cancellationToken: ct);

            if (response.IsSuccessStatusCode && body is { IsActivated: true })
            {
                Preferences.Default.Set(KeyActivated, true);
                Preferences.Default.Set(KeyCode, body.Code ?? code);
                Preferences.Default.Set(KeyBundle, body.BundleName ?? "Full Version");
                ActivationChanged?.Invoke();
                return new ActivationResult(
                    true,
                    body.MessageAr ?? "تم تفعيل جميع المراحل بنجاح.",
                    body.MessageEn ?? "All stages unlocked successfully.",
                    body.BundleName);
            }

            return new ActivationResult(
                false,
                body?.MessageAr ?? "تعذر تفعيل الكود. تأكد من كتابته بشكل صحيح.",
                body?.MessageEn ?? "Could not activate code. Please verify and try again.");
        }
        catch (HttpRequestException)
        {
            return new ActivationResult(false, "تأكد من اتصالك بالإنترنت لتفعيل الكود لأول مرة.", "Please check your internet connection.");
        }
        catch (TaskCanceledException)
        {
            return new ActivationResult(false, "انتهت مهلة الاتصال بالسيرفر، حاول مرة أخرى.", "Connection timed out. Please try again.");
        }
        catch
        {
            return new ActivationResult(false, "حدث خطأ غير متوقع أثناء التفعيل.", "An unexpected error occurred.");
        }
    }

    public async Task<bool> VerifyActivationAsync(CancellationToken ct = default)
    {
        if (!IsActivated)
            return false;

        var code = ActivatedCode;
        if (string.IsNullOrWhiteSpace(code))
            return false;

        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(8));

            var payload = new
            {
                Code = code,
                DeviceId = GetOrCreateDeviceId()
            };

            using var response = await _http.PostAsJsonAsync("api/phonics/verify", payload, cts.Token);
            if (!response.IsSuccessStatusCode)
                return true;

            var body = await response.Content.ReadFromJsonAsync<PhonicsVerifyApiResponse>(JsonOptions, cts.Token);
            if (body is null)
                return true;

            if (!body.IsValid)
            {
                Preferences.Default.Remove(KeyActivated);
                Preferences.Default.Remove(KeyCode);
                Preferences.Default.Remove(KeyBundle);
                ActivationChanged?.Invoke();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(body.BundleName))
                Preferences.Default.Set(KeyBundle, body.BundleName);

            return true;
        }
        catch
        {
            // Keep offline activation intact when network is unavailable or times out.
            return true;
        }
    }

    private sealed record PhonicsVerifyApiResponse(bool IsValid, string? BundleName, string? Reason);
}
