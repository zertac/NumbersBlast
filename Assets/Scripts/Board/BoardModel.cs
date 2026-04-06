namespace NumbersBlast.Board
{
    /// <summary>
    /// Pure data model representing the board grid and its cell states.
    /// </summary>
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

        /// <summary>
        /// Returns true if the given row and column are within the board boundaries.
        /// </summary>
        public bool IsInBounds(int row, int column)
        {
            return row >= 0 && row < Rows && column >= 0 && column < Columns;
        }

        /// <summary>
        /// Returns the CellData at the specified position, or null if out of bounds.
        /// </summary>
        public CellData GetCell(int row, int column)
        {
            if (!IsInBounds(row, column)) return null;
            return Cells[row, column];
        }

        /// <summary>
        /// Returns true if the cell at the given position is empty, or false if occupied or out of bounds.
        /// </summary>
        public bool IsCellEmpty(int row, int column)
        {
            if (!IsInBounds(row, column)) return false;
            return Cells[row, column].IsEmpty;
        }

        /// <summary>
        /// Returns true if every cell in the specified row is occupied.
        /// </summary>
        public bool IsRowFull(int row)
        {
            for (int c = 0; c < Columns; c++)
                if (IsCellEmpty(row, c)) return false;
            return true;
        }

        /// <summary>
        /// Returns true if every cell in the specified column is occupied.
        /// </summary>
        public bool IsColumnFull(int column)
        {
            for (int r = 0; r < Rows; r++)
                if (IsCellEmpty(r, column)) return false;
            return true;
        }

        /// <summary>
        /// Checks whether a piece with the given shape can be placed at the specified board position without overlapping.
        /// </summary>
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
