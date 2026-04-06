using UnityEngine;
using NumbersBlast.Core;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace NumbersBlast.Feedback
{
    /// <summary>
    /// Provides cross-platform haptic feedback for Android and iOS devices with persistent enable/disable preference.
    /// </summary>
    public static class HapticManager
    {
        private static bool _enabled = true;

        /// <summary>
        /// Gets or sets whether haptic feedback is enabled, persisting the preference via PlayerPrefs.
        /// </summary>
        public static bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                PlayerPrefs.SetInt(GameConstants.HapticEnabledKey, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        static HapticManager()
        {
            _enabled = PlayerPrefs.GetInt(GameConstants.HapticEnabledKey, 1) == 1;
        }

        /// <summary>
        /// Triggers a light haptic vibration.
        /// </summary>
        public static void Light()
        {
            if (!_enabled) return;
#if UNITY_ANDROID && !UNITY_EDITOR
            VibrateAndroid(20);
#elif UNITY_IOS && !UNITY_EDITOR
            VibrateIOS(1519);
#endif
        }

        /// <summary>
        /// Triggers a medium haptic vibration.
        /// </summary>
        public static void Medium()
        {
            if (!_enabled) return;
#if UNITY_ANDROID && !UNITY_EDITOR
            VibrateAndroid(40);
#elif UNITY_IOS && !UNITY_EDITOR
            VibrateIOS(1520);
#endif
        }

        /// <summary>
        /// Triggers a heavy haptic vibration.
        /// </summary>
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
            catch (System.Exception e)
            {
#if DEBUG || UNITY_EDITOR
                Debug.LogWarning($"[Haptic] {e.Message}");
#endif
            }
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
            catch (System.Exception e)
            {
#if DEBUG || UNITY_EDITOR
                Debug.LogWarning($"[Haptic] {e.Message}");
#endif
            }
        }
#endif
    }
}
