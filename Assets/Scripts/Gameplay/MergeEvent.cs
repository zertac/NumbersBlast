using System.Collections.Generic;
using UnityEngine;

namespace NumbersBlast.Gameplay
{
    public struct MergeEvent
    {
        public Vector2Int TargetPos;
        public List<Vector2Int> AbsorbedPositions;
        public bool IsChain;
    }
}
