using System.Collections.Generic;
using UnityEngine;
using NumbersBlast.Audio;
using NumbersBlast.Board;
using NumbersBlast.Core;
using NumbersBlast.Feedback;
using NumbersBlast.Piece;
using NumbersBlast.StateMachine;

namespace NumbersBlast.Gameplay
{
    public class PlacementHandler
    {
        private readonly BoardManager _boardManager;
        private readonly MergeResolver _mergeResolver;
        private readonly LineClearResolver _lineClearResolver;
        private readonly PieceTray _pieceTray;
        private readonly BoardView _boardView;
        private readonly FeedbackManager _feedbackManager;
        private readonly GameStateManager _gameStateManager;
        private readonly AudioManager _audioManager;

        public event System.Action OnPlacementComplete;

        public PlacementHandler(BoardManager boardManager, MergeResolver mergeResolver, LineClearResolver lineClearResolver, PieceTray pieceTray, BoardView boardView, FeedbackManager feedbackManager, GameStateManager gameStateManager, AudioManager audioManager)
        {
            _boardManager = boardManager;
            _mergeResolver = mergeResolver;
            _lineClearResolver = lineClearResolver;
            _pieceTray = pieceTray;
            _boardView = boardView;
            _feedbackManager = feedbackManager;
            _gameStateManager = gameStateManager;
            _audioManager = audioManager;
        }

        public void Enable()
        {
            GameEvents.OnPiecePlaced += HandlePiecePlaced;
        }

        public void Disable()
        {
            GameEvents.OnPiecePlaced -= HandlePiecePlaced;
        }

        private void HandlePiecePlaced(PieceView pieceView, Vector2Int boardPos)
        {
            var model = _boardManager.Model;
            var pieceModel = pieceView.Model;

            PlaceCells(model, pieceModel, boardPos);
            DG.Tweening.DOTween.Kill(pieceView.transform);
            Object.Destroy(pieceView.gameObject);
            _pieceTray.RemovePiece(pieceView);

            // Merge (data only, no visual yet)
            var mergeResult = _mergeResolver.Resolve(model, pieceModel, boardPos);

            // Single refresh after place + merge data is resolved
            _boardView.RefreshAll();

            // Place feedback
            var placedCells = GetCellViews(pieceModel, boardPos);
            _feedbackManager.PlayPlaceEffect(placedCells);

            // Merge feedback
            PlayMergeFeedback(mergeResult);

            // Line clear
            var clearResult = _lineClearResolver.Resolve(model);
            if (clearResult.Score > 0)
            {
                _audioManager.PlayLineClear();
                GameEvents.ScoreChanged(clearResult.Score);

                var clearViews = new CellView[clearResult.ClearedPositions.Count];
                for (int i = 0; i < clearResult.ClearedPositions.Count; i++)
                {
                    clearViews[i] = _boardView.GetCellView(clearResult.ClearedPositions[i].x, clearResult.ClearedPositions[i].y);
                }

                _feedbackManager.PlayLineClearEffect(clearViews, () =>
                {
                    if (_gameStateManager.CurrentState == GameState.GameOver) return;

                    _boardView.RefreshAll();
                    CheckGameOver();
                    if (_gameStateManager.CurrentState != GameState.GameOver
                        && _gameStateManager.CurrentState != GameState.Paused)
                    {
                        _gameStateManager.TransitionTo(GameState.Idle);
                        OnPlacementComplete?.Invoke();
                    }
                });
            }
            else
            {
                _boardView.RefreshAll();
                CheckGameOver();
                if (_gameStateManager.CurrentState != GameState.GameOver)
                {
                    _gameStateManager.TransitionTo(GameState.Idle);
                    OnPlacementComplete?.Invoke();
                }
            }
        }

        private void PlayMergeFeedback(List<MergeEvent> mergeResult)
        {
            for (int i = 0; i < mergeResult.Count; i++)
            {
                var merge = mergeResult[i];
                var targetView = _boardView.GetCellView(merge.TargetPos.x, merge.TargetPos.y);
                var absorbedViews = new CellView[merge.AbsorbedPositions.Count];
                for (int j = 0; j < merge.AbsorbedPositions.Count; j++)
                {
                    absorbedViews[j] = _boardView.GetCellView(merge.AbsorbedPositions[j].x, merge.AbsorbedPositions[j].y);
                }

                if (merge.IsChain)
                {
                    _feedbackManager.PlayChainMergeSmash(targetView);
                    _audioManager.PlayChainMerge();
                }
                else
                {
                    _feedbackManager.PlayMergeSmash(targetView, absorbedViews);
                    _audioManager.PlayMerge();
                }
            }
        }

        private CellView[] GetCellViews(PieceModel pieceModel, Vector2Int boardPos)
        {
            var views = new CellView[pieceModel.CellCount];
            for (int i = 0; i < pieceModel.CellCount; i++)
            {
                int row = boardPos.x + pieceModel.Positions[i].x;
                int col = boardPos.y + pieceModel.Positions[i].y;
                views[i] = _boardView.GetCellView(row, col);
            }
            return views;
        }

        private void PlaceCells(BoardModel model, PieceModel pieceModel, Vector2Int boardPos)
        {
            for (int i = 0; i < pieceModel.CellCount; i++)
            {
                int row = boardPos.x + pieceModel.Positions[i].x;
                int col = boardPos.y + pieceModel.Positions[i].y;
                model.GetCell(row, col).SetValue(pieceModel.GetValueAt(i));
            }
        }

        private void CheckGameOver()
        {
            if (!HasValidMoveForTray())
            {
                _gameStateManager.TransitionTo(GameState.GameOver);
                _feedbackManager.PlayGameOverEffect(() => GameEvents.GameOver());
            }
        }

        private bool HasValidMoveForTray()
        {
            var model = _boardManager.Model;
            var remainingPieces = _pieceTray.GetRemainingPieces();

            for (int i = 0; i < remainingPieces.Length; i++)
            {
                if (remainingPieces[i] == null) continue;

                var positions = remainingPieces[i].Model.Positions;

                for (int r = 0; r < model.Rows; r++)
                {
                    for (int c = 0; c < model.Columns; c++)
                    {
                        if (model.CanFitPiece(positions, r, c))
                            return true;
                    }
                }
            }

            return false;
        }

    }
}
