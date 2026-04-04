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

    [Inject]
    public GameplayInitializer(BoardManager boardManager, PieceTray pieceTray, BoardConfig config, BoardView boardView, PlacementHandler placementHandler)
    {
        _boardManager = boardManager;
        _pieceTray = pieceTray;
        _config = config;
        _boardView = boardView;
        _placementHandler = placementHandler;
    }

    public void Start()
    {
        _boardManager.Initialize();

        var canvas = _boardView.GetComponentInParent<Canvas>();
        float cellSize = _boardManager.GetCellSize();
        _pieceTray.Initialize(_config, cellSize, canvas, _boardView, _boardManager);
        _pieceTray.SpawnPieces();

        _placementHandler.Enable();
    }
}
