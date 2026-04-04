using UnityEngine;
using UnityEngine.UI;

public class BoardView : MonoBehaviour
{
    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private GridLayoutGroup _gridLayout;
    [SerializeField] private RectTransform _boardRect;

    private CellView[,] _cellViews;

    public CellView[,] CellViews => _cellViews;
    public float CellSize { get; private set; }

    public void Initialize(BoardModel model, BoardConfig config)
    {
        ConfigureGrid(config);
        CreateCells(model, config);
    }

    private void ConfigureGrid(BoardConfig config)
    {
        _gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _gridLayout.constraintCount = config.Columns;
        CellSize = CalculateCellSize(config).x;
        _gridLayout.cellSize = new Vector2(CellSize, CellSize);
        _gridLayout.spacing = new Vector2(config.CellSpacing, config.CellSpacing);
        _gridLayout.childAlignment = TextAnchor.MiddleCenter;

        float totalWidth = (_gridLayout.cellSize.x * config.Columns) + (config.CellSpacing * (config.Columns - 1));
        float totalHeight = (_gridLayout.cellSize.y * config.Rows) + (config.CellSpacing * (config.Rows - 1));
        _boardRect.sizeDelta = new Vector2(totalWidth, totalHeight);
    }

    private Vector2 CalculateCellSize(BoardConfig config)
    {
        float availableWidth = _boardRect.rect.width;

        if (availableWidth <= 0)
            availableWidth = 832f;

        float cellSize = (availableWidth - (config.CellSpacing * (config.Columns - 1))) / config.Columns;
        return new Vector2(cellSize, cellSize);
    }

    private void CreateCells(BoardModel model, BoardConfig config)
    {
        _cellViews = new CellView[model.Rows, model.Columns];

        for (int r = 0; r < model.Rows; r++)
        {
            for (int c = 0; c < model.Columns; c++)
            {
                var cellGo = Instantiate(_cellPrefab, _gridLayout.transform);
                cellGo.name = $"Cell_{r}_{c}";

                var cellView = cellGo.GetComponent<CellView>();
                cellView.Initialize(model.GetCell(r, c), config);
                _cellViews[r, c] = cellView;
            }
        }
    }

    public CellView GetCellView(int row, int column)
    {
        if (row < 0 || row >= _cellViews.GetLength(0)) return null;
        if (column < 0 || column >= _cellViews.GetLength(1)) return null;
        return _cellViews[row, column];
    }

    public void RefreshAll()
    {
        for (int r = 0; r < _cellViews.GetLength(0); r++)
        {
            for (int c = 0; c < _cellViews.GetLength(1); c++)
            {
                _cellViews[r, c].Refresh();
            }
        }
    }
}
