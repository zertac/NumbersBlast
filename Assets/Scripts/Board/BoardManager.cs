using VContainer;

public class BoardManager
{
    private readonly BoardConfig _config;
    private readonly BoardView _view;

    private BoardModel _model;

    public BoardModel Model => _model;
    public BoardConfig Config => _config;

    [Inject]
    public BoardManager(BoardConfig config, BoardView view)
    {
        _config = config;
        _view = view;
    }

    public void Initialize()
    {
        if (_config == null) { Debug.LogError("[BoardManager] BoardConfig is null!"); return; }
        if (_view == null) { Debug.LogError("[BoardManager] BoardView is null!"); return; }

        _model = new BoardModel(_config.Rows, _config.Columns);
        _view.Initialize(_model, _config);
    }

    public CellView GetCellView(int row, int column)
    {
        return _view.GetCellView(row, column);
    }

    public float GetCellSize()
    {
        return _view.CellSize;
    }

    public void RefreshView()
    {
        _view.RefreshAll();
    }
}
