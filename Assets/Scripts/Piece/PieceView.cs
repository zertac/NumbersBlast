using UnityEngine;
using UnityEngine.UI;
using NumbersBlast.Data;

namespace NumbersBlast.Piece
{
    /// <summary>
    /// Visual representation of a draggable piece, responsible for instantiating and laying out cell views.
    /// </summary>
    public class PieceView : MonoBehaviour
    {
        [SerializeField] private GameObject _cellPrefab;
        [SerializeField] private Image _raycastImage;

        private PieceModel _model;
        private BoardConfig _config;
        private float _cellSize;

        public RectTransform RectTransform { get; private set; }
        public PieceModel Model => _model;

        /// <summary>
        /// Initializes the piece view with its data model and creates the cell visuals.
        /// </summary>
        public void Initialize(PieceModel model, BoardConfig config, float cellSize)
        {
            _model = model;
            _config = config;
            _cellSize = cellSize;
            if (RectTransform == null) RectTransform = GetComponent<RectTransform>();

            CreateCells();
        }

        private void CreateCells()
        {
            int maxRow = 0, maxCol = 0;
            for (int i = 0; i < _model.CellCount; i++)
            {
                if (_model.Positions[i].x > maxRow) maxRow = _model.Positions[i].x;
                if (_model.Positions[i].y > maxCol) maxCol = _model.Positions[i].y;
            }
            float totalWidth = (maxCol + 1) * _cellSize;
            float totalHeight = (maxRow + 1) * _cellSize;
            float offsetX = -totalWidth * 0.5f;
            float offsetY = totalHeight * 0.5f;

            for (int i = 0; i < _model.CellCount; i++)
            {
                var cellGo = Instantiate(_cellPrefab, transform);
                var cellView = cellGo.GetComponent<PieceCellView>();
                cellView.Initialize(_model.GetValueAt(i), _config.Theme);

                var rect = cellView.RectTransform;
                rect.sizeDelta = new Vector2(_cellSize, _cellSize);
                rect.anchoredPosition = new Vector2(
                    offsetX + _model.Positions[i].y * _cellSize + _cellSize * 0.5f,
                    offsetY - _model.Positions[i].x * _cellSize - _cellSize * 0.5f
                );
            }

            RectTransform.sizeDelta = new Vector2(totalWidth, totalHeight);

            if (_raycastImage == null)
                _raycastImage = gameObject.AddComponent<Image>();
            _raycastImage.color = new Color(0, 0, 0, 0);
            _raycastImage.raycastTarget = true;
        }

        public void SetScale(float scale)
        {
            transform.localScale = Vector3.one * scale;
        }
    }
}
