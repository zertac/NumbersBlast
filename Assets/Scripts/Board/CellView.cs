using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NumbersBlast.Core;
using NumbersBlast.Data;

namespace NumbersBlast.Board
{
    /// <summary>
    /// MonoBehaviour that renders a single board cell, displaying its value and highlight state based on theme data.
    /// </summary>
    public class CellView : MonoBehaviour
    {
        [SerializeField] private Image _background;
        [SerializeField] private Image _highlight;
        [SerializeField] private TextMeshProUGUI _valueText;

        private CellData _data;
        private ThemeData _theme;
        private HighlightType _highlightType;

        /// <summary>
        /// The current highlight state applied to this cell.
        /// </summary>
        public HighlightType CurrentHighlight => _highlightType;

        /// <summary>
        /// The underlying data model for this cell.
        /// </summary>
        public CellData Data => _data;

        /// <summary>
        /// Cached RectTransform for layout and positioning calculations.
        /// </summary>
        public RectTransform RectTransform { get; private set; }

        /// <summary>
        /// Binds this view to its cell data and theme, then performs the initial visual refresh.
        /// </summary>
        public void Initialize(CellData data, ThemeData theme)
        {
            _data = data;
            _theme = theme;
            if (RectTransform == null) RectTransform = GetComponent<RectTransform>();

            if (_theme.HighlightSprite != null)
                _highlight.sprite = _theme.HighlightSprite;
            _highlight.gameObject.SetActive(false);
            Refresh();
        }

        /// <summary>
        /// Updates the cell's visual appearance (sprite, color, text) to reflect the current data value.
        /// </summary>
        public void Refresh()
        {
            if (_data.IsEmpty)
            {
                _valueText.text = "";
                _background.sprite = _theme.EmptyCellSprite;
                _background.color = _theme.EmptyCellColor;
                return;
            }

            var visual = _theme.GetBlockVisual(_data.Value);
            _valueText.text = StringCache.IntToString(_data.Value);
            _background.sprite = visual.Sprite != null ? visual.Sprite : _theme.BlockSprite;
            _background.color = visual.Color;
        }

        /// <summary>
        /// Applies or clears a highlight overlay on the cell based on the given highlight type.
        /// </summary>
        public void SetHighlight(HighlightType type)
        {
            _highlightType = type;

            if (type == HighlightType.None)
            {
                _highlight.gameObject.SetActive(false);
                Refresh();
                return;
            }

            _highlight.gameObject.SetActive(true);
            _highlight.color = type switch
            {
                HighlightType.Placement => _theme.PlacementHighlight,
                HighlightType.Invalid => _theme.InvalidHighlight,
                HighlightType.Merge => _theme.MergeHighlight,
                HighlightType.LineClear => _theme.LineClearHighlight,
                HighlightType.TutorialTarget => _theme.TutorialTargetHighlight,
                _ => Color.clear
            };
        }
    }
}
