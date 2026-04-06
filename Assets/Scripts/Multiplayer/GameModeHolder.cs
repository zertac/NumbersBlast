namespace NumbersBlast.Multiplayer
{
    /// <summary>
    /// Holds the current game mode selection between scenes.
    /// Registered in ProjectLifetimeScope for DI access.
    /// Static Instance available for MonoBehaviour access where DI is unavailable.
    /// </summary>
    public class GameModeHolder
    {
        private static GameModeHolder _instance;

        /// <summary>
        /// Gets the singleton instance, creating one if it does not exist.
        /// </summary>
        public static GameModeHolder Instance => _instance ??= new GameModeHolder();

        /// <summary>
        /// Gets or sets the currently selected game mode.
        /// </summary>
        public GameMode CurrentMode { get; set; } = GameMode.SinglePlayer;

        public GameModeHolder()
        {
            _instance = this;
        }
    }
}
