using UnityEngine;

namespace NumbersBlast.Piece
{
    /// <summary>
    /// Data model for a draggable piece, holding its cell positions and numeric values.
    /// </summary>
    public class PieceModel
    {
        /// <summary>
        /// The shape definition that this piece was created from. Null for manually constructed pieces.
        /// </summary>
        public PieceShapeData Shape { get; private set; }

        /// <summary>
        /// Normalized cell positions relative to the piece origin.
        /// </summary>
        public Vector2Int[] Positions { get; private set; }

        /// <summary>
        /// Numeric value assigned to each cell, indexed in the same order as <see cref="Positions"/>.
        /// </summary>
        public int[] Values { get; private set; }

        /// <summary>
        /// Creates a piece from a shape definition with randomly generated cell values.
        /// </summary>
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

        /// <summary>
        /// Creates a piece with explicit positions and predetermined values (used for tutorials).
        /// </summary>
        public PieceModel(Vector2Int[] positions, int[] fixedValues)
        {
            Shape = null;
            Positions = positions;
            Values = fixedValues;
        }

        public int GetValueAt(int index)
        {
            return Values[index];
        }

        public int CellCount => Positions.Length;
    }
}
