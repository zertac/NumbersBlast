using System;
using UnityEngine;
using NumbersBlast.Piece;

namespace NumbersBlast.Core
{
    public static class GameEvents
    {
        public static event Action<PieceView> OnPiecePickedUp;
        public static event Action<PieceView> OnPieceReleased;
        public static event Action<PieceView, Vector2Int> OnPiecePlaced;
        public static event Action<int> OnScoreChanged;
        public static event Action OnGameOver;
        public static event Action OnTrayRefilled;
        public static event Action OnPopupOpened;
        public static event Action OnPopupClosed;

        public static void PiecePickedUp(PieceView piece) => OnPiecePickedUp?.Invoke(piece);
        public static void PieceReleased(PieceView piece) => OnPieceReleased?.Invoke(piece);
        public static void PiecePlaced(PieceView piece, Vector2Int boardPos) => OnPiecePlaced?.Invoke(piece, boardPos);
        public static void ScoreChanged(int score) => OnScoreChanged?.Invoke(score);
        public static void GameOver() => OnGameOver?.Invoke();
        public static void TrayRefilled() => OnTrayRefilled?.Invoke();
        public static void PopupOpened() => OnPopupOpened?.Invoke();
        public static void PopupClosed() => OnPopupClosed?.Invoke();

        public static void ClearAll()
        {
            OnPiecePickedUp = null;
            OnPieceReleased = null;
            OnPiecePlaced = null;
            OnScoreChanged = null;
            OnGameOver = null;
            OnTrayRefilled = null;
            OnPopupOpened = null;
            OnPopupClosed = null;
        }
    }
}
