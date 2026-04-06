using System.Collections.Generic;
using UnityEngine;

namespace NumbersBlast.Gameplay
{
    /// <summary>
    /// Contains the score and cleared cell positions resulting from a line clear operation.
    /// </summary>
    public struct LineClearResult
    {
        /// <summary>
        /// Total score earned from the cleared cells.
        /// </summary>
        public int Score;

        /// <summary>
        /// Board positions of all cells that were cleared.
        /// </summary>
        public List<Vector2Int> ClearedPositions;
    }
}
