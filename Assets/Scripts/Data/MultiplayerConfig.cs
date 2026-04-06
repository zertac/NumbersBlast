using UnityEngine;

namespace NumbersBlast.Data
{
    /// <summary>
    /// ScriptableObject containing multiplayer settings including turn timing, opponent AI behavior weights, and fake opponent personality parameters.
    /// </summary>
    [CreateAssetMenu(fileName = "MultiplayerConfig", menuName = "NumbersBlast/Multiplayer Config")]
    public class MultiplayerConfig : ScriptableObject
    {
        [Header("Turn")]
        public float TurnDuration = 20f;
        public float PenaltyPercent = 0.05f;

        [Header("Opponent Search")]
        public float MinSearchDuration = 2f;
        public float MaxSearchDuration = 5f;
        public string[] FakeNames;

        [Header("Opponent Personality")]
        [Range(0f, 1f)] public float MistakeChance = 0.2f;
        [Range(0f, 1f)] public float HesitationChance = 0.5f;
        [Range(0f, 1f)] public float CancelChance = 0.3f;

        [Header("Opponent AI Weights")]
        public float MergeWeight = 10f;
        public float LineClearWeight = 50f;
        public float CenterBonus = 5f;

        [Header("Opponent Timing")]
        public float MinThinkTime = 1.5f;
        public float MaxThinkTime = 4f;
        public float MinHoverTime = 0.5f;
        public float MaxHoverTime = 1.5f;
        public float MinHesitationTime = 0.8f;
        public float MaxHesitationTime = 2f;
        public float MoveSpeed = 0.4f;

        [Header("Opponent Wander")]
        [Range(0f, 1f)] public float InvalidMoveChance = 0.3f;
        public float MinWanderPause = 0.3f;
        public float MaxWanderPause = 1.0f;
    }
}
