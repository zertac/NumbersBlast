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
    private TutorialManager _tutorialManager;
    private FeedbackManager _feedbackManager;

    public void Initialize(BoardConfig config, float cellSize, Canvas canvas, BoardView boardView, BoardManager boardManager, TutorialManager tutorialManager = null, FeedbackManager feedbackManager = null)
    {
        _config = config;
        _cellSize = cellSize;
        _canvas = canvas;
        _boardView = boardView;
        _boardManager = boardManager;
        _tutorialManager = tutorialManager;
        _feedbackManager = feedbackManager;
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
            dragHandler.Initialize(pieceView, _canvas, _boardView, _boardManager, _config, _tutorialManager, _feedbackManager);

            var slotRect = _pieceSlots[i].GetComponent<RectTransform>();
            float scale = CalculateFitScale(model.Shape, slotRect);
            pieceView.SetScale(scale);

            _pieceViews[i] = pieceView;
        }
    }

    public PieceView[] GetRemainingPieces()
    {
        return _pieceViews;
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

    public void SpawnTutorialPiece(PieceModel model)
    {
        ClearAll();

        var pieceGo = Instantiate(_piecePrefab, _pieceSlots[0]);
        var pieceView = pieceGo.GetComponent<PieceView>();
        pieceView.Initialize(model, _config, _cellSize);

        var dragHandler = pieceGo.AddComponent<PieceDragHandler>();
        dragHandler.Initialize(pieceView, _canvas, _boardView, _boardManager, _config, _tutorialManager);

        var slotRect = _pieceSlots[0].GetComponent<RectTransform>();
        float pieceWidth = 1;
        float pieceHeight = 1;
        for (int i = 0; i < model.Positions.Length; i++)
        {
            if (model.Positions[i].y + 1 > pieceWidth) pieceWidth = model.Positions[i].y + 1;
            if (model.Positions[i].x + 1 > pieceHeight) pieceHeight = model.Positions[i].x + 1;
        }

        float scaleX = slotRect.sizeDelta.x / (pieceWidth * _cellSize);
        float scaleY = slotRect.sizeDelta.y / (pieceHeight * _cellSize);
        pieceView.SetScale(Mathf.Min(scaleX, scaleY, 0.6f));

        _pieceViews[0] = pieceView;
    }

    public void ClearAll()
    {
        for (int i = 0; i < _pieceViews.Length; i++)
        {
            if (_pieceViews[i] != null)
            {
                Destroy(_pieceViews[i].gameObject);
                _pieceViews[i] = null;
            }
        }
    }

    public PieceView GetFirstOccupiedPiece()
    {
        for (int i = 0; i < _pieceViews.Length; i++)
        {
            if (_pieceViews[i] != null)
                return _pieceViews[i];
        }
        return null;
    }

    public RectTransform GetFirstOccupiedPieceRect()
    {
        for (int i = 0; i < _pieceViews.Length; i++)
        {
            if (_pieceViews[i] != null)
                return _pieceViews[i].RectTransform;
        }
        return null;
    }

    public RectTransform GetFirstOccupiedSlotRect()
    {
        for (int i = 0; i < _pieceViews.Length; i++)
        {
            if (_pieceViews[i] != null)
                return _pieceSlots[i].GetComponent<RectTransform>();
        }
        return null;
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
