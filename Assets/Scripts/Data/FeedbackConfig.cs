using UnityEngine;

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

    [Header("Merge - Absorbed Cell")]
    public float AbsorbedFlashScale = 1.3f;
    public float AbsorbedFlashDuration = 0.06f;
    public float AbsorbedShrinkDuration = 0.1f;

    [Header("Merge - Impact")]
    public float ImpactDelay = 0.18f;
    public float ImpactScale = 1.5f;
    public float ImpactDuration = 0.08f;
    public float ChainImpactScale = 1.8f;

    [Header("Line Clear")]
    public float ClearDuration = 0.3f;
    public float ClearDelay = 0.03f;
    public float ClearGrowRatio = 0.3f;

    [Header("Ghost Piece")]
    public float GhostAlpha = 0.6f;
}
