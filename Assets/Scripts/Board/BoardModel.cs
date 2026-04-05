namespace NumbersBlast.Board
{
    public class BoardModel
    {
        public int Rows { get; private set; }
        public int Columns { get; private set; }
        public CellData[,] Cells { get; private set; }

        public BoardModel(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Cells = new CellData[rows, columns];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    Cells[r, c] = new CellData(r, c);
                }
            }
        }

        public bool IsInBounds(int row, int column)
        {
            return row >= 0 && row < Rows && column >= 0 && column < Columns;
        }

        public CellData GetCell(int row, int column)
        {
            if (!IsInBounds(row, column)) return null;
            return Cells[row, column];
        }

        public bool IsCellEmpty(int row, int column)
        {
            if (!IsInBounds(row, column)) return false;
            return Cells[row, column].IsEmpty;
        }

        public bool CanFitPiece(UnityEngine.Vector2Int[] positions, int startRow, int startCol)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                int row = startRow + positions[i].x;
                int col = startCol + positions[i].y;
                if (!IsInBounds(row, col)) return false;
                if (!IsCellEmpty(row, col)) return false;
            }
            return true;
        }
    }
}
