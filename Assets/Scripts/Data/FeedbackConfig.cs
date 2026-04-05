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

    [Header("Merge")]
    public float MergePunchScale = 0.3f;

    [Header("Line Clear")]
    public float ClearDuration = 0.3f;
    public float ClearDelay = 0.03f;

    [Header("Hover Anticipation")]
    public float HoverBoardScale = 1.015f;
    public float HoverGrowDuration = 0.3f;
}
