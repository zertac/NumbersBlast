using UnityEngine;
using UnityEngine.UI;

public class PieceView : MonoBehaviour
{
    [SerializeField] private GameObject _cellPrefab;

    private PieceModel _model;
    private BoardConfig _config;
    private PieceCellView[] _cellViews;
    private float _cellSize;

    public RectTransform RectTransform { get; private set; }
    public PieceModel Model => _model;

    public void Initialize(PieceModel model, BoardConfig config, float cellSize)
    {
        _model = model;
        _config = config;
        _cellSize = cellSize;
        RectTransform = GetComponent<RectTransform>();

        CreateCells();
    }

    private void CreateCells()
    {
        _cellViews = new PieceCellView[_model.CellCount];

        var shapeSize = _model.Shape.GetNormalizedSize();
        float totalWidth = shapeSize.y * _cellSize;
        float totalHeight = shapeSize.x * _cellSize;
        float offsetX = -totalWidth * 0.5f;
        float offsetY = totalHeight * 0.5f;

        for (int i = 0; i < _model.CellCount; i++)
        {
            var cellGo = Instantiate(_cellPrefab, transform);
            var cellView = cellGo.GetComponent<PieceCellView>();
            cellView.Initialize(_model.GetValueAt(i), _config);

            var rect = cellView.RectTransform;
            rect.sizeDelta = new Vector2(_cellSize, _cellSize);
            rect.anchoredPosition = new Vector2(
                offsetX + _model.Positions[i].y * _cellSize + _cellSize * 0.5f,
                offsetY - _model.Positions[i].x * _cellSize - _cellSize * 0.5f
            );

            _cellViews[i] = cellView;
        }

        RectTransform.sizeDelta = new Vector2(totalWidth, totalHeight);

        var image = gameObject.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0);
        image.raycastTarget = true;
    }

    public void SetScale(float scale)
    {
        transform.localScale = Vector3.one * scale;
    }
}
