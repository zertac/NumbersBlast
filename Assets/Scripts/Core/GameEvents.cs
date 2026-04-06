using System;
using UnityEngine;
using NumbersBlast.Piece;

namespace NumbersBlast.Core
{
    /// <summary>
    /// Central event bus for all gameplay events. Provides static events and raise methods to decouple game systems.
    /// </summary>
    public static class GameEvents
    {
        /// <summary>Raised when a piece is picked up from the tray.</summary>
        public static event Action<PieceView> OnPiecePickedUp;

        /// <summary>Raised when a held piece is released without being placed.</summary>
        public static event Action<PieceView> OnPieceReleased;

        /// <summary>Raised when a piece is successfully placed on the board.</summary>
        public static event Action<PieceView, Vector2Int> OnPiecePlaced;

        /// <summary>Raised when the player's score changes, carrying the new score value.</summary>
        public static event Action<int> OnScoreChanged;

        /// <summary>Raised when no valid moves remain and the game ends.</summary>
        public static event Action OnGameOver;

        /// <summary>Raised when the piece tray is refilled with new pieces.</summary>
        public static event Action OnTrayRefilled;

        /// <summary>Raised when any popup is opened.</summary>
        public static event Action OnPopupOpened;

        /// <summary>Raised when any popup is closed.</summary>
        public static event Action OnPopupClosed;

        public static void PiecePickedUp(PieceView piece) => OnPiecePickedUp?.Invoke(piece);
        public static void PieceReleased(PieceView piece) => OnPieceReleased?.Invoke(piece);
        public static void PiecePlaced(PieceView piece, Vector2Int boardPos) => OnPiecePlaced?.Invoke(piece, boardPos);
        public static void ScoreChanged(int score) => OnScoreChanged?.Invoke(score);
        public static void GameOver() => OnGameOver?.Invoke();
        public static void TrayRefilled() => OnTrayRefilled?.Invoke();
        public static void PopupOpened() => OnPopupOpened?.Invoke();
        public static void PopupClosed() => OnPopupClosed?.Invoke();

        /// <summary>
        /// Unsubscribes all listeners from every event. Call during scene transitions to prevent stale references.
        /// </summary>
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
