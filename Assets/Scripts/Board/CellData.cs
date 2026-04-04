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

    public void SetValue(int value)
    {
        Value = value;
    }

    public void Clear()
    {
        Value = 0;
    }
}
