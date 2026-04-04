using UnityEngine;

public class PieceModel
{
    public PieceShapeData Shape { get; private set; }
    public Vector2Int[] Positions { get; private set; }
    public int[] Values { get; private set; }

    public PieceModel(PieceShapeData shape, int minValue, int maxValue)
    {
        Shape = shape;
        Positions = shape.GetNormalizedPositions();
        Values = new int[Positions.Length];

        for (int i = 0; i < Values.Length; i++)
        {
            Values[i] = Random.Range(minValue, maxValue + 1);
        }
    }

    public int GetValueAt(int index)
    {
        return Values[index];
    }

    public int CellCount => Positions.Length;
}
