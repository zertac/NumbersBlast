using UnityEngine;
using VContainer;
using VContainer.Unity;
using NumbersBlast.Audio;
using NumbersBlast.Board;
using NumbersBlast.Data;
using NumbersBlast.Feedback;
using NumbersBlast.Gameplay;
using NumbersBlast.Multiplayer;
using NumbersBlast.Piece;
using NumbersBlast.StateMachine;
using NumbersBlast.Tutorial;
using NumbersBlast.UI;

namespace NumbersBlast.Core
{
    /// <summary>
    /// Bootstraps the gameplay scene by initializing all core systems, wiring up event listeners, and starting the appropriate game mode.
    /// </summary>
    public class GameplayInitializer : IStartable
    {
        // Core - constructor inject
        private readonly BoardManager _boardManager;
        private readonly BoardConfig _config;
        private readonly BoardView _boardView;
        private readonly PlacementHandler _placementHandler;
        private readonly GameStateManager _gameStateManager;
        private readonly IUIManager _uiManager;
        private readonly AudioManager _audioManager;
        private readonly IFeedbackManager _feedbackManager;

        // UI - property inject
        [Inject] private PieceTray _pieceTray;
        [Inject] private ScoreUI _scoreUI;
        [Inject] private GameplayHUD _gameplayHUD;

        // Tutorial - property inject
        [Inject] private TutorialManager _tutorialManager;

        // Multiplayer - property inject
        [Inject] private MultiplayerManager _multiplayerManager;
        [Inject] private OpponentVisualPlayer _opponentVisualPlayer;
        [Inject] private MultiplayerConfig _multiplayerConfig;
        [Inject] private MultiplayerHUD _multiplayerHUD;
        [Inject] private OpponentAI _opponentAI;

        [Inject]
        public GameplayInitializer(BoardManager boardManager, BoardConfig config, BoardView boardView,
            PlacementHandler placementHandler, GameStateManager gameStateManager, IUIManager uiManager,
            AudioManager audioManager, IFeedbackManager feedbackManager)
        {
            _boardManager = boardManager;
            _config = config;
            _boardView = boardView;
            _placementHandler = placementHandler;
            _gameStateManager = gameStateManager;
            _uiManager = uiManager;
            _audioManager = audioManager;
            _feedbackManager = feedbackManager;
        }

        /// <summary>
        /// Initializes the board, UI, audio, and event subscriptions, then starts single-player or multiplayer mode.
        /// </summary>
        public void Start()
        {
            GameEvents.ClearAll();
            DG.Tweening.DOTween.KillAll();
            _boardManager.Initialize();

            var canvas = _boardView.GetComponentInParent<Canvas>();
            float cellSize = _boardManager.GetCellSize();

            bool isMultiplayer = GameModeHolder.Instance.CurrentMode == GameMode.Multiplayer;

            _pieceTray.Initialize(_config, cellSize, canvas, _boardView, _boardManager,
                isMultiplayer ? null : _tutorialManager, _feedbackManager, _gameStateManager);

            _feedbackManager.Initialize(_boardView.GetComponent<RectTransform>());
            _gameplayHUD.Initialize(_uiManager, _gameStateManager);
            _placementHandler.Enable();

            GameEvents.OnGameOver += HandleGameOver;
            GameEvents.OnPiecePickedUp += HandlePiecePickedUp;
            GameEvents.OnPiecePlaced += HandlePiecePlaced;
            GameEvents.OnPieceReleased += HandlePieceReleased;
            GameEvents.OnScoreChanged += HandleScoreChanged;
            GameEvents.OnTrayRefilled += HandleTrayRefilled;
            GameEvents.OnPopupOpened += HandlePopupOpened;
            GameEvents.OnPopupClosed += HandlePopupClosed;

            _audioManager.StopMusic();
            _audioManager.PlayGameplayMusic();
            _audioManager.PlayGameStart();

            Application.runInBackground = isMultiplayer;

            if (isMultiplayer)
                StartMultiplayer();
            else
                StartSinglePlayer();
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
                _opponentAI, _config);

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
                    UnityEngine.SceneManagement.SceneManager.LoadScene(GameConstants.MainMenuScene);
                });
        }

        private void HandlePiecePickedUp(PieceView piece)
        {
            _audioManager.PlayPiecePickup();
        }

        private void HandlePieceReleased(PieceView piece)
        {
            _audioManager.PlayPieceReturn();
        }

        private void HandleScoreChanged(int score)
        {
            _audioManager.PlayScoreUp();
        }

        private void HandleTrayRefilled()
        {
            _audioManager.PlayNewPiecesSpawn();
        }

        private void HandlePopupClosed()
        {
            _audioManager.PlayPopupClose();
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

            if (GameModeHolder.Instance.CurrentMode == GameMode.Multiplayer)
            {
                _multiplayerManager.Stop();
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
}
