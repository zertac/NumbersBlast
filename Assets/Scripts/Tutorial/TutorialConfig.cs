using UnityEngine;

namespace NumbersBlast.Tutorial
{
    [CreateAssetMenu(fileName = "TutorialConfig", menuName = "NumbersBlast/Tutorial Config")]
    public class TutorialConfig : ScriptableObject
    {
        public TutorialStepData[] Steps;

        [Header("Hand Icon")]
        public Vector2 HandOffset = new(30f, -30f);
    }
}
