using UnityEngine;

[CreateAssetMenu(fileName = "TutorialConfig", menuName = "NumbersBlast/Tutorial Config")]
public class TutorialConfig : ScriptableObject
{
    public TutorialStepData[] Steps;
}
