using UnityEngine;

public class PlacementHandler
{
    private readonly BoardManager _boardManager;
    private readonly MergeResolver _mergeResolver;
    private readonly LineClearResolver _lineClearResolver;
    private readonly PieceTray _pieceTray;
    private readonly BoardView _boardView;

    public PlacementHandler(BoardManager boardManager, MergeResolver mergeResolver, LineClearResolver lineClearResolver, PieceTray pieceTray, BoardView boardView)
    {
        _boardManager = boardManager;
        _mergeResolver = mergeResolver;
        _lineClearResolver = lineClearResolver;
        _pieceTray = pieceTray;
        _boardView = boardView;
    }

    public void Enable()
    {
        GameEvents.OnPiecePlaced += HandlePiecePlaced;
    }

    public void Disable()
    {
        GameEvents.OnPiecePlaced -= HandlePiecePlaced;
    }

    private void HandlePiecePlaced(PieceView pieceView, Vector2Int boardPos)
    {
        var model = _boardManager.Model;
        var pieceModel = pieceView.Model;

        PlaceCells(model, pieceModel, boardPos);
        Object.Destroy(pieceView.gameObject);
        _pieceTray.RemovePiece(pieceView);

        _boardView.RefreshAll();

        _mergeResolver.Resolve(model, pieceModel, boardPos, _boardView);

        int clearedScore = _lineClearResolver.Resolve(model, _boardView);
        if (clearedScore > 0)
        {
            GameEvents.ScoreChanged(clearedScore);
        }

        _boardView.RefreshAll();

        if (!HasValidMove())
        {
            GameEvents.GameOver();
        }
    }

    private void PlaceCells(BoardModel model, PieceModel pieceModel, Vector2Int boardPos)
    {
        for (int i = 0; i < pieceModel.CellCount; i++)
        {
            int row = boardPos.x + pieceModel.Positions[i].x;
            int col = boardPos.y + pieceModel.Positions[i].y;
            model.GetCell(row, col).SetValue(pieceModel.GetValueAt(i));
        }
    }

    private bool HasValidMove()
    {
        var model = _boardManager.Model;
        var spawnConfig = _boardManager.Config.PieceSpawnConfig;

        // Check current tray pieces first
        // Then check if any shape from config can fit
        for (int s = 0; s < spawnConfig.Shapes.Length; s++)
        {
            var positions = spawnConfig.Shapes[s].GetNormalizedPositions();

            for (int r = 0; r < model.Rows; r++)
            {
                for (int c = 0; c < model.Columns; c++)
                {
                    if (CanFitAt(model, positions, r, c))
                        return true;
                }
            }
        }

        return false;
    }

    private bool CanFitAt(BoardModel model, Vector2Int[] positions, int startRow, int startCol)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            int row = startRow + positions[i].x;
            int col = startCol + positions[i].y;

            if (!model.IsInBounds(row, col)) return false;
            if (!model.IsCellEmpty(row, col)) return false;
        }
        return true;
    }
}
