using System;
using System.Collections.Generic;
using UnityEngine;
using NumbersBlast.Board;

namespace NumbersBlast.Feedback
{
    /// <summary>
    /// Contract for visual and haptic feedback effects triggered by gameplay events.
    /// </summary>
    public interface IFeedbackManager
    {
        /// <summary>
        /// Initializes the feedback system with the target RectTransform used for screen shake.
        /// </summary>
        void Initialize(RectTransform shakeTarget);

        /// <summary>
        /// Starts the hover animation on the board and merge-candidate cells.
        /// </summary>
        void StartMergeHover(IReadOnlyList<CellView> mergeCells);

        /// <summary>
        /// Stops the merge hover animation and resets cell scales.
        /// </summary>
        void StopMergeHover(IReadOnlyList<CellView> mergeCells);

        /// <summary>
        /// Plays a pop-in scale effect on newly placed cells with a light haptic.
        /// </summary>
        void PlayPlaceEffect(CellView[] cells);

        /// <summary>
        /// Plays the merge impact animation where absorbed cells shrink into the target cell.
        /// </summary>
        void PlayMergeSmash(CellView targetCell, CellView[] absorbedCells);

        /// <summary>
        /// Plays an intensified merge impact animation for chain merges.
        /// </summary>
        void PlayChainMergeSmash(CellView targetCell);

        /// <summary>
        /// Plays a sequential grow-and-shrink clear animation on cells, invoking onComplete when all finish.
        /// </summary>
        void PlayLineClearEffect(CellView[] cells, Action onComplete);

        /// <summary>
        /// Plays a heavy screen shake and haptic for the game over event.
        /// </summary>
        void PlayGameOverEffect(Action onComplete);

        /// <summary>
        /// Plays a punch-scale effect on the piece transform when picked up.
        /// </summary>
        void PlayPiecePickupEffect(Transform pieceTransform);
    }
}
