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
    private readonly GameOverUI _gameOverUI;
    private readonly TutorialManager _tutorialManager;
    private readonly FeedbackManager _feedbackManager;

    [Inject]
    public GameplayInitializer(BoardManager boardManager, PieceTray pieceTray, BoardConfig config,
        BoardView boardView, PlacementHandler placementHandler, ScoreUI scoreUI, GameOverUI gameOverUI,
        TutorialManager tutorialManager, FeedbackManager feedbackManager)
    {
        _boardManager = boardManager;
        _pieceTray = pieceTray;
        _config = config;
        _boardView = boardView;
        _placementHandler = placementHandler;
        _scoreUI = scoreUI;
        _gameOverUI = gameOverUI;
        _tutorialManager = tutorialManager;
        _feedbackManager = feedbackManager;
    }

    public void Start()
    {
        _boardManager.Initialize();

        var canvas = _boardView.GetComponentInParent<Canvas>();
        var canvasRect = canvas.GetComponent<RectTransform>();
        float cellSize = _boardManager.GetCellSize();

        _pieceTray.Initialize(_config, cellSize, canvas, _boardView, _boardManager, _tutorialManager, _feedbackManager);
        _feedbackManager.Initialize(_boardView.GetComponent<RectTransform>());
        _scoreUI.Initialize();
        _gameOverUI.Initialize();
        _placementHandler.Enable();

        if (_tutorialManager.ShouldRunTutorial())
        {
            _tutorialManager.StartTutorial();
        }
        else
        {
            _pieceTray.SpawnPieces();
        }
    }
}
