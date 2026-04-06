namespace NumbersBlast.StateMachine
{
    /// <summary>
    /// Enumerates the possible states of the game loop.
    /// </summary>
    public enum GameState
    {
        Idle,
        Dragging,
        Processing,
        Tutorial,
        Paused,
        GameOver
    }
}
