using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using NumbersBlast.Board;
using NumbersBlast.Core;
using NumbersBlast.Data;
using NumbersBlast.Piece;

namespace NumbersBlast.Multiplayer
{
    public class OpponentVisualPlayer : MonoBehaviour
    {
        private const float PieceSelectScale = 1.15f;
        private const float PieceSelectDuration = 0.2f;
        private const float PieceDeselectDuration = 0.15f;
        private const float PieceScaleResetDuration = 0.1f;
        private const float DecoyHoldDuration = 0.5f;
        private const float PostDeselectPause = 0.3f;
        private const float PostSelectPause = 0.3f;
        private const float PostPlaceWait = 1.5f;
        private const float GhostAlpha = 0.6f;
        private const int GhostTextFontSize = 24;

        private MultiplayerConfig _config;
        private BoardView _boardView;
        private PieceTray _pieceTray;
        private BoardManager _boardManager;
        private OpponentAI _ai;
        private BoardConfig _boardConfig;

        private GameObject _ghostPiece;
        private CanvasGroup _ghostCanvasGroup;
        private readonly List<GameObject> _ghostCellPool = new(8);
        private Vector3 _selectedPieceOriginalScale;
        private PieceView _selectedPiece;
        private CancellationTokenSource _cts;

        public void Initialize(MultiplayerConfig config, BoardView boardView, PieceTray pieceTray,
            BoardManager boardManager, OpponentAI ai, BoardConfig boardConfig)
        {
            _config = config;
            _boardView = boardView;
            _pieceTray = pieceTray;
            _boardManager = boardManager;
            _ai = ai;
            _boardConfig = boardConfig;
        }

        public void PerformTurn(Action onComplete)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            PlayTurnAsync(onComplete, _cts.Token).Forget();
        }

        public void CancelTurn()
        {
            _cts?.Cancel();
            ClearHighlights();
            HideGhost();
            if (_selectedPiece != null)
            {
                _selectedPiece.transform.localScale = _selectedPieceOriginalScale;
                _selectedPiece = null;
            }
        }

        private async UniTaskVoid PlayTurnAsync(Action onComplete, CancellationToken token)
        {
            var trayPieces = _pieceTray.GetRemainingPieces();
            if (trayPieces == null) { onComplete?.Invoke(); return; }

            var finalMove = _ai.CalculateMove(_boardManager.Model, trayPieces);
            if (!finalMove.IsValid) { onComplete?.Invoke(); return; }

            // Phase 1: Think
            float thinkTime = UnityEngine.Random.Range(_config.MinThinkTime, _config.MaxThinkTime);
            await UniTask.Delay((int)(thinkTime * 1000), cancellationToken: token);

            // Phase 2: Maybe try wrong piece (hesitation)
            bool willHesitate = UnityEngine.Random.value < _config.HesitationChance;
            if (willHesitate)
            {
                var decoyMove = _ai.CalculateDecoyMove(_boardManager.Model, trayPieces);
                if (decoyMove.IsValid && decoyMove.PieceIndex != finalMove.PieceIndex)
                    await AnimatePickAndCancelAsync(trayPieces, decoyMove, token);
            }

            // Phase 3: Select real piece
            _selectedPiece = trayPieces[finalMove.PieceIndex];
            var selectedPiece = _selectedPiece;
            if (selectedPiece == null) { onComplete?.Invoke(); return; }

            await AnimateSelectPieceAsync(selectedPiece, token);

            // Phase 4: Wander
            var piece = selectedPiece.Model;
            int wanderCount = UnityEngine.Random.Range(1, 4);
            var board = _boardManager.Model;

            for (int w = 0; w < wanderCount; w++)
            {
                if (token.IsCancellationRequested) return;

                // Wander near final position (within 3 cells) for natural feel
                int maxOffset = 3;
                int offsetR = UnityEngine.Random.Range(-maxOffset, maxOffset + 1);
                int offsetC = UnityEngine.Random.Range(-maxOffset, maxOffset + 1);

                var wanderPos = new Vector2Int(
                    Mathf.Clamp(finalMove.BoardPosition.x + offsetR, 0, board.Rows - 1),
                    Mathf.Clamp(finalMove.BoardPosition.y + offsetC, 0, board.Columns - 1));

                // Skip if same as final
                if (wanderPos == finalMove.BoardPosition) continue;

                bool canPlace = CanFitAt(piece, wanderPos);
                await AnimateHoverAsync(wanderPos, canPlace, token);

                float wanderPause = UnityEngine.Random.Range(_config.MinWanderPause, _config.MaxWanderPause);
                await UniTask.Delay((int)(wanderPause * 1000), cancellationToken: token);
                ClearHighlights();
            }

            // Phase 5: Maybe cancel
            bool willCancel = UnityEngine.Random.value < _config.CancelChance;
            if (willCancel)
            {
                // Cancel position: nearby but different from final
                int cancelOffR = UnityEngine.Random.Range(-2, 3);
                int cancelOffC = UnityEngine.Random.Range(-2, 3);
                var cancelPos = new Vector2Int(
                    Mathf.Clamp(finalMove.BoardPosition.x + cancelOffR, 0, board.Rows - 1),
                    Mathf.Clamp(finalMove.BoardPosition.y + cancelOffC, 0, board.Columns - 1));

                if (cancelPos != finalMove.BoardPosition)
                {
                    bool canPlaceCancel = CanFitAt(piece, cancelPos);
                    await AnimateHoverAsync(cancelPos, canPlaceCancel, token);
                    float hesitateTime = UnityEngine.Random.Range(_config.MinHesitationTime, _config.MaxHesitationTime);
                    await UniTask.Delay((int)(hesitateTime * 1000), cancellationToken: token);
                    ClearHighlights();
                }
            }

            // Phase 6: Final position
            await AnimateHoverAsync(finalMove.BoardPosition, true, token);
            float hoverTime = UnityEngine.Random.Range(_config.MinHoverTime, _config.MaxHoverTime);
            await UniTask.Delay((int)(hoverTime * 1000), cancellationToken: token);

            // Phase 7: Place
            if (token.IsCancellationRequested) return;

            ClearHighlights();
            HideGhost();

            if (_selectedPiece != null && _selectedPiece.gameObject != null)
            {
                _selectedPiece.transform.DOKill();
                _selectedPiece.transform.DOScale(_selectedPieceOriginalScale, PieceScaleResetDuration)
                    .SetLink(_selectedPiece.gameObject);
            }

            if (selectedPiece != null && selectedPiece.gameObject != null)
                GameEvents.PiecePlaced(selectedPiece, finalMove.BoardPosition);

            await UniTask.Delay((int)(PostPlaceWait * 1000), cancellationToken: token);

            _selectedPiece = null;
            onComplete?.Invoke();
        }

