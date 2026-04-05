using UnityEngine;

public class MultiplayerManager
{
    private readonly MultiplayerConfig _config;
    private readonly TurnManager _turnManager;
    private readonly OpponentAI _opponentAI;
    private readonly OpponentVisualPlayer _visualPlayer;
    private readonly MultiplayerHUD _hud;
    private readonly BoardManager _boardManager;
    private readonly PieceTray _pieceTray;
    private readonly UIManager _uiManager;
    private readonly PlacementHandler _placementHandler;

    private bool _isActive;
    private bool _isScoreForPlayer;

    public bool IsActive => _isActive;
    public bool IsPlayerTurn => _turnManager.IsPlayerTurn;

    public MultiplayerManager(MultiplayerConfig config, TurnManager turnManager, OpponentAI opponentAI,
        OpponentVisualPlayer visualPlayer, MultiplayerHUD hud, BoardManager boardManager,
        PieceTray pieceTray, UIManager uiManager, PlacementHandler placementHandler)
    {
        _config = config;
        _turnManager = turnManager;
        _opponentAI = opponentAI;
        _visualPlayer = visualPlayer;
        _hud = hud;
        _boardManager = boardManager;
        _pieceTray = pieceTray;
        _uiManager = uiManager;
        _placementHandler = placementHandler;
    }

    public void StartMultiplayer(string opponentName)
    {
        _isActive = true;
        _hud.Initialize(opponentName);

        _turnManager.OnPlayerTurnStart += HandlePlayerTurnStart;
        _turnManager.OnOpponentTurnStart += HandleOpponentTurnStart;
        _turnManager.OnTimerTick += HandleTimerTick;
        _turnManager.OnTurnTimeout += HandleTimeout;

        GameEvents.OnScoreChanged += HandleScoreChanged;
        _placementHandler.OnPlacementComplete += HandlePlacementComplete;

        _turnManager.Start(_visualPlayer);
    }

    public void Stop()
    {
        _isActive = false;
        _turnManager.Stop();
        _visualPlayer.CancelTurn();

        _turnManager.OnPlayerTurnStart -= HandlePlayerTurnStart;
        _turnManager.OnOpponentTurnStart -= HandleOpponentTurnStart;
        _turnManager.OnTimerTick -= HandleTimerTick;
        _turnManager.OnTurnTimeout -= HandleTimeout;

        GameEvents.OnScoreChanged -= HandleScoreChanged;
        _placementHandler.OnPlacementComplete -= HandlePlacementComplete;
    }

    public void Tick(float deltaTime)
    {
        if (!_isActive) return;
        _turnManager.Tick(deltaTime);
    }

    private void HandlePlayerTurnStart()
    {
        _isScoreForPlayer = true;
        _hud.SetPlayerTurn();
    }

    private void HandleOpponentTurnStart()
    {
        _isScoreForPlayer = false;
        _isProcessingAITurn = true;
        _hud.SetOpponentTurn();

        _visualPlayer.PerformTurn(() =>
        {
            _isProcessingAITurn = false;
            if (_isActive)
                _turnManager.EndCurrentTurn();
        });
    }

    private void HandleTimerTick(float normalized)
    {
        _hud.UpdateTimer(normalized);
    }

    private void HandleTimeout()
    {
        bool isPlayer = _turnManager.IsPlayerTurn;
        int currentScore = isPlayer ? _hud.PlayerScore : _hud.OpponentScore;
        int penalty = Mathf.RoundToInt(_turnManager.GetPenaltyAmount(currentScore));

        if (penalty > 0)
            _hud.ApplyPenalty(isPlayer, penalty);

        // Cancel AI turn if it was their timeout
        if (!isPlayer)
        {
            _visualPlayer.CancelTurn();
            _isProcessingAITurn = false;
        }
    }

    private void HandleScoreChanged(int points)
    {
        if (_isScoreForPlayer)
            _hud.AddPlayerScore(points);
        else
            _hud.AddOpponentScore(points);
    }

    private bool _isProcessingAITurn;

    private void HandlePlacementComplete()
    {
        if (!_isActive) return;
        if (_isProcessingAITurn) return;

        _turnManager.EndCurrentTurn();
    }

    public string GetWinner()
    {
        if (_hud.PlayerScore > _hud.OpponentScore)
            return "YOU WIN!";
        else if (_hud.OpponentScore > _hud.PlayerScore)
            return "YOU LOSE!";
        else
            return "DRAW!";
    }
}
