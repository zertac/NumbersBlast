namespace NumbersBlast.Core
{
    /// <summary>
    /// Global constants for scene names, PlayerPrefs keys, and gameplay configuration values.
    /// </summary>
    public static class GameConstants
    {
        // Scene Names
        public const string BootScene = "BootScene";
        public const string MainMenuScene = "MainMenuScene";
        public const string GameScene = "GameScene";

        // PlayerPrefs Keys
        public const string TutorialCompleteKey = "TutorialComplete";
        public const string MusicEnabledKey = "MusicEnabled";
        public const string SFXEnabledKey = "SFXEnabled";
        public const string HapticEnabledKey = "HapticEnabled";

        // Gameplay
        public const float MaxPieceTrayScale = 0.6f;

        public static readonly UnityEngine.Vector2Int[] MergeDirections =
        {
            new(-1, 0), new(1, 0), new(0, -1), new(0, 1)
        };
    }
}
