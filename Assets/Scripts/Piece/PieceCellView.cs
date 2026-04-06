using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NumbersBlast.Core;
using NumbersBlast.Data;

namespace NumbersBlast.Piece
{
    /// <summary>
    /// Visual representation of a single cell within a piece, displaying its numeric value and themed background.
    /// </summary>
    public class PieceCellView : MonoBehaviour
    {
        [SerializeField] private Image _background;
        [SerializeField] private TextMeshProUGUI _valueText;

        private int _value;
        private ThemeData _theme;

        public RectTransform RectTransform { get; private set; }

        /// <summary>
        /// Sets the cell value and theme, then refreshes the visual appearance.
        /// </summary>
        public void Initialize(int value, ThemeData theme)
        {
            _value = value;
            _theme = theme;
            if (RectTransform == null) RectTransform = GetComponent<RectTransform>();
            Refresh();
        }

        private void Refresh()
        {
            var visual = _theme.GetBlockVisual(_value);
            _valueText.text = StringCache.IntToString(_value);
            _background.color = visual.Color;
            _background.sprite = visual.Sprite != null ? visual.Sprite : _theme.BlockSprite;

            _background.raycastTarget = false;
            _valueText.raycastTarget = false;
        }
    }
}
