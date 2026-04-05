using UnityEngine;
using NumbersBlast.Board;
using NumbersBlast.Core;
using NumbersBlast.Data;
using NumbersBlast.Feedback;
using NumbersBlast.Input;
using NumbersBlast.StateMachine;
using NumbersBlast.Tutorial;

namespace NumbersBlast.Piece
{
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
        private IFeedbackManager _feedbackManager;
        private GameStateManager _gameStateManager;
        private RectTransform[] _slotRects;

        // L-08: 8 parameters acknowledged. MonoBehaviour cannot use constructor injection,
        // so all dependencies must be passed via Initialize. Refactoring to a config object
        // would add complexity without meaningful benefit here.
        public void Initialize(BoardConfig config, float cellSize, Canvas canvas, BoardView boardView, BoardManager boardManager, TutorialManager tutorialManager = null, IFeedbackManager feedbackManager = null, GameStateManager gameStateManager = null)
        {
            _config = config;
            _cellSize = cellSize;
            _canvas = canvas;
            _boardView = boardView;
            _boardManager = boardManager;
            _tutorialManager = tutorialManager;
            _feedbackManager = feedbackManager;
            _gameStateManager = gameStateManager;

            _slotRects = new RectTransform[_pieceSlots.Length];
            for (int i = 0; i < _pieceSlots.Length; i++)
                _slotRects[i] = _pieceSlots[i].GetComponent<RectTransform>();
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
                _pieceViews[i] = SpawnSinglePiece(model, i);
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

        private float CalculateFitScale(PieceModel model, RectTransform slotRect)
        {
            int maxRow = 0, maxCol = 0;
            for (int i = 0; i < model.Positions.Length; i++)
            {
                if (model.Positions[i].x + 1 > maxRow) maxRow = model.Positions[i].x + 1;
                if (model.Positions[i].y + 1 > maxCol) maxCol = model.Positions[i].y + 1;
            }

            float pieceWidth = maxCol * _cellSize;
            float pieceHeight = maxRow * _cellSize;

            if (pieceWidth <= 0 || pieceHeight <= 0) return GameConstants.MaxPieceTrayScale;

            float scaleX = slotRect.sizeDelta.x / pieceWidth;
            float scaleY = slotRect.sizeDelta.y / pieceHeight;

            return Mathf.Min(scaleX, scaleY, GameConstants.MaxPieceTrayScale);
        }

        public void SpawnTutorialPiece(PieceModel model)
        {
            ClearAll();
            _pieceViews[0] = SpawnSinglePiece(model, 0);
        }

        private PieceView SpawnSinglePiece(PieceModel model, int slotIndex)
        {
            var pieceGo = Instantiate(_piecePrefab, _pieceSlots[slotIndex]);
            var pieceView = pieceGo.GetComponent<PieceView>();
            pieceView.Initialize(model, _config, _cellSize);

            // TODO: PieceDragHandler should be pre-attached on the piece prefab instead of added as fallback.
            var dragHandler = pieceGo.GetComponent<PieceDragHandler>();
            if (dragHandler == null)
                dragHandler = pieceGo.AddComponent<PieceDragHandler>();
            dragHandler.Initialize(pieceView, _canvas, _boardView, _boardManager, _config, _tutorialManager, _feedbackManager, _gameStateManager);

            var slotRect = _slotRects[slotIndex];
            float scale = CalculateFitScale(model, slotRect);
            pieceView.SetScale(scale);

            return pieceView;
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
                    return _slotRects[i];
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
                GameEvents.TrayRefilled();
            }
        }
    }
}
