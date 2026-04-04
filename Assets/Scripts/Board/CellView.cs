using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CellView : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private TextMeshProUGUI _valueText;

    private static readonly Color ValidHighlightColor = new(0.5f, 0.9f, 0.5f, 0.6f);
    private static readonly Color InvalidHighlightColor = new(0.9f, 0.4f, 0.4f, 0.6f);

    private CellData _data;
    private BoardConfig _config;
    private bool _isHighlighted;

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
        if (_isHighlighted) return;

        if (_data.IsEmpty)
        {
            _valueText.text = "";
            _background.color = _config.EmptyCellColor;
            return;
        }

        _valueText.text = _data.Value.ToString();
        _background.color = _config.GetBlockColor(_data.Value);
    }

    public void SetHighlight(bool active, bool canPlace)
    {
        _isHighlighted = active;

        if (active)
        {
            _background.color = canPlace ? ValidHighlightColor : InvalidHighlightColor;
        }
        else
        {
            Refresh();
        }
    }
}
