namespace NumbersBlast.Board
{
    /// <summary>
    /// Represents the data model for a single cell on the board, holding its position and numeric value.
    /// </summary>
    public class CellData
    {
        public int Row { get; private set; }
        public int Column { get; private set; }
        public int Value { get; private set; }
        public bool IsEmpty => Value == 0;

        public CellData(int row, int column)
        {
            Row = row;
            Column = column;
            Value = 0;
        }

        /// <summary>
        /// Sets the numeric value of this cell.
        /// </summary>
        public void SetValue(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Clears this cell by resetting its value to zero.
        /// </summary>
        public void Clear()
        {
            Value = 0;
        }
    }
}
