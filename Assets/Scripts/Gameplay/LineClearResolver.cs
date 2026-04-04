public class LineClearResolver
{
    public int Resolve(BoardModel model, BoardView boardView)
    {
        int totalScore = 0;

        totalScore += ClearFullRows(model);
        totalScore += ClearFullColumns(model);

        return totalScore;
    }

    private int ClearFullRows(BoardModel model)
    {
        int score = 0;

        for (int r = 0; r < model.Rows; r++)
        {
            if (IsRowFull(model, r))
            {
                score += ClearRow(model, r);
            }
        }

        return score;
    }

    private int ClearFullColumns(BoardModel model)
    {
        int score = 0;

        for (int c = 0; c < model.Columns; c++)
        {
            if (IsColumnFull(model, c))
            {
                score += ClearColumn(model, c);
            }
        }

        return score;
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

    private int ClearRow(BoardModel model, int row)
    {
        int score = 0;
        for (int c = 0; c < model.Columns; c++)
        {
            score += model.GetCell(row, c).Value;
            model.GetCell(row, c).Clear();
        }
        return score;
    }

    private int ClearColumn(BoardModel model, int column)
    {
        int score = 0;
        for (int r = 0; r < model.Rows; r++)
        {
            score += model.GetCell(r, column).Value;
            model.GetCell(r, column).Clear();
        }
        return score;
    }
}
