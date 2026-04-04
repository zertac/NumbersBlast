using System.Collections.Generic;

public class LineClearResolver
{
    public int Resolve(BoardModel model, BoardView boardView)
    {
        var rowsToClear = new List<int>();
        var columnsToClear = new List<int>();

        // Find all full rows and columns FIRST
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

        if (rowsToClear.Count == 0 && columnsToClear.Count == 0)
            return 0;

        // Track cleared cells to avoid double-scoring intersections
        var clearedCells = new HashSet<long>();
        int totalScore = 0;

        for (int i = 0; i < rowsToClear.Count; i++)
        {
            int r = rowsToClear[i];
            for (int c = 0; c < model.Columns; c++)
            {
                long key = (long)r * model.Columns + c;
                if (clearedCells.Add(key))
                {
                    totalScore += model.GetCell(r, c).Value;
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
                    totalScore += model.GetCell(r, c).Value;
                }
            }
        }

        // Clear all at once
        foreach (long key in clearedCells)
        {
            int r = (int)(key / model.Columns);
            int c = (int)(key % model.Columns);
            model.GetCell(r, c).Clear();
        }

        return totalScore;
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
