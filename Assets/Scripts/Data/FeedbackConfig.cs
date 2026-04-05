using UnityEngine;

namespace NumbersBlast.Data
{
    [CreateAssetMenu(fileName = "FeedbackConfig", menuName = "NumbersBlast/Feedback Config")]
    public class FeedbackConfig : ScriptableObject
    {
        [Header("Screen Shake")]
        public float ShakeStrength = 10f;
        public float ShakeDuration = 0.3f;
        public int ShakeVibrato = 15;

        [Header("Cell Place")]
        public float PlacePunchScale = 0.2f;
        public float PlaceDuration = 0.25f;
        public float PlaceDelayPerCell = 0.03f;

        [Header("Merge - Hover")]
        public float HoverBoardScale = 1.015f;
        public float HoverGrowDuration = 0.3f;
        public float HoverPulseIncrement = 0.005f;
        public float HoverCellScale = 1.08f;

        [Header("Merge - Reset")]
        public float MergeResetDuration = 0.05f;

        [Header("Merge - Absorbed Cell")]
        public float AbsorbedFlashScale = 1.3f;
        public float AbsorbedFlashDuration = 0.06f;
        public float AbsorbedShrinkDuration = 0.1f;

        [Header("Merge - Impact")]
        public float ImpactDelay = 0.18f;
        public float ImpactScale = 1.5f;
        public float ImpactDuration = 0.08f;
        public float ChainImpactScale = 1.8f;
        public float ChainImpactDuration = 0.1f;

        [Header("Merge - Squash/Stretch")]
        public float SquashScaleX = 0.85f;
        public float SquashScaleY = 1.15f;
        public float StretchScaleX = 1.1f;
        public float StretchScaleY = 0.9f;
        public float SquashDuration = 0.06f;
        public float StretchDuration = 0.05f;
        public float SettleDuration = 0.1f;
        public float ChainSquashScaleX = 0.7f;
        public float ChainSquashScaleY = 1.3f;
        public float ChainStretchScaleX = 1.15f;
        public float ChainStretchScaleY = 0.85f;
        public float ChainSquashDuration = 0.07f;
        public float ChainStretchDuration = 0.05f;
        public float ChainSettleDuration = 0.12f;

        [Header("Hover")]
        public float HoverPulseDuration = 0.4f;
        public float HoverCellPulseDuration = 0.3f;
        public float HoverResetDuration = 0.15f;

        [Header("Piece Pickup")]
        public float PickupPunchScale = 0.1f;
        public float PickupPunchDuration = 0.15f;

        [Header("Line Clear")]
        public float ClearDuration = 0.3f;
        public float ClearDelay = 0.03f;
        public float ClearGrowRatio = 0.3f;
        public float ClearGrowScale = 1.2f;

        [Header("Ghost Piece")]
        public float GhostAlpha = 0.6f;
    }
}
