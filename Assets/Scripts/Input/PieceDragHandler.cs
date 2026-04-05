using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using NumbersBlast.Board;
using NumbersBlast.Core;
using NumbersBlast.Data;
using NumbersBlast.Feedback;
using NumbersBlast.Piece;
using NumbersBlast.StateMachine;
using NumbersBlast.Tutorial;

namespace NumbersBlast.Input
{
    public class PieceDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
    {
        private PieceView _pieceView;
        private Canvas _canvas;
        private RectTransform _canvasRect;
        private BoardView _boardView;
        private RectTransform _boardRect;
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

        private TutorialManager _tutorialManager;
        private FeedbackManager _feedbackManager;
        private GameStateManager _gameStateManager;
        private readonly List<CellView> _currentMergeHoverCells = new(16);
        private readonly List<CellView> _mergeCellCache = new(16);
        private readonly HashSet<Vector2Int> _placedSet = new(16);
        private readonly HashSet<Vector2Int> _occupiedAfterPlace = new(64);

        public void Initialize(PieceView pieceView, Canvas canvas, BoardView boardView, BoardManager boardManager, BoardConfig config, TutorialManager tutorialManager = null, FeedbackManager feedbackManager = null, GameStateManager gameStateManager = null)
        {
            _pieceView = pieceView;
            _canvas = canvas;
            _canvasRect = canvas.GetComponent<RectTransform>();
            _boardView = boardView;
            _boardRect = boardView.GetComponent<RectTransform>();
            _boardManager = boardManager;
            _config = config;
            _tutorialManager = tutorialManager;
            _feedbackManager = feedbackManager;
            _gameStateManager = gameStateManager;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _originalScale = _pieceView.transform.localScale.x;
            _originalPosition = _pieceView.RectTransform.anchoredPosition;
            _originalParent = _pieceView.transform.parent;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_gameStateManager != null && !_gameStateManager.IsInputAllowed)
            {
                eventData.pointerDrag = null;
                return;
            }

            _isDragging = true;
            _gameStateManager?.TransitionTo(GameState.Dragging);

            _pieceView.transform.SetParent(_canvasRect, true);
            _pieceView.transform.SetAsLastSibling();

            _pieceView.transform.DOKill();
            _pieceView.transform.DOScale(DragScale, PickupDuration).SetEase(Ease.OutBack).SetLink(_pieceView.gameObject);

            _dragOffset = new Vector2(0, DragOffsetY);

            _feedbackManager?.PlayPiecePickupEffect(_pieceView.transform);
            _tutorialManager?.OnPiecePickedUp();
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

            if (_feedbackManager != null && _currentMergeHoverCells.Count > 0)
            {
                _feedbackManager.StopMergeHover(_currentMergeHoverCells);
                _currentMergeHoverCells.Clear();
            }

            var boardPos = GetBoardPosition();
            var forcedPos = _tutorialManager?.ForcedPlacement;

            if (forcedPos.HasValue)
            {
                if (boardPos.HasValue && boardPos.Value == forcedPos.Value && CanPlace(boardPos.Value))
                {
                    _gameStateManager?.TransitionTo(GameState.Processing);
                    GameEvents.PiecePlaced(_pieceView, boardPos.Value);
                }
                else
                {
                    _gameStateManager?.TransitionTo(_tutorialManager != null && _tutorialManager.IsActive ? GameState.Tutorial : GameState.Idle);
                    ReturnToTray();
                }
            }
            else if (boardPos.HasValue && CanPlace(boardPos.Value))
            {
                _gameStateManager?.TransitionTo(GameState.Processing);
                GameEvents.PiecePlaced(_pieceView, boardPos.Value);
            }
            else
            {
                _gameStateManager?.TransitionTo(GameState.Idle);
                ReturnToTray();
                GameEvents.PieceReleased(_pieceView);
            }

            ClearDragHighlight();
        }

        private void UpdateBoardHighlight()
        {
            ClearDragHighlight();

            var boardPos = GetBoardPosition();
            if (!boardPos.HasValue) return;

            bool canPlace = CanPlace(boardPos.Value);

            if (!canPlace)
            {
                HighlightPlacement(boardPos.Value, HighlightType.Invalid);
                return;
            }

            HighlightPlacement(boardPos.Value, HighlightType.Placement);
            var mergeCells = HighlightMerges(boardPos.Value);
            HighlightLineClear(boardPos.Value);

            if (_feedbackManager != null)
            {
                bool sameCells = AreSameCells(_currentMergeHoverCells, mergeCells);

                if (!sameCells)
                {
                    if (_currentMergeHoverCells.Count > 0)
                    {
                        _feedbackManager.StopMergeHover(_currentMergeHoverCells);
                    }

                    _currentMergeHoverCells.Clear();

                    if (mergeCells != null && mergeCells.Count > 0)
                    {
                        _currentMergeHoverCells.AddRange(mergeCells);
                        _feedbackManager.StartMergeHover(_currentMergeHoverCells);
                    }
                }
            }
        }

