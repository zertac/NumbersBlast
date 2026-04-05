using System;
using UnityEngine;

public class GameStateManager
{
    private GameState _currentState;

    public GameState CurrentState => _currentState;

    public event Action<GameState, GameState> OnStateChanged;

    public void Initialize(GameState initialState)
    {
        _currentState = initialState;
    }

    public bool CanTransitionTo(GameState newState)
    {
        return (_currentState, newState) switch
        {
            (GameState.Idle, GameState.Dragging) => true,
            (GameState.Idle, GameState.GameOver) => true,
            (GameState.Dragging, GameState.Idle) => true,
            (GameState.Dragging, GameState.Processing) => true,
            (GameState.Processing, GameState.Idle) => true,
            (GameState.Processing, GameState.GameOver) => true,
            (GameState.Tutorial, GameState.Dragging) => true,
            (GameState.Tutorial, GameState.Processing) => true,
            (GameState.Tutorial, GameState.Idle) => true,
            (_, GameState.Tutorial) => true,
            _ => false
        };
    }

    public bool TransitionTo(GameState newState)
    {
        if (!CanTransitionTo(newState))
        {
            Debug.LogWarning($"[GameState] Invalid transition: {_currentState} -> {newState}");
            return false;
        }

        var previous = _currentState;
        _currentState = newState;
        OnStateChanged?.Invoke(previous, newState);
        return true;
    }

    public bool IsInputAllowed => _currentState == GameState.Idle || _currentState == GameState.Dragging || _currentState == GameState.Tutorial;
}
