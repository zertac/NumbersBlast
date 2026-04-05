using UnityEngine;

namespace NumbersBlast.Piece
{
    [CreateAssetMenu(fileName = "PieceShape", menuName = "NumbersBlast/Piece Shape")]
    public class PieceShapeData : ScriptableObject
    {
        public int Rows = 3;
        public int Columns = 3;

        [HideInInspector]
        public bool[] Cells = new bool[9];

        public void ResizeGrid(int newRows, int newColumns)
        {
            if (newRows == Rows && newColumns == Columns && Cells.Length == newRows * newColumns) return;

            var newCells = new bool[newRows * newColumns];
            int minRows = Mathf.Min(Rows, newRows);
            int minCols = Mathf.Min(Columns, newColumns);

            for (int r = 0; r < minRows; r++)
            {
                for (int c = 0; c < minCols; c++)
                {
                    newCells[r * newColumns + c] = Cells[r * Columns + c];
                }
            }

            Rows = newRows;
            Columns = newColumns;
            Cells = newCells;
        }

        public bool GetCell(int row, int column)
        {
            if (row < 0 || row >= Rows || column < 0 || column >= Columns)
                return false;
            return Cells[row * Columns + column];
        }

        public void SetCell(int row, int column, bool value)
        {
            if (row < 0 || row >= Rows || column < 0 || column >= Columns)
                return;
            Cells[row * Columns + column] = value;
        }

        public int GetActiveCellCount()
        {
            int count = 0;
            for (int i = 0; i < Cells.Length; i++)
            {
                if (Cells[i]) count++;
            }
            return count;
        }

        public Vector2Int[] GetNormalizedPositions()
        {
            int activeCellCount = GetActiveCellCount();
            if (activeCellCount == 0) return new Vector2Int[0];

            var positions = new Vector2Int[activeCellCount];
            int index = 0;
            int minRow = int.MaxValue;
            int minCol = int.MaxValue;

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    if (GetCell(r, c))
                    {
                        if (r < minRow) minRow = r;
                        if (c < minCol) minCol = c;
                        positions[index++] = new Vector2Int(r, c);
                    }
                }
            }

            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = new Vector2Int(positions[i].x - minRow, positions[i].y - minCol);
            }

            return positions;
        }

        public Vector2Int GetNormalizedSize()
        {
            var positions = GetNormalizedPositions();
            if (positions.Length == 0) return Vector2Int.zero;

            int maxRow = 0;
            int maxCol = 0;
            for (int i = 0; i < positions.Length; i++)
            {
                if (positions[i].x > maxRow) maxRow = positions[i].x;
                if (positions[i].y > maxCol) maxCol = positions[i].y;
            }
            return new Vector2Int(maxRow + 1, maxCol + 1);
        }
    }
}