        private void HighlightPlacement(Vector2Int boardPos, HighlightType type)
        {
            var positions = _pieceView.Model.Positions;

            for (int i = 0; i < positions.Length; i++)
            {
                int row = boardPos.x + positions[i].x;
                int col = boardPos.y + positions[i].y;

                var cellView = _boardView.GetCellView(row, col);
                if (cellView != null)
                {
                    cellView.SetHighlight(type);
                }
            }
        }

        private List<CellView> HighlightMerges(Vector2Int boardPos)
        {
            var model = _boardManager.Model;
            var pieceModel = _pieceView.Model;
            _placedSet.Clear();
            _mergeCellCache.Clear();

            for (int i = 0; i < pieceModel.CellCount; i++)
            {
                _placedSet.Add(new Vector2Int(
                    boardPos.x + pieceModel.Positions[i].x,
                    boardPos.y + pieceModel.Positions[i].y
                ));
            }

            for (int i = 0; i < pieceModel.CellCount; i++)
            {
                int cellRow = boardPos.x + pieceModel.Positions[i].x;
                int cellCol = boardPos.y + pieceModel.Positions[i].y;
                int value = pieceModel.GetValueAt(i);

                for (int d = 0; d < GameConstants.MergeDirections.Length; d++)
                {
                    int nRow = cellRow + GameConstants.MergeDirections[d].x;
                    int nCol = cellCol + GameConstants.MergeDirections[d].y;

                    if (_placedSet.Contains(new Vector2Int(nRow, nCol))) continue;

                    var neighbor = model.GetCell(nRow, nCol);
                    if (neighbor != null && !neighbor.IsEmpty && neighbor.Value == value)
                    {
                        var cellView = _boardView.GetCellView(nRow, nCol);
                        if (cellView != null)
                        {
                            cellView.SetHighlight(HighlightType.Merge);
                            _mergeCellCache.Add(cellView);
                        }
                    }
                }
            }

            return _mergeCellCache;
        }

        private bool AreSameCells(List<CellView> a, List<CellView> b)
        {
            if (a.Count == 0 && (b == null || b.Count == 0)) return true;
            if (b == null || a.Count != b.Count) return false;
            for (int i = 0; i < a.Count; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        private void HighlightLineClear(Vector2Int boardPos)
        {
            var model = _boardManager.Model;
            var pieceModel = _pieceView.Model;

            _occupiedAfterPlace.Clear();

            // Collect currently occupied cells
            for (int r = 0; r < model.Rows; r++)
            {
                for (int c = 0; c < model.Columns; c++)
                {
                    if (!model.IsCellEmpty(r, c))
                        _occupiedAfterPlace.Add(new Vector2Int(r, c));
                }
            }

            // Add piece cells
            for (int i = 0; i < pieceModel.CellCount; i++)
            {
                _occupiedAfterPlace.Add(new Vector2Int(
                    boardPos.x + pieceModel.Positions[i].x,
                    boardPos.y + pieceModel.Positions[i].y
                ));
            }

            // Check rows
            for (int r = 0; r < model.Rows; r++)
            {
                bool full = true;
                for (int c = 0; c < model.Columns; c++)
                {
                    if (!_occupiedAfterPlace.Contains(new Vector2Int(r, c)))
                    {
                        full = false;
                        break;
                    }
                }

                if (full)
                {
                    for (int c = 0; c < model.Columns; c++)
                    {
                        _boardView.GetCellView(r, c)?.SetHighlight(HighlightType.LineClear);
                    }
                }
            }

            // Check columns
            for (int c = 0; c < model.Columns; c++)
            {
                bool full = true;
                for (int r = 0; r < model.Rows; r++)
                {
                    if (!_occupiedAfterPlace.Contains(new Vector2Int(r, c)))
                    {
                        full = false;
                        break;
                    }
                }

                if (full)
                {
                    for (int r = 0; r < model.Rows; r++)
                    {
                        _boardView.GetCellView(r, c)?.SetHighlight(HighlightType.LineClear);
                    }
                }
            }
        }

        private void ClearDragHighlight()
        {
            var cellViews = _boardView.CellViews;
            for (int r = 0; r < cellViews.GetLength(0); r++)
            {
                for (int c = 0; c < cellViews.GetLength(1); c++)
                {
                    var h = cellViews[r, c];
                    if (h.CurrentHighlight != HighlightType.TutorialTarget)
                        h.SetHighlight(HighlightType.None);
                }
            }
        }

        private Vector2Int? GetBoardPosition()
        {
            var positions = _pieceView.Model.Positions;
            if (positions.Length == 0) return null;

            var boardRect = _boardRect;
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
            return _boardManager.Model.CanFitPiece(_pieceView.Model.Positions, boardPos.x, boardPos.y);
        }

        private void ReturnToTray()
        {
            _pieceView.transform.SetParent(_originalParent, true);
            _pieceView.RectTransform.DOKill();
            _pieceView.transform.DOKill();
            _pieceView.RectTransform.DOAnchorPos(_originalPosition, ReturnDuration).SetEase(Ease.OutCubic).SetLink(_pieceView.gameObject);
            _pieceView.transform.DOScale(_originalScale, ReturnDuration).SetEase(Ease.OutCubic).SetLink(_pieceView.gameObject);
        }
    }
}
