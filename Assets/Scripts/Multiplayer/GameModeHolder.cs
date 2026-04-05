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
        public static GameModeHolder Instance => _instance ??= new GameModeHolder();

        public GameMode CurrentMode { get; set; } = GameMode.SinglePlayer;

        public GameModeHolder()
        {
            _instance = this;
        }
    }
}