        private async UniTask AnimatePickAndCancelAsync(PieceView[] trayPieces, OpponentMove decoyMove, CancellationToken token)
        {
            var decoyPiece = trayPieces[decoyMove.PieceIndex];
            if (decoyPiece == null) return;

            var originalScale = decoyPiece.transform.localScale;

            decoyPiece.transform.DOKill();
            decoyPiece.transform.DOScale(originalScale * PieceSelectScale, PieceSelectDuration)
                .SetEase(Ease.OutBack).SetLink(decoyPiece.gameObject);

            await UniTask.Delay((int)(DecoyHoldDuration * 1000), cancellationToken: token);

            float hesitateTime = UnityEngine.Random.Range(_config.MinHesitationTime, _config.MaxHesitationTime);
            await UniTask.Delay((int)(hesitateTime * 1000), cancellationToken: token);

            decoyPiece.transform.DOKill();
            decoyPiece.transform.DOScale(originalScale, PieceDeselectDuration)
                .SetEase(Ease.InQuad).SetLink(decoyPiece.gameObject);

            await UniTask.Delay((int)(PostDeselectPause * 1000), cancellationToken: token);
        }

        private async UniTask AnimateSelectPieceAsync(PieceView piece, CancellationToken token)
        {
            _selectedPieceOriginalScale = piece.transform.localScale;

            piece.transform.DOKill();
            piece.transform.DOScale(_selectedPieceOriginalScale * PieceSelectScale, PieceSelectDuration)
                .SetEase(Ease.OutBack).SetLink(piece.gameObject);

            await UniTask.Delay((int)(PostSelectPause * 1000), cancellationToken: token);
            CreateGhost(piece);
        }

        private async UniTask AnimateHoverAsync(Vector2Int boardPos, bool canPlace, CancellationToken token)
        {
            if (_ghostPiece == null) return;

            var cellView = _boardView.GetCellView(boardPos.x, boardPos.y);
            if (cellView == null) return;

            Vector3 targetPos = cellView.RectTransform.position;

            _ghostPiece.transform.DOKill();
            _ghostPiece.transform.DOMove(targetPos, _config.MoveSpeed)
                .SetEase(Ease.InOutCubic).SetLink(_ghostPiece);

            await UniTask.Delay((int)(_config.MoveSpeed * 1000), cancellationToken: token);
            HighlightBoardCells(boardPos, canPlace);
        }

