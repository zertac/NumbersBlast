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
    [SerializeField] private TextMeshProUGUI _valueText;

    private static readonly Color PlacementColor = new(0.4f, 0.85f, 0.5f, 1f);
    private static readonly Color InvalidColor = new(0.9f, 0.4f, 0.4f, 0.6f);
    private static readonly Color MergeColor = new(1f, 0.9f, 0.3f, 0.7f);
    private static readonly Color LineClearColor = new(0.3f, 0.7f, 1f, 0.7f);
    private static readonly Color TutorialTargetColor = new(1f, 0.9f, 0.2f, 1f);

    private CellData _data;
    private BoardConfig _config;
    private HighlightType _highlightType;

    public HighlightType CurrentHighlight => _highlightType;

    public CellData Data => _data;
    public RectTransform RectTransform { get; private set; }

    public void Initialize(CellData data, BoardConfig config)
    {
        _data = data;
        _config = config;
        RectTransform = GetComponent<RectTransform>();
        Refresh();
    }

    public void Refresh()
    {
        if (_highlightType != HighlightType.None) return;

        if (_data.IsEmpty)
        {
            _valueText.text = "";
            _background.color = _config.EmptyCellColor;
            return;
        }

        _valueText.text = _data.Value.ToString();
        _background.color = _config.GetBlockColor(_data.Value);
    }

    public void SetHighlight(HighlightType type)
    {
        _highlightType = type;

        if (type == HighlightType.None)
        {
            Refresh();
            return;
        }

        _background.color = type switch
        {
            HighlightType.Placement => PlacementColor,
            HighlightType.Invalid => InvalidColor,
            HighlightType.Merge => MergeColor,
            HighlightType.LineClear => LineClearColor,
            HighlightType.TutorialTarget => TutorialTargetColor,
            _ => _config.EmptyCellColor
        };
    }
}
