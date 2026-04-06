using UnityEngine;

namespace NumbersBlast.Tutorial
{
    /// <summary>
    /// ScriptableObject holding the sequence of tutorial steps and shared tutorial settings.
    /// </summary>
    [CreateAssetMenu(fileName = "TutorialConfig", menuName = "NumbersBlast/Tutorial Config")]
    public class TutorialConfig : ScriptableObject
    {
        public TutorialStepData[] Steps;

        [Header("Hand Icon")]
        public Vector2 HandOffset = new(30f, -30f);
    }
}
