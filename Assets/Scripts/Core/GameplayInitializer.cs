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
    private readonly GameplayHUD _gameplayHUD;
    private readonly MultiplayerManager _multiplayerManager;
    private readonly OpponentVisualPlayer _opponentVisualPlayer;
    private readonly MultiplayerConfig _multiplayerConfig;
    private readonly MultiplayerHUD _multiplayerHUD;

    [Inject]
    public GameplayInitializer(BoardManager boardManager, PieceTray pieceTray, BoardConfig config,
        BoardView boardView, PlacementHandler placementHandler, ScoreUI scoreUI, UIManager uiManager,
        TutorialManager tutorialManager, FeedbackManager feedbackManager, GameStateManager gameStateManager,
        AudioManager audioManager, GameplayHUD gameplayHUD, MultiplayerManager multiplayerManager,
        OpponentVisualPlayer opponentVisualPlayer, MultiplayerConfig multiplayerConfig, MultiplayerHUD multiplayerHUD)
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
        _gameplayHUD = gameplayHUD;
        _multiplayerManager = multiplayerManager;
        _opponentVisualPlayer = opponentVisualPlayer;
        _multiplayerConfig = multiplayerConfig;
        _multiplayerHUD = multiplayerHUD;
    }

    public void Start()
    {
        GameEvents.ClearAll();
        DG.Tweening.DOTween.KillAll();
        _boardManager.Initialize();

        var canvas = _boardView.GetComponentInParent<Canvas>();
        float cellSize = _boardManager.GetCellSize();

        _pieceTray.Initialize(_config, cellSize, canvas, _boardView, _boardManager, _tutorialManager, _feedbackManager, _gameStateManager);
        _feedbackManager.Initialize(_boardView.GetComponent<RectTransform>());
        _gameplayHUD.Initialize(_uiManager, _gameStateManager);
        _placementHandler.Enable();

        GameEvents.OnGameOver += HandleGameOver;
        GameEvents.OnPiecePickedUp += _ => _audioManager.PlayPiecePickup();
        GameEvents.OnPiecePlaced += HandlePiecePlaced;
        GameEvents.OnPieceReleased += _ => _audioManager.PlayPieceReturn();
        GameEvents.OnScoreChanged += _ => _audioManager.PlayScoreUp();
        GameEvents.OnTrayRefilled += () => _audioManager.PlayNewPiecesSpawn();
        GameEvents.OnPopupOpened += HandlePopupOpened;
        GameEvents.OnPopupClosed += () => _audioManager.PlayPopupClose();

        _audioManager.StopMusic();
        _audioManager.PlayGameplayMusic();
        _audioManager.PlayGameStart();

        bool isMultiplayer = GameModeHolder.CurrentMode == GameMode.Multiplayer;

        if (isMultiplayer)
        {
            StartMultiplayer();
        }
        else
        {
            StartSinglePlayer();
        }
    }

    private void StartSinglePlayer()
    {
        _scoreUI.Initialize();
        _multiplayerHUD?.Hide();

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

    private void StartMultiplayer()
    {
        _scoreUI.gameObject.SetActive(false);
        _gameStateManager.Initialize(GameState.Processing);

        _opponentVisualPlayer.Initialize(_multiplayerConfig, _boardView, _pieceTray, _boardManager,
            new OpponentAI(_multiplayerConfig), _config);

        // Show search popup
        var searchPopup = _uiManager.ShowPopup<OpponentSearchPopup>();
        searchPopup?.StartSearch(_multiplayerConfig,
            onFound: () =>
            {
                string opponentName = searchPopup.FoundOpponentName;
                _pieceTray.SpawnPieces();
                _multiplayerManager.StartMultiplayer(opponentName);
            },
            onCancel: () =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
            });
    }

    private void HandlePiecePlaced(PieceView piece, Vector2Int pos)
    {
        _audioManager.PlayPiecePlace();
    }

    private void HandlePopupOpened()
    {
        _audioManager.PlayPopupOpen();
        var settingsPopup = _uiManager.GetPopup<SettingsPopup>();
        if (settingsPopup != null)
            settingsPopup.SetAudioManager(_audioManager);
    }

    private void HandleGameOver()
    {
        _audioManager.PlayGameOver();

        if (GameModeHolder.CurrentMode == GameMode.Multiplayer)
        {
            _multiplayerManager.Stop();
            string result = _multiplayerManager.GetWinner();
            var gameOverUI = _uiManager.ShowPopup<GameOverUI>();
            if (gameOverUI != null)
                gameOverUI.SetScore(_multiplayerHUD.PlayerScore);
        }
        else
        {
            var gameOverUI = _uiManager.ShowPopup<GameOverUI>();
            if (gameOverUI != null)
                gameOverUI.SetScore(_scoreUI.GetScore());
        }
    }
}
