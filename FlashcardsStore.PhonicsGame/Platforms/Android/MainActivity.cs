using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Speech;
using AndroidX.Core.View;

namespace FlashcardsStore.PhonicsGame;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                           ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    private const int SpeechRequestCode = 9412;
    private static TaskCompletionSource<IReadOnlyList<string>>? _speechTcs;
    private static WeakReference<MainActivity>? _currentActivity;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        _currentActivity = new WeakReference<MainActivity>(this);

        if (Window is not null)
        {
            WindowCompat.SetDecorFitsSystemWindows(Window, false);
            var controller = WindowCompat.GetInsetsController(Window, Window.DecorView);
            if (controller is not null)
            {
                controller.AppearanceLightStatusBars = false;
            }
        }
    }

    public static Task<IReadOnlyList<string>> RecognizeSpeechAsync(string prompt)
    {
        if (_currentActivity is null || !_currentActivity.TryGetTarget(out var activity))
            return Task.FromResult<IReadOnlyList<string>>([]);

        _speechTcs?.TrySetResult([]);
        _speechTcs = new TaskCompletionSource<IReadOnlyList<string>>();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            try
            {
                var intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                intent.PutExtra(RecognizerIntent.ExtraLanguage, "en-US");
                intent.PutExtra(RecognizerIntent.ExtraMaxResults, 5);
                intent.PutExtra(RecognizerIntent.ExtraPrompt, prompt);
                activity.StartActivityForResult(intent, SpeechRequestCode);
            }
            catch
            {
                _speechTcs?.TrySetResult([]);
            }
        });

        return _speechTcs.Task;
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);

        if (requestCode == SpeechRequestCode && _speechTcs is not null)
        {
            if (resultCode == Result.Ok && data is not null)
            {
                var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                _speechTcs.TrySetResult(matches?.ToList() ?? []);
            }
            else
            {
                _speechTcs.TrySetResult([]);
            }
        }
    }
}
