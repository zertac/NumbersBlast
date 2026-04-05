using System;
using UnityEngine;

namespace NumbersBlast.Data
{
    [CreateAssetMenu(fileName = "ThemeData", menuName = "NumbersBlast/Theme Data")]
    public class ThemeData : ScriptableObject
    {
        [Header("Background")]
        public Color BackgroundColor = new(0.15f, 0.15f, 0.3f);
        public Sprite BackgroundSprite;

        [Header("Board")]
        public Sprite BoardFrameSprite;
        public Color BoardFrameColor = Color.white;

        [Header("Cells")]
        public Sprite EmptyCellSprite;
        public Color EmptyCellColor = new(0.85f, 0.85f, 0.85f);
        public Sprite BlockSprite;
        public BlockVisual[] BlockVisuals;

        [Header("Highlights")]
        public Sprite HighlightSprite;
        public Color PlacementHighlight = new(0.4f, 0.85f, 0.5f, 0.5f);
        public Color InvalidHighlight = new(0.9f, 0.4f, 0.4f, 0.6f);
        public Color MergeHighlight = new(1f, 0.9f, 0.3f, 0.7f);
        public Color LineClearHighlight = new(0.3f, 0.7f, 1f, 0.7f);
        public Color TutorialTargetHighlight = new(1f, 0.9f, 0.2f, 1f);

        [Header("UI")]
        public Color ScoreTextColor = Color.white;
        public Color ButtonColor = new(0.3f, 0.7f, 0.4f);
        public Color ButtonTextColor = Color.white;

        [Header("Multiplayer")]
        public Color PlayerTurnColor = new(0.3f, 0.85f, 0.4f);
        public Color OpponentTurnColor = new(0.9f, 0.5f, 0.3f);

        public BlockVisual GetBlockVisual(int value)
        {
            for (int i = 0; i < BlockVisuals.Length; i++)
            {
                if (BlockVisuals[i].Value == value)
                    return BlockVisuals[i];
            }

            return new BlockVisual { Value = value, Color = Color.gray };
        }
    }

    [Serializable]
    public struct BlockVisual
    {
        public int Value;
        public Color Color;
        public Sprite Sprite;
    }
}
