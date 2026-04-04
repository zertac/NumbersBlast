using System.Collections.Generic;

public class LineClearResolver
{
    public LineClearResult Resolve(BoardModel model, BoardView boardView)
    {
        var rowsToClear = new List<int>();
        var columnsToClear = new List<int>();

        for (int r = 0; r < model.Rows; r++)
        {
            if (IsRowFull(model, r))
                rowsToClear.Add(r);
        }

        for (int c = 0; c < model.Columns; c++)
        {
            if (IsColumnFull(model, c))
                columnsToClear.Add(c);
        }

        var result = new LineClearResult
        {
            Score = 0,
            ClearedPositions = new List<UnityEngine.Vector2Int>()
        };

        if (rowsToClear.Count == 0 && columnsToClear.Count == 0)
            return result;

        var clearedCells = new HashSet<long>();

        for (int i = 0; i < rowsToClear.Count; i++)
        {
            int r = rowsToClear[i];
            for (int c = 0; c < model.Columns; c++)
            {
                long key = (long)r * model.Columns + c;
                if (clearedCells.Add(key))
                {
                    result.Score += model.GetCell(r, c).Value;
                    result.ClearedPositions.Add(new UnityEngine.Vector2Int(r, c));
                }
            }
        }

        for (int i = 0; i < columnsToClear.Count; i++)
        {
            int c = columnsToClear[i];
            for (int r = 0; r < model.Rows; r++)
            {
                long key = (long)r * model.Columns + c;
                if (clearedCells.Add(key))
                {
                    result.Score += model.GetCell(r, c).Value;
                    result.ClearedPositions.Add(new UnityEngine.Vector2Int(r, c));
                }
            }
        }

        foreach (long key in clearedCells)
        {
            int r = (int)(key / model.Columns);
            int c = (int)(key % model.Columns);
            model.GetCell(r, c).Clear();
        }

        return result;
    }

    private bool IsRowFull(BoardModel model, int row)
    {
        for (int c = 0; c < model.Columns; c++)
        {
            if (model.IsCellEmpty(row, c)) return false;
        }
        return true;
    }

    private bool IsColumnFull(BoardModel model, int column)
    {
        for (int r = 0; r < model.Rows; r++)
        {
            if (model.IsCellEmpty(r, column)) return false;
        }
        return true;
    }
}
