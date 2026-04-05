using System;
using UnityEngine;

public class GameStateManager
{
    private GameState _currentState;
    private GameState _stateBeforePause;

    public GameState CurrentState => _currentState;

    public event Action<GameState, GameState> OnStateChanged;

    public void Initialize(GameState initialState)
    {
        _currentState = initialState;
    }

    public bool CanTransitionTo(GameState newState)
    {
        if (_currentState == newState) return true;

        return (_currentState, newState) switch
        {
            (GameState.Idle, GameState.Dragging) => true,
            (GameState.Idle, GameState.Processing) => true,
            (GameState.Idle, GameState.Paused) => true,
            (GameState.Idle, GameState.GameOver) => true,
            (GameState.Dragging, GameState.Idle) => true,
            (GameState.Dragging, GameState.Processing) => true,
            (GameState.Processing, GameState.Idle) => true,
            (GameState.Processing, GameState.GameOver) => true,
            (GameState.Processing, GameState.Paused) => true,
            (GameState.Paused, GameState.Idle) => true,
            (GameState.Paused, GameState.Tutorial) => true,
            (GameState.Tutorial, GameState.Dragging) => true,
            (GameState.Tutorial, GameState.Processing) => true,
            (GameState.Tutorial, GameState.Idle) => true,
            (GameState.Tutorial, GameState.Paused) => true,
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

    public void Pause()
    {
        if (_currentState == GameState.Paused || _currentState == GameState.GameOver) return;
        _stateBeforePause = _currentState;
        TransitionTo(GameState.Paused);
    }

    public void Resume()
    {
        if (_currentState != GameState.Paused) return;
        _currentState = _stateBeforePause;
        OnStateChanged?.Invoke(GameState.Paused, _currentState);
    }

    public bool IsInputAllowed => _currentState == GameState.Idle || _currentState == GameState.Dragging || _currentState == GameState.Tutorial;
}
