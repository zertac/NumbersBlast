using System.Collections.Generic;
using UnityEngine;

namespace NumbersBlast.Gameplay
{
    /// <summary>
    /// Represents a single merge operation, including the target cell and all absorbed neighbor positions.
    /// </summary>
    public struct MergeEvent
    {
        /// <summary>
        /// Board position of the cell that absorbs neighboring values.
        /// </summary>
        public Vector2Int TargetPos;

        /// <summary>
        /// Positions of neighbor cells whose values were absorbed into the target.
        /// </summary>
        public List<Vector2Int> AbsorbedPositions;

        /// <summary>
        /// Indicates whether this merge was triggered as a chain reaction from a previous merge.
        /// </summary>
        public bool IsChain;
    }
}
