namespace NumbersBlast.StateMachine
{
    /// <summary>
    /// Enumerates the possible states of the game loop.
    /// </summary>
    public enum GameState
    {
        /// <summary>No active input or processing; awaiting player action.</summary>
        Idle,
        /// <summary>Player is dragging a piece from the tray.</summary>
        Dragging,
        /// <summary>Board is resolving merges, line clears, or placement effects.</summary>
        Processing,
        /// <summary>Tutorial step is active; only guided input is allowed.</summary>
        Tutorial,
        /// <summary>Game is paused; input is blocked until resumed.</summary>
        Paused,
        /// <summary>Terminal state; no further transitions allowed.</summary>
        GameOver
    }
}
