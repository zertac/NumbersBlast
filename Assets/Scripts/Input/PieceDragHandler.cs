using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PieceDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    private PieceView _pieceView;
    private Canvas _canvas;
    private RectTransform _canvasRect;
    private BoardView _boardView;
    private BoardManager _boardManager;
    private BoardConfig _config;

    private Vector2 _originalPosition;
    private Transform _originalParent;
    private float _originalScale;
    private Vector2 _dragOffset;
    private bool _isDragging;

    private const float DragScale = 1f;
    private const float PickupDuration = 0.15f;
    private const float ReturnDuration = 0.2f;
    private const float DragOffsetY = 150f;

    public void Initialize(PieceView pieceView, Canvas canvas, BoardView boardView, BoardManager boardManager, BoardConfig config)
    {
        _pieceView = pieceView;
        _canvas = canvas;
        _canvasRect = canvas.GetComponent<RectTransform>();
        _boardView = boardView;
        _boardManager = boardManager;
        _config = config;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _originalScale = _pieceView.transform.localScale.x;
        _originalPosition = _pieceView.RectTransform.anchoredPosition;
        _originalParent = _pieceView.transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDragging = true;

        _pieceView.transform.SetParent(_canvasRect, true);
        _pieceView.transform.SetAsLastSibling();

        _pieceView.transform.DOScale(DragScale, PickupDuration).SetEase(Ease.OutBack);

        _dragOffset = new Vector2(0, DragOffsetY);

        GameEvents.PiecePickedUp(_pieceView);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRect, eventData.position, _canvas.worldCamera, out var localPoint);

        _pieceView.RectTransform.anchoredPosition = localPoint + _dragOffset;

        UpdateBoardHighlight();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;
        _isDragging = false;

        var boardPos = GetBoardPosition();

        if (boardPos.HasValue && CanPlace(boardPos.Value))
        {
            GameEvents.PiecePlaced(_pieceView, boardPos.Value);
        }
        else
        {
            ReturnToTray();
            GameEvents.PieceReleased(_pieceView);
        }

        ClearBoardHighlight();
    }

    private void UpdateBoardHighlight()
    {
        ClearBoardHighlight();

        var boardPos = GetBoardPosition();
        if (!boardPos.HasValue) return;

        var positions = _pieceView.Model.Positions;
        bool canPlace = CanPlace(boardPos.Value);

        for (int i = 0; i < positions.Length; i++)
        {
            int row = boardPos.Value.x + positions[i].x;
            int col = boardPos.Value.y + positions[i].y;

            var cellView = _boardView.GetCellView(row, col);
            if (cellView != null)
            {
                cellView.SetHighlight(true, canPlace);
            }
        }
    }

    private void ClearBoardHighlight()
    {
        var cellViews = _boardView.CellViews;
        for (int r = 0; r < cellViews.GetLength(0); r++)
        {
            for (int c = 0; c < cellViews.GetLength(1); c++)
            {
                cellViews[r, c].SetHighlight(false, false);
            }
        }
    }

    private Vector2Int? GetBoardPosition()
    {
        var positions = _pieceView.Model.Positions;
        if (positions.Length == 0) return null;

        var firstCellView = _boardView.GetCellView(0, 0);
        if (firstCellView == null) return null;

        var boardRect = _boardView.GetComponent<RectTransform>();
        var pieceWorldPos = _pieceView.RectTransform.position;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            boardRect,
            RectTransformUtility.WorldToScreenPoint(_canvas.worldCamera, pieceWorldPos),
            _canvas.worldCamera,
            out var localPoint);

        float cellSize = _boardView.CellSize;
        float spacing = _config.CellSpacing;
        float cellStep = cellSize + spacing;

        float boardWidth = cellStep * _config.Columns - spacing;
        float boardHeight = cellStep * _config.Rows - spacing;

        float relX = localPoint.x + boardWidth * 0.5f;
        float relY = boardHeight * 0.5f - localPoint.y;

        int col = Mathf.RoundToInt(relX / cellStep - 0.5f);
        int row = Mathf.RoundToInt(relY / cellStep - 0.5f);

        return new Vector2Int(row, col);
    }

    private bool CanPlace(Vector2Int boardPos)
    {
        var positions = _pieceView.Model.Positions;
        var model = _boardManager.Model;

        for (int i = 0; i < positions.Length; i++)
        {
            int row = boardPos.x + positions[i].x;
            int col = boardPos.y + positions[i].y;

            if (!model.IsInBounds(row, col)) return false;
            if (!model.IsCellEmpty(row, col)) return false;
        }

        return true;
    }

    private void ReturnToTray()
    {
        _pieceView.transform.SetParent(_originalParent, true);
        _pieceView.RectTransform.DOAnchorPos(_originalPosition, ReturnDuration).SetEase(Ease.OutCubic);
        _pieceView.transform.DOScale(_originalScale, ReturnDuration).SetEase(Ease.OutCubic);
    }
}
