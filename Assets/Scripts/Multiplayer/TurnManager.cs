using System;
using UnityEngine;
using NumbersBlast.Data;
using NumbersBlast.StateMachine;

namespace NumbersBlast.Multiplayer
{
    /// <summary>
    /// Manages turn-based flow in multiplayer, including turn timers, transitions, and timeout penalties.
    /// </summary>
    public class TurnManager
    {
        private readonly MultiplayerConfig _config;
        private readonly GameStateManager _gameStateManager;

        private float _turnTimer;
        private bool _isActive;
        private bool _isPlayerTurn;
        private bool _turnEnded;

        public bool IsActive => _isActive;
        public bool IsPlayerTurn => _isPlayerTurn;
        public float TurnTimeRemaining => _turnTimer;
        public float TurnTimeNormalized => _turnTimer / _config.TurnDuration;

        /// <summary>Raised when the player's turn begins.</summary>
        public event Action OnPlayerTurnStart;
        /// <summary>Raised when the opponent's turn begins.</summary>
        public event Action OnOpponentTurnStart;
        /// <summary>Raised each frame with the normalized remaining turn time.</summary>
        public event Action<float> OnTimerTick;
        /// <summary>Raised when the current turn expires without a move.</summary>
        public event Action OnTurnTimeout;

        public TurnManager(MultiplayerConfig config, GameStateManager gameStateManager)
        {
            _config = config;
            _gameStateManager = gameStateManager;
        }

        /// <summary>
        /// Activates turn management and randomly assigns the first turn.
        /// </summary>
        public void Start()
        {
            _isActive = true;

            if (UnityEngine.Random.value > 0.5f)
                StartPlayerTurn();
            else
                StartOpponentTurn();
        }

        /// <summary>
        /// Deactivates turn management.
        /// </summary>
        public void Stop()
        {
            _isActive = false;
        }

        /// <summary>
        /// Advances the turn timer by the given delta time and triggers timeout if expired.
        /// </summary>
        public void Tick(float deltaTime)
        {
            if (!_isActive) return;
            // Timer runs even during pause for realism
            if (_gameStateManager.CurrentState == GameState.GameOver) return;

            if (_turnTimer <= 0) return;

            _turnTimer -= deltaTime;
            OnTimerTick?.Invoke(TurnTimeNormalized);

            if (_turnTimer <= 0)
            {
                HandleTimeout();
            }
        }

        /// <summary>
        /// Begins the player's turn, resets the timer, and transitions the game state to Idle.
        /// </summary>
        public void StartPlayerTurn()
        {
            if (!_isActive) return;
            if (_gameStateManager.CurrentState == GameState.GameOver) return;

            _isPlayerTurn = true;
            _turnTimer = _config.TurnDuration;
            _turnEnded = false;

            if (_gameStateManager.CurrentState != GameState.Idle)
                _gameStateManager.TransitionTo(GameState.Idle);

            OnPlayerTurnStart?.Invoke();
        }

        /// <summary>
        /// Begins the opponent's turn, resets the timer, and transitions the game state to Processing.
        /// </summary>
        public void StartOpponentTurn()
        {
            if (!_isActive) return;
            if (_gameStateManager.CurrentState == GameState.GameOver) return;

            _isPlayerTurn = false;
            _turnTimer = _config.TurnDuration;
            _turnEnded = false;
            _gameStateManager.TransitionTo(GameState.Processing);
            OnOpponentTurnStart?.Invoke();
        }

        /// <summary>
        /// Ends the current turn and switches to the other player's turn.
        /// </summary>
        public void EndCurrentTurn()
        {
            if (!_isActive) return;
            if (_turnEnded) return;
            if (_gameStateManager.CurrentState == GameState.GameOver) return;

            _turnEnded = true;

            if (_isPlayerTurn)
                StartOpponentTurn();
            else
                StartPlayerTurn();
        }

        private void HandleTimeout()
        {
            _turnTimer = 0f;
            OnTurnTimeout?.Invoke();
            EndCurrentTurn();
        }

        /// <summary>
        /// Calculates the score penalty for a turn timeout based on the configured penalty percentage.
        /// </summary>
        public int GetPenaltyAmount(int currentScore)
        {
            if (currentScore <= 0) return 0;
            return Mathf.Max(1, Mathf.CeilToInt(currentScore * _config.PenaltyPercent));
        }
    }
}
