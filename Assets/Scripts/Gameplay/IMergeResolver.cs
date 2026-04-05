using System.Collections.Generic;
using UnityEngine;
using NumbersBlast.Board;
using NumbersBlast.Piece;

namespace NumbersBlast.Gameplay
{
    public interface IMergeResolver
    {
        List<MergeEvent> Resolve(BoardModel model, PieceModel pieceModel, Vector2Int boardPos);
    }
}
