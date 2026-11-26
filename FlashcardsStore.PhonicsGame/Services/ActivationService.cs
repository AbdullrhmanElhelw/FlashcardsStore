using System.Net.Http.Json;

namespace FlashcardsStore.PhonicsGame.Services;

/// <summary>Handles online activation codes and local offline activation persistence.</summary>
public sealed class ActivationService
{
    private const string KeyActivated = "phonics_is_activated";
    private const string KeyCode = "phonics_activation_code";
    private const string KeyBundle = "phonics_bundle_name";
    private const string KeyDeviceId = "phonics_device_id";

    private readonly HttpClient _http;

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

    public sealed record ActivationResult(bool Success, string MessageAr, string? BundleName = null);

    private sealed record ApiResponse(
        bool IsActivated,
        string? Code,
        string? BundleName,
        int MaxDevices,
        int UsedDevices,
        int RemainingDevices,
        string? MessageAr,
        string? MessageEn);

    public async Task<ActivationResult> ActivateAsync(string rawCode, CancellationToken ct = default)
    {
        var code = (rawCode ?? string.Empty).Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(code))
            return new ActivationResult(false, "من فضلك أدخل كود التفعيل أولاً.");

        try
        {
            var payload = new
            {
                Code = code,
                DeviceId = GetOrCreateDeviceId(),
                DeviceName = $"{DeviceInfo.Current.Manufacturer} {DeviceInfo.Current.Model}".Trim()
            };

            using var response = await _http.PostAsJsonAsync("api/phonics/activate", payload, ct);
            var body = await response.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: ct);

            if (response.IsSuccessStatusCode && body is { IsActivated: true })
            {
                Preferences.Default.Set(KeyActivated, true);
                Preferences.Default.Set(KeyCode, body.Code ?? code);
                Preferences.Default.Set(KeyBundle, body.BundleName ?? "النسخة الكاملة");
                return new ActivationResult(true, body.MessageAr ?? "تم التفعيل بنجاح! 🎉", body.BundleName);
            }

            return new ActivationResult(false, body?.MessageAr ?? "تعذر تفعيل الكود. تأكد من كتابته بشكل صحيح.");
        }
        catch (HttpRequestException)
        {
            return new ActivationResult(false, "تأكد من اتصالك بالإنترنت لتفعيل الكود لأول مرة.");
        }
        catch (TaskCanceledException)
        {
            return new ActivationResult(false, "انتهت مهلة الاتصال بالسيرفر، حاول مرة أخرى.");
        }
        catch
        {
            return new ActivationResult(false, "حدث خطأ غير متوقع أثناء التفعيل.");
        }
    }

    public void DeactivateLocal()
    {
        Preferences.Default.Remove(KeyActivated);
        Preferences.Default.Remove(KeyCode);
        Preferences.Default.Remove(KeyBundle);
    }
}