        private void CreateGhost(PieceView piece)
        {
            HideGhost();
            EnsureGhostRoot();

            var model = piece.Model;
            var theme = _boardConfig.Theme;
            float cellSize = _boardView.CellSize;

            EnsureGhostCells(model.CellCount);

            for (int i = 0; i < model.CellCount; i++)
            {
                var cellGo = _ghostCellPool[i];
                cellGo.SetActive(true);

                var rect = cellGo.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(cellSize, cellSize);
                rect.anchoredPosition = new Vector2(
                    model.Positions[i].y * cellSize,
                    -model.Positions[i].x * cellSize);

                var image = cellGo.GetComponent<Image>();
                var visual = theme.GetBlockVisual(model.GetValueAt(i));
                image.color = visual.Color;
                if (theme.BlockSprite != null)
                    image.sprite = theme.BlockSprite;

                var text = cellGo.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                text.text = model.GetValueAt(i).ToString();
            }

            // Hide unused cells
            for (int i = model.CellCount; i < _ghostCellPool.Count; i++)
                _ghostCellPool[i].SetActive(false);

            _ghostPiece.SetActive(true);
            _ghostPiece.transform.position = piece.transform.position;
        }

        private void EnsureGhostRoot()
        {
            if (_ghostPiece != null) return;

            _ghostPiece = new GameObject("OpponentGhost", typeof(RectTransform), typeof(CanvasGroup));
            _ghostPiece.transform.SetParent(_boardView.transform.parent, false);
            _ghostCanvasGroup = _ghostPiece.GetComponent<CanvasGroup>();
            _ghostCanvasGroup.alpha = GhostAlpha;
        }

        private void EnsureGhostCells(int count)
        {
            while (_ghostCellPool.Count < count)
            {
                var cellGo = new GameObject($"GhostCell_{_ghostCellPool.Count}", typeof(RectTransform), typeof(Image));
                cellGo.transform.SetParent(_ghostPiece.transform, false);

                var textGo = new GameObject("Value", typeof(RectTransform), typeof(TMPro.TextMeshProUGUI));
                textGo.transform.SetParent(cellGo.transform, false);
                var textRect = textGo.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.sizeDelta = Vector2.zero;
                var text = textGo.GetComponent<TMPro.TextMeshProUGUI>();
                text.alignment = TMPro.TextAlignmentOptions.Center;
                text.fontSize = GhostTextFontSize;
                text.color = Color.white;

                cellGo.SetActive(false);
                _ghostCellPool.Add(cellGo);
            }
        }

        private void HideGhost()
        {
            if (_ghostPiece != null)
                _ghostPiece.SetActive(false);
        }

        private void HighlightBoardCells(Vector2Int boardPos, bool canPlace)
        {
            ClearHighlights();

            if (_selectedPiece == null) return;
            var piece = _selectedPiece.Model;
            var highlightType = canPlace ? HighlightType.Placement : HighlightType.Invalid;

            for (int i = 0; i < piece.CellCount; i++)
            {
                int r = boardPos.x + piece.Positions[i].x;
                int c = boardPos.y + piece.Positions[i].y;

                var cell = _boardView.GetCellView(r, c);
                if (cell != null)
                    cell.SetHighlight(highlightType);
            }
        }

        private void ClearHighlights()
        {
            var cellViews = _boardView.CellViews;
            if (cellViews == null) return;
            for (int r = 0; r < cellViews.GetLength(0); r++)
                for (int c = 0; c < cellViews.GetLength(1); c++)
                    if (cellViews[r, c].CurrentHighlight == HighlightType.Placement ||
                        cellViews[r, c].CurrentHighlight == HighlightType.Invalid)
                        cellViews[r, c].SetHighlight(HighlightType.None);
        }

        private bool CanFitAt(PieceModel piece, Vector2Int pos)
        {
            var model = _boardManager.Model;
            for (int i = 0; i < piece.CellCount; i++)
            {
                int r = pos.x + piece.Positions[i].x;
                int c = pos.y + piece.Positions[i].y;
                if (!model.IsInBounds(r, c) || !model.IsCellEmpty(r, c)) return false;
            }
            return true;
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            if (_ghostPiece != null)
                Destroy(_ghostPiece);
        }
    }
}
