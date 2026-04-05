using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameplayInitializer : IStartable
{
    private readonly BoardManager _boardManager;
    private readonly PieceTray _pieceTray;
    private readonly BoardConfig _config;
    private readonly BoardView _boardView;
    private readonly PlacementHandler _placementHandler;
    private readonly ScoreUI _scoreUI;
    private readonly UIManager _uiManager;
    private readonly TutorialManager _tutorialManager;
    private readonly FeedbackManager _feedbackManager;
    private readonly GameStateManager _gameStateManager;
    private readonly AudioManager _audioManager;

    [Inject]
    public GameplayInitializer(BoardManager boardManager, PieceTray pieceTray, BoardConfig config,
        BoardView boardView, PlacementHandler placementHandler, ScoreUI scoreUI, UIManager uiManager,
        TutorialManager tutorialManager, FeedbackManager feedbackManager, GameStateManager gameStateManager,
        AudioManager audioManager)
    {
        _boardManager = boardManager;
        _pieceTray = pieceTray;
        _config = config;
        _boardView = boardView;
        _placementHandler = placementHandler;
        _scoreUI = scoreUI;
        _uiManager = uiManager;
        _tutorialManager = tutorialManager;
        _feedbackManager = feedbackManager;
        _gameStateManager = gameStateManager;
        _audioManager = audioManager;
    }

    public void Start()
    {
        GameEvents.ClearAll();
        _boardManager.Initialize();

        var canvas = _boardView.GetComponentInParent<Canvas>();
        float cellSize = _boardManager.GetCellSize();

        _pieceTray.Initialize(_config, cellSize, canvas, _boardView, _boardManager, _tutorialManager, _feedbackManager, _gameStateManager);
        _feedbackManager.Initialize(_boardView.GetComponent<RectTransform>());
        _scoreUI.Initialize();
        _placementHandler.Enable();

        GameEvents.OnGameOver += HandleGameOver;
        GameEvents.OnPiecePickedUp += _ => _audioManager.PlayPiecePickup();
        GameEvents.OnPiecePlaced += (_, __) => _audioManager.PlayPiecePlace();
        GameEvents.OnPieceReleased += _ => _audioManager.PlayPieceReturn();
        GameEvents.OnScoreChanged += _ => _audioManager.PlayScoreUp();
        GameEvents.OnTrayRefilled += () => _audioManager.PlayNewPiecesSpawn();
        GameEvents.OnPopupOpened += () => _audioManager.PlayPopupOpen();
        GameEvents.OnPopupClosed += () => _audioManager.PlayPopupClose();

        _audioManager.PlayGameplayMusic();
        _audioManager.PlayGameStart();

        if (_tutorialManager.ShouldRunTutorial())
        {
            _gameStateManager.Initialize(GameState.Tutorial);
            _tutorialManager.StartTutorial();
        }
        else
        {
            _gameStateManager.Initialize(GameState.Idle);
            _pieceTray.SpawnPieces();
        }
    }

    private void HandleGameOver()
    {
        _audioManager.PlayGameOver();
        _uiManager.ShowPopup(PopupType.GameOver);
        var gameOverUI = _uiManager.GetPopup<GameOverUI>(PopupType.GameOver);
        if (gameOverUI != null)
            gameOverUI.SetScore(_scoreUI.GetScore());
    }
}
