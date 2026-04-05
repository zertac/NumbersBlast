using System;
using UnityEngine;

public class TurnManager
{
    private readonly MultiplayerConfig _config;
    private readonly GameStateManager _gameStateManager;

    private float _turnTimer;
    private bool _isActive;
    private bool _isPlayerTurn;
    private bool _turnEnded;
    private MonoBehaviour _coroutineRunner;

    public bool IsActive => _isActive;
    public bool IsPlayerTurn => _isPlayerTurn;
    public float TurnTimeRemaining => _turnTimer;
    public float TurnTimeNormalized => _turnTimer / _config.TurnDuration;

    public event Action OnPlayerTurnStart;
    public event Action OnOpponentTurnStart;
    public event Action<float> OnTimerTick;
    public event Action OnTurnTimeout;

    public TurnManager(MultiplayerConfig config, GameStateManager gameStateManager)
    {
        _config = config;
        _gameStateManager = gameStateManager;
    }

    public void Start(MonoBehaviour coroutineRunner)
    {
        _coroutineRunner = coroutineRunner;
        _isActive = true;
        StartPlayerTurn();
    }

    public void Stop()
    {
        _isActive = false;
    }

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

    public float GetPenaltyAmount(int currentScore)
    {
        return currentScore * _config.PenaltyPercent;
    }
}
