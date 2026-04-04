using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BoardConfig", menuName = "NumbersBlast/Board Config")]
public class BoardConfig : ScriptableObject
{
    [Header("Board")]
    public int Rows = 8;
    public int Columns = 8;
    public float CellSpacing = 4f;

    [Header("Piece Spawning")]
    public PieceSpawnConfig PieceSpawnConfig;

    [Header("Block Values")]
    public int MinBlockValue = 1;
    public int MaxBlockValue = 4;

    [Header("Cell Visuals")]
    public Color EmptyCellColor = Color.white;
    public BlockColorEntry[] BlockColors;

    public Color GetBlockColor(int value)
    {
        for (int i = 0; i < BlockColors.Length; i++)
        {
            if (BlockColors[i].Value == value)
                return BlockColors[i].Color;
        }
        return Color.gray;
    }

    public bool ValidatePieceShape(PieceShapeData shape)
    {
        var size = shape.GetNormalizedSize();
        return size.x <= Rows && size.y <= Columns;
    }

    public void OnValidate()
    {
        if (PieceSpawnConfig == null || PieceSpawnConfig.Shapes == null) return;

        for (int i = 0; i < PieceSpawnConfig.Shapes.Length; i++)
        {
            var shape = PieceSpawnConfig.Shapes[i];
            if (shape == null) continue;

            if (!ValidatePieceShape(shape))
            {
                Debug.LogWarning($"[BoardConfig] Piece shape '{shape.name}' is too large for {Rows}x{Columns} board.");
            }
        }
    }
}

[Serializable]
public struct BlockColorEntry
{
    public int Value;
    public Color Color;
}
