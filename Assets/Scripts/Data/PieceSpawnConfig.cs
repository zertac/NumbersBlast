using UnityEngine;
using NumbersBlast.Piece;

namespace NumbersBlast.Data
{
    [CreateAssetMenu(fileName = "PieceSpawnConfig", menuName = "NumbersBlast/Piece Spawn Config")]
    public class PieceSpawnConfig : ScriptableObject
    {
        [Header("Available Shapes")]
        public PieceShapeData[] Shapes;

        [Header("Tray")]
        public int PiecesPerTray = 3;
    }
}
