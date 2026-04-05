using System;
using System.Collections.Generic;
using UnityEngine;
using NumbersBlast.Board;

namespace NumbersBlast.Feedback
{
    public interface IFeedbackManager
    {
        void Initialize(RectTransform shakeTarget);
        void StartMergeHover(IReadOnlyList<CellView> mergeCells);
        void StopMergeHover(IReadOnlyList<CellView> mergeCells);
        void PlayPlaceEffect(CellView[] cells);
        void PlayMergeSmash(CellView targetCell, CellView[] absorbedCells);
        void PlayChainMergeSmash(CellView targetCell);
        void PlayLineClearEffect(CellView[] cells, Action onComplete);
        void PlayGameOverEffect(Action onComplete);
        void PlayPiecePickupEffect(Transform pieceTransform);
    }
}
