using UnityEngine;

namespace NumbersBlast.Multiplayer
{
    public struct OpponentMove
    {
        public int PieceIndex;
        public Vector2Int BoardPosition;
        public float Score;
        public bool IsValid;
    }
}
