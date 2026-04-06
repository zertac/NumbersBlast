using UnityEngine;
using NumbersBlast.Data;

namespace NumbersBlast.Board
{
    /// <summary>
    /// Coordinates between the BoardModel and BoardView, serving as the main entry point for board operations.
    /// </summary>
    public class BoardManager
    {
        private readonly BoardConfig _config;
        private readonly BoardView _view;

        private BoardModel _model;

        /// <summary>
        /// The data model representing the current board state.
        /// </summary>
        public BoardModel Model => _model;

        /// <summary>
        /// The configuration asset defining board dimensions, spacing, and theme.
        /// </summary>
        public BoardConfig Config => _config;

        public BoardManager(BoardConfig config, BoardView view)
        {
            _config = config;
            _view = view;
        }

        /// <summary>
        /// Creates the board model and initializes the view with the configured dimensions and theme.
        /// </summary>
        public void Initialize()
        {
            if (_config == null) { Debug.LogError("[BoardManager] BoardConfig is null!"); return; }
            if (_view == null) { Debug.LogError("[BoardManager] BoardView is null!"); return; }

            _model = new BoardModel(_config.Rows, _config.Columns);
            _view.Initialize(_model, _config);
        }

        /// <summary>
        /// Returns the CellView at the given position, or null if out of bounds.
        /// </summary>
        public CellView GetCellView(int row, int column)
        {
            return _view.GetCellView(row, column);
        }

        /// <summary>
        /// Returns the computed cell size in pixels used by the board layout.
        /// </summary>
        public float GetCellSize()
        {
            return _view.CellSize;
        }

        /// <summary>
        /// Refreshes the visual state of all board cells to match the current model data.
        /// </summary>
        public void RefreshView()
        {
            _view.RefreshAll();
        }
    }
}
