using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum HighlightType
{
    None,
    Placement,
    Invalid,
    Merge,
    LineClear,
    TutorialTarget
}

public class CellView : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private Image _highlight;
    [SerializeField] private TextMeshProUGUI _valueText;

    private CellData _data;
    private ThemeData _theme;
    private HighlightType _highlightType;

    public HighlightType CurrentHighlight => _highlightType;
    public CellData Data => _data;
    public RectTransform RectTransform { get; private set; }

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
        _valueText.text = _data.Value.ToString();
        _background.sprite = visual.Sprite != null ? visual.Sprite : _theme.BlockSprite;
        _background.color = visual.Color;
    }

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
