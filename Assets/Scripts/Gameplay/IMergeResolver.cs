using System.Collections.Generic;
using UnityEngine;
using NumbersBlast.Board;
using NumbersBlast.Piece;

namespace NumbersBlast.Gameplay
{
    /// <summary>
    /// Resolves merge operations after a piece is placed on the board.
    /// </summary>
    public interface IMergeResolver
    {
        /// <summary>
        /// Finds and executes all merges triggered by placing a piece at the given board position.
        /// </summary>
        List<MergeEvent> Resolve(BoardModel model, PieceModel pieceModel, Vector2Int boardPos);
    }
}
