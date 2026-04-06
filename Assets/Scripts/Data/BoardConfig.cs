using UnityEngine;
using NumbersBlast.Piece;

namespace NumbersBlast.Data
{
    /// <summary>
    /// ScriptableObject defining board dimensions, cell spacing, block value range, and references to piece spawn and theme configurations.
    /// </summary>
    // L-17: Public fields are required here instead of properties because Unity's
    // ScriptableObject serialization system only serializes public fields (or [SerializeField]).
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

        [Header("Theme")]
        public ThemeData Theme;

        /// <summary>
        /// Checks whether the given piece shape fits within the board dimensions.
        /// </summary>
        public bool ValidatePieceShape(PieceShapeData shape)
        {
            var size = shape.GetNormalizedSize();
            return size.x <= Rows && size.y <= Columns;
        }

        private void OnValidate()
        {
            if (PieceSpawnConfig == null || PieceSpawnConfig.Shapes == null) return;

            for (int i = 0; i < PieceSpawnConfig.Shapes.Length; i++)
            {
                var shape = PieceSpawnConfig.Shapes[i];
                if (shape == null) continue;

                if (!ValidatePieceShape(shape))
                {
#if UNITY_EDITOR || DEBUG
                    Debug.LogWarning($"[BoardConfig] Piece shape '{shape.name}' is too large for {Rows}x{Columns} board.");
#endif
                }
            }
        }
    }
}
