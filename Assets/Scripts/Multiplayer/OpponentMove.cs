using UnityEngine;

namespace NumbersBlast.Multiplayer
{
    /// <summary>
    /// Represents a candidate move for the AI opponent, including target piece, board position, and evaluation score.
    /// </summary>
    public struct OpponentMove
    {
        /// <summary>Index of the selected piece in the tray.</summary>
        public int PieceIndex;
        /// <summary>Target board position for the placement.</summary>
        public Vector2Int BoardPosition;
        /// <summary>Evaluation score assigned by the AI.</summary>
        public float Score;
        /// <summary>Whether a valid placement was found.</summary>
        public bool IsValid;
    }
}
