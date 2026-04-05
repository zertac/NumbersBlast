using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class OpponentVisualPlayer : MonoBehaviour
{
    private MultiplayerConfig _config;
    private BoardView _boardView;
    private PieceTray _pieceTray;
    private BoardManager _boardManager;
    private OpponentAI _ai;
    private BoardConfig _boardConfig;

    private GameObject _ghostPiece;
    private Action _onMoveComplete;
    private Vector3 _selectedPieceOriginalScale;
    private PieceView _selectedPiece;

    private const float PieceSelectScale = 1.15f;
    private const float PieceSelectDuration = 0.2f;
    private const float PieceDeselectDuration = 0.15f;
    private const float PieceScaleResetDuration = 0.1f;
    private const float PostDeselectPause = 0.3f;
    private const float PostSelectPause = 0.3f;
    private const float DecoyHoldDuration = 0.5f;
    private const float GhostAlpha = 0.6f;
    private const float PostPlaceWait = 1.5f;
    private const int GhostTextFontSize = 24;

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

    private bool _isCancelled;

    public void PerformTurn(Action onComplete)
    {
        _isCancelled = false;
        _onMoveComplete = onComplete;
        StartCoroutine(PlayTurnSequence());
    }

    public void CancelTurn()
    {
        _isCancelled = true;
        StopAllCoroutines();
        ClearHighlights();
        DestroyGhost();
        if (_selectedPiece != null)
        {
            _selectedPiece.transform.localScale = _selectedPieceOriginalScale;
            _selectedPiece = null;
        }
    }

    private IEnumerator PlayTurnSequence()
    {
        var trayPieces = _pieceTray.GetRemainingPieces();
        if (trayPieces == null) { _onMoveComplete?.Invoke(); yield break; }

        var finalMove = _ai.CalculateMove(_boardManager.Model, trayPieces);

        if (!finalMove.IsValid)
        {
            _onMoveComplete?.Invoke();
            yield break;
        }

        // Phase 1: Think before selecting
        float thinkTime = UnityEngine.Random.Range(_config.MinThinkTime, _config.MaxThinkTime);
        yield return new WaitForSeconds(thinkTime);

        // Phase 2: Maybe try a wrong piece first (hesitation)
        bool willHesitate = UnityEngine.Random.value < _config.HesitationChance;

        if (willHesitate)
        {
            var decoyMove = _ai.CalculateDecoyMove(_boardManager.Model, trayPieces);
            if (decoyMove.IsValid && decoyMove.PieceIndex != finalMove.PieceIndex)
            {
                yield return StartCoroutine(AnimatePickAndCancel(trayPieces, decoyMove));
            }
        }

        // Phase 3: Select the real piece
        _selectedPiece = trayPieces[finalMove.PieceIndex];
        var selectedPiece = _selectedPiece;
        if (selectedPiece == null)
        {
            _onMoveComplete?.Invoke();
            yield break;
        }

        yield return StartCoroutine(AnimateSelectPiece(selectedPiece));

        // Phase 4: Wander around board (mix of valid and invalid spots)
        int wanderCount = UnityEngine.Random.Range(1, 4);
        var piece = selectedPiece.Model;

        for (int w = 0; w < wanderCount; w++)
        {
            // Sometimes pick a random spot (could be invalid) for realism
            bool tryInvalid = UnityEngine.Random.value < _config.InvalidMoveChance;
            Vector2Int wanderPos;

            if (tryInvalid)
            {
                wanderPos = new Vector2Int(
                    UnityEngine.Random.Range(0, _boardManager.Model.Rows),
                    UnityEngine.Random.Range(0, _boardManager.Model.Columns)
                );
            }
            else
            {
                var wanderMove = _ai.CalculateDecoyMove(_boardManager.Model, trayPieces);
                if (!wanderMove.IsValid) continue;
                wanderPos = wanderMove.BoardPosition;
            }

            bool canPlace = CanFitAt(piece, wanderPos);
            yield return StartCoroutine(AnimateHoverPosition(wanderPos, canPlace));

            float wanderPause = UnityEngine.Random.Range(_config.MinWanderPause, _config.MaxWanderPause);
            yield return new WaitForSeconds(wanderPause);

            ClearHighlights();
        }

        // Phase 5: Maybe hover wrong position and cancel
        bool willCancel = UnityEngine.Random.value < _config.CancelChance;

        if (willCancel)
        {
            var decoyMove = _ai.CalculateDecoyMove(_boardManager.Model, trayPieces);
            if (decoyMove.IsValid && decoyMove.BoardPosition != finalMove.BoardPosition)
            {
                yield return StartCoroutine(AnimateHoverPosition(decoyMove.BoardPosition, true));

                float hesitateTime = UnityEngine.Random.Range(_config.MinHesitationTime, _config.MaxHesitationTime);
                yield return new WaitForSeconds(hesitateTime);
                ClearHighlights();
            }
        }

        // Phase 6: Move to final position
        yield return StartCoroutine(AnimateHoverPosition(finalMove.BoardPosition, true));

        float hoverTime = UnityEngine.Random.Range(_config.MinHoverTime, _config.MaxHoverTime);
        yield return new WaitForSeconds(hoverTime);

        // Phase 7: Place
        if (_isCancelled) yield break;

        ClearHighlights();
        DestroyGhost();

        if (_selectedPiece != null && _selectedPiece.gameObject != null)
            _selectedPiece.transform.DOScale(_selectedPieceOriginalScale, PieceScaleResetDuration);

        if (selectedPiece != null && selectedPiece.gameObject != null)
            GameEvents.PiecePlaced(selectedPiece, finalMove.BoardPosition);

        // Wait for placement processing to complete
        yield return new WaitForSeconds(PostPlaceWait);

        _selectedPiece = null;
        _onMoveComplete?.Invoke();
    }

    private IEnumerator AnimatePickAndCancel(PieceView[] trayPieces, OpponentMove decoyMove)
    {
        var decoyPiece = trayPieces[decoyMove.PieceIndex];
        if (decoyPiece == null) yield break;

        var originalScale = decoyPiece.transform.localScale;

        decoyPiece.transform.DOScale(originalScale * PieceSelectScale, PieceSelectDuration).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(DecoyHoldDuration);

        float hesitateTime = UnityEngine.Random.Range(_config.MinHesitationTime, _config.MaxHesitationTime);
        yield return new WaitForSeconds(hesitateTime);

        decoyPiece.transform.DOScale(originalScale, PieceDeselectDuration).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(PostDeselectPause);
    }

    private IEnumerator AnimateSelectPiece(PieceView piece)
    {
        _selectedPieceOriginalScale = piece.transform.localScale;
        piece.transform.DOScale(_selectedPieceOriginalScale * PieceSelectScale, PieceSelectDuration).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(PostSelectPause);

        CreateGhost(piece);
    }

    private IEnumerator AnimateHoverPosition(Vector2Int boardPos, bool canPlace = true)
    {
        if (_ghostPiece == null) yield break;

        var cellView = _boardView.GetCellView(boardPos.x, boardPos.y);
        if (cellView == null) yield break;

        Vector3 targetPos = cellView.RectTransform.position;
        _ghostPiece.transform.DOMove(targetPos, _config.MoveSpeed).SetEase(Ease.InOutCubic);
        yield return new WaitForSeconds(_config.MoveSpeed);

        HighlightBoardCells(boardPos, canPlace);
    }

    private void CreateGhost(PieceView piece)
    {
        DestroyGhost();

        _ghostPiece = new GameObject("OpponentGhost", typeof(RectTransform), typeof(CanvasGroup));
        _ghostPiece.transform.SetParent(_boardView.transform.parent, false);

        var canvasGroup = _ghostPiece.GetComponent<CanvasGroup>();
        canvasGroup.alpha = GhostAlpha;

        // Copy piece cells visually
        var model = piece.Model;
        var theme = _boardConfig.Theme;
        float cellSize = _boardView.CellSize;

        for (int i = 0; i < model.CellCount; i++)
        {
            var cellGo = new GameObject($"Cell_{i}", typeof(RectTransform), typeof(UnityEngine.UI.Image));
            cellGo.transform.SetParent(_ghostPiece.transform, false);

            var rect = cellGo.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(cellSize, cellSize);
            rect.anchoredPosition = new Vector2(
                model.Positions[i].y * cellSize,
                -model.Positions[i].x * cellSize
            );

            var image = cellGo.GetComponent<UnityEngine.UI.Image>();
            var visual = theme.GetBlockVisual(model.GetValueAt(i));
            image.color = visual.Color;
            if (theme.BlockSprite != null)
                image.sprite = theme.BlockSprite;

            var textGo = new GameObject("Value", typeof(RectTransform), typeof(TMPro.TextMeshProUGUI));
            textGo.transform.SetParent(cellGo.transform, false);
            var textRect = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            var text = textGo.GetComponent<TMPro.TextMeshProUGUI>();
            text.text = model.GetValueAt(i).ToString();
            text.alignment = TMPro.TextAlignmentOptions.Center;
            text.fontSize = GhostTextFontSize;
            text.color = Color.white;
        }

        _ghostPiece.transform.position = piece.transform.position;
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

    private void DestroyGhost()
    {
        if (_ghostPiece != null)
        {
            Destroy(_ghostPiece);
            _ghostPiece = null;
        }
    }

    private void OnDestroy()
    {
        DestroyGhost();
    }
}
