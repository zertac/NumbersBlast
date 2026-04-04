using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PieceCellView : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private TextMeshProUGUI _valueText;

    private int _value;
    private BoardConfig _config;

    public RectTransform RectTransform { get; private set; }

    public void Initialize(int value, BoardConfig config)
    {
        _value = value;
        _config = config;
        RectTransform = GetComponent<RectTransform>();
        Refresh();
    }

    private void Refresh()
    {
        _valueText.text = _value.ToString();
        _background.color = _config.GetBlockColor(_value);
        _background.raycastTarget = false;
        _valueText.raycastTarget = false;
    }
}
