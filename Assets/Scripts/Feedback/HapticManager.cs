using UnityEngine;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

public static class HapticManager
{
    private static bool _enabled = true;
    private const string HapticEnabledKey = "HapticEnabled";

    public static bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            PlayerPrefs.SetInt(HapticEnabledKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    static HapticManager()
    {
        _enabled = PlayerPrefs.GetInt(HapticEnabledKey, 1) == 1;
    }

    public static void Light()
    {
        if (!_enabled) return;
#if UNITY_ANDROID && !UNITY_EDITOR
        VibrateAndroid(20);
#elif UNITY_IOS && !UNITY_EDITOR
        VibrateIOS(1519);
#endif
    }

    public static void Medium()
    {
        if (!_enabled) return;
#if UNITY_ANDROID && !UNITY_EDITOR
        VibrateAndroid(40);
#elif UNITY_IOS && !UNITY_EDITOR
        VibrateIOS(1520);
#endif
    }

    public static void Heavy()
    {
        if (!_enabled) return;
#if UNITY_ANDROID && !UNITY_EDITOR
        VibrateAndroid(80);
#elif UNITY_IOS && !UNITY_EDITOR
        VibrateIOS(1521);
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaObject _vibrator;

    private static void VibrateAndroid(long milliseconds)
    {
        try
        {
            if (_vibrator == null)
            {
                using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                _vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
            }
            _vibrator.Call("vibrate", milliseconds);
        }
        catch (System.Exception) { }
    }
#endif

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void AudioServicesPlaySystemSound(int soundId);

    private static void VibrateIOS(int soundId)
    {
        try
        {
            AudioServicesPlaySystemSound(soundId);
        }
        catch (System.Exception) { }
    }
#endif
}
