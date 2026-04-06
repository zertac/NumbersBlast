using UnityEngine;

namespace NumbersBlast.Data
{
    /// <summary>
    /// ScriptableObject holding all visual theme settings including background, board, cell, highlight, and UI colors and sprites.
    /// </summary>
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

        private BlockVisual[] _visualCache;
        private static readonly BlockVisual DefaultVisual = new() { Value = -1, Color = Color.gray };

        /// <summary>
        /// Returns the <see cref="BlockVisual"/> for the given block value, or a default gray visual if not found.
        /// </summary>
        public BlockVisual GetBlockVisual(int value)
        {
            if (_visualCache == null || _visualCache.Length == 0)
                BuildCache();

            if (value >= 0 && value < _visualCache.Length && _visualCache[value].Value == value)
                return _visualCache[value];

            return DefaultVisual;
        }

        private void BuildCache()
        {
            if (BlockVisuals == null || BlockVisuals.Length == 0) return;

            int maxValue = 0;
            for (int i = 0; i < BlockVisuals.Length; i++)
            {
                if (BlockVisuals[i].Value > maxValue)
                    maxValue = BlockVisuals[i].Value;
            }

            _visualCache = new BlockVisual[maxValue + 1];
            for (int i = 0; i < BlockVisuals.Length; i++)
            {
                _visualCache[BlockVisuals[i].Value] = BlockVisuals[i];
            }
        }

        private void OnEnable()
        {
            BuildCache();
        }
    }

}
