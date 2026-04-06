using UnityEngine;
using NumbersBlast.Core;

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
            // Intentionally empty — Handheld.Vibrate is too strong for light taps
        }

        /// <summary>
        /// Triggers a medium haptic vibration.
        /// </summary>
        public static void Medium()
        {
            if (!_enabled) return;
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            Handheld.Vibrate();
#endif
        }

        /// <summary>
        /// Triggers a heavy haptic vibration.
        /// </summary>
        public static void Heavy()
        {
            if (!_enabled) return;
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            Handheld.Vibrate();
#endif
        }
    }
}
