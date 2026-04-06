using UnityEngine;
using UnityEngine.UI;
using NumbersBlast.Data;

namespace NumbersBlast.Board
{
    /// <summary>
    /// MonoBehaviour responsible for rendering the board grid, creating cell views, and applying the visual theme.
    /// </summary>
    public class BoardView : MonoBehaviour
    {
        private const float FallbackBoardWidth = 832f;
        [SerializeField] private CellView _cellPrefab;
        [SerializeField] private GridLayoutGroup _gridLayout;
        [SerializeField] private RectTransform _boardRect;

        private CellView[,] _cellViews;

        /// <summary>
        /// Two-dimensional array of all instantiated cell views on the board.
        /// </summary>
        public CellView[,] CellViews => _cellViews;

        /// <summary>
        /// The calculated pixel size of each cell, based on board width and column count.
        /// </summary>
        public float CellSize { get; private set; }

        [SerializeField] private Image _boardFrame;

        /// <summary>
        /// Sets up the board visuals by applying the theme, configuring the grid layout, and instantiating all cell views.
        /// </summary>
        public void Initialize(BoardModel model, BoardConfig config)
        {
            ApplyTheme(config.Theme);
            ConfigureGrid(config);
            CreateCells(model, config);
        }

        private void ApplyTheme(ThemeData theme)
        {
            var camera = Camera.main;
            if (camera != null)
                camera.backgroundColor = theme.BackgroundColor;

            var canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                var bgTransform = canvas.transform.Find("Background");
                if (bgTransform != null)
                {
                    var bgImage = bgTransform.GetComponent<Image>();
                    if (bgImage != null)
                    {
                        if (theme.BackgroundSprite != null)
                        {
                            bgImage.sprite = theme.BackgroundSprite;
                            bgImage.color = Color.white;
                        }
                        else
                        {
                            bgImage.color = theme.BackgroundColor;
                        }
                    }
                }
            }

            if (_boardFrame != null)
            {
                if (theme.BoardFrameSprite != null)
                {
                    _boardFrame.sprite = theme.BoardFrameSprite;
                    _boardFrame.color = theme.BoardFrameColor;
                    _boardFrame.gameObject.SetActive(true);
                }
                else
                {
                    _boardFrame.gameObject.SetActive(false);
                }
            }
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
                availableWidth = FallbackBoardWidth;

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
                    var cellView = Instantiate(_cellPrefab, _gridLayout.transform);
                    cellView.name = $"Cell_{r}_{c}";
                    cellView.Initialize(model.GetCell(r, c), config.Theme);
                    _cellViews[r, c] = cellView;
                }
            }
        }

        /// <summary>
        /// Returns the CellView at the given row and column, or null if out of bounds.
        /// </summary>
        public CellView GetCellView(int row, int column)
        {
            if (row < 0 || row >= _cellViews.GetLength(0)) return null;
            if (column < 0 || column >= _cellViews.GetLength(1)) return null;
            return _cellViews[row, column];
        }

        /// <summary>
        /// Refreshes the visual state of every cell on the board to match current data.
        /// </summary>
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
}
