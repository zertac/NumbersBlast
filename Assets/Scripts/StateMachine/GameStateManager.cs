using System;
using UnityEngine;

namespace NumbersBlast.StateMachine
{
    /// <summary>
    /// Controls game state transitions with validation, and provides pause/resume functionality.
    /// </summary>
    public class GameStateManager
    {
        private GameState _currentState;
        private GameState _stateBeforePause;

        public GameState CurrentState => _currentState;

        /// <summary>
        /// Raised when the game state changes, providing the previous and new state.
        /// </summary>
        public event Action<GameState, GameState> OnStateChanged;

        /// <summary>
        /// Sets the initial game state without firing any transition events.
        /// </summary>
        public void Initialize(GameState initialState)
        {
            _currentState = initialState;
        }

        /// <summary>
        /// Returns whether transitioning from the current state to the given state is valid.
        /// </summary>
        public bool CanTransitionTo(GameState newState)
        {
            if (_currentState == newState) return false;

            return (_currentState, newState) switch
            {
                // From Idle
                (GameState.Idle, GameState.Dragging) => true,
                (GameState.Idle, GameState.Processing) => true,
                (GameState.Idle, GameState.Paused) => true,
                (GameState.Idle, GameState.GameOver) => true,
                (GameState.Idle, GameState.Tutorial) => true,

                // From Dragging
                // Note: Dragging->GameOver is intentionally absent. By design it goes
                // through Dragging->Processing->GameOver so placement logic completes first.
                (GameState.Dragging, GameState.Idle) => true,
                (GameState.Dragging, GameState.Processing) => true,
                (GameState.Dragging, GameState.Paused) => true,
                (GameState.Dragging, GameState.Tutorial) => true,

                // From Processing
                (GameState.Processing, GameState.Idle) => true,
                (GameState.Processing, GameState.GameOver) => true,
                (GameState.Processing, GameState.Paused) => true,

                // From Tutorial
                (GameState.Tutorial, GameState.Dragging) => true,
                (GameState.Tutorial, GameState.Processing) => true,
                (GameState.Tutorial, GameState.Idle) => true,
                (GameState.Tutorial, GameState.Paused) => true,

                // From Paused - handled by Resume()
                // From GameOver - terminal state, no transitions out

                _ => false
            };
        }

        /// <summary>
        /// Attempts to transition to the given state, logging a warning if the transition is invalid.
        /// </summary>
        public bool TransitionTo(GameState newState)
        {
            if (_currentState == newState) return true;

            if (!CanTransitionTo(newState))
            {
#if UNITY_EDITOR || DEBUG
                Debug.LogWarning($"[GameState] Invalid transition: {_currentState} -> {newState}");
#endif
                return false;
            }

            var previous = _currentState;
            _currentState = newState;
            OnStateChanged?.Invoke(previous, newState);
            return true;
        }

        /// <summary>
        /// Pauses the game by saving the current state and transitioning to Paused.
        /// </summary>
        public void Pause()
        {
            if (_currentState == GameState.Paused || _currentState == GameState.GameOver) return;
            _stateBeforePause = _currentState;
            TransitionTo(GameState.Paused);
        }

        /// <summary>
        /// Resumes the game by restoring the state that was active before pausing.
        /// </summary>
        public void Resume()
        {
            if (_currentState != GameState.Paused) return;

            // Prevent restoring to GameOver - if game ended while paused, stay in GameOver
            if (_stateBeforePause == GameState.GameOver)
            {
                var prev = _currentState;
                _currentState = GameState.GameOver;
                OnStateChanged?.Invoke(prev, _currentState);
                return;
            }

            var previous = _currentState;
            _currentState = _stateBeforePause;
            OnStateChanged?.Invoke(previous, _currentState);
        }

        /// <summary>
        /// True when the current state permits player input (Idle, Dragging, or Tutorial).
        /// </summary>
        public bool IsInputAllowed => _currentState == GameState.Idle || _currentState == GameState.Dragging || _currentState == GameState.Tutorial;
    }
}
