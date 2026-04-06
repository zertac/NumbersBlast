using UnityEngine;
using NumbersBlast.Piece;

namespace NumbersBlast.Data
{
    /// <summary>
    /// ScriptableObject defining which piece shapes are available for spawning and how many pieces appear per tray.
    /// </summary>
    [CreateAssetMenu(fileName = "PieceSpawnConfig", menuName = "NumbersBlast/Piece Spawn Config")]
    public class PieceSpawnConfig : ScriptableObject
    {
        [Header("Available Shapes")]
        public PieceShapeData[] Shapes;

        [Header("Tray")]
        public int PiecesPerTray = 3;
    }
}
