using UnityEngine;

public class PieceTray : MonoBehaviour
{
    [SerializeField] private Transform[] _pieceSlots;
    [SerializeField] private GameObject _piecePrefab;

    private PieceView[] _pieceViews;
    private BoardConfig _config;
    private BoardView _boardView;
    private BoardManager _boardManager;
    private Canvas _canvas;
    private float _cellSize;

    public void Initialize(BoardConfig config, float cellSize, Canvas canvas, BoardView boardView, BoardManager boardManager)
    {
        _config = config;
        _cellSize = cellSize;
        _canvas = canvas;
        _boardView = boardView;
        _boardManager = boardManager;
        _pieceViews = new PieceView[_pieceSlots.Length];
    }

    public void SpawnPieces()
    {
        var spawnConfig = _config.PieceSpawnConfig;

        for (int i = 0; i < _pieceSlots.Length; i++)
        {
            if (_pieceViews[i] != null) continue;

            var shape = spawnConfig.Shapes[Random.Range(0, spawnConfig.Shapes.Length)];
            var model = new PieceModel(shape, _config.MinBlockValue, _config.MaxBlockValue);

            var pieceGo = Instantiate(_piecePrefab, _pieceSlots[i]);
            var pieceView = pieceGo.GetComponent<PieceView>();
            pieceView.Initialize(model, _config, _cellSize);

            var dragHandler = pieceGo.AddComponent<PieceDragHandler>();
            dragHandler.Initialize(pieceView, _canvas, _boardView, _boardManager, _config);

            var slotRect = _pieceSlots[i].GetComponent<RectTransform>();
            float scale = CalculateFitScale(model.Shape, slotRect);
            pieceView.SetScale(scale);

            _pieceViews[i] = pieceView;
        }
    }

    public bool IsEmpty()
    {
        for (int i = 0; i < _pieceViews.Length; i++)
        {
            if (_pieceViews[i] != null) return false;
        }
        return true;
    }

    private float CalculateFitScale(PieceShapeData shape, RectTransform slotRect)
    {
        var size = shape.GetNormalizedSize();
        float pieceWidth = size.y * _cellSize;
        float pieceHeight = size.x * _cellSize;

        float scaleX = slotRect.sizeDelta.x / pieceWidth;
        float scaleY = slotRect.sizeDelta.y / pieceHeight;

        return Mathf.Min(scaleX, scaleY, 0.6f);
    }

    public void RemovePiece(PieceView piece)
    {
        for (int i = 0; i < _pieceViews.Length; i++)
        {
            if (_pieceViews[i] == piece)
            {
                _pieceViews[i] = null;
                break;
            }
        }

        if (IsEmpty())
        {
            SpawnPieces();
        }
    }
}
