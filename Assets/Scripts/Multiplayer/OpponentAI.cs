using System.Collections.Generic;
using UnityEngine;
using NumbersBlast.Board;
using NumbersBlast.Data;
using NumbersBlast.Piece;

namespace NumbersBlast.Multiplayer
{
    public class OpponentAI
    {
        private readonly MultiplayerConfig _config;

        private static readonly Vector2Int[] MergeDirections =
        {
            new(-1, 0), new(1, 0), new(0, -1), new(0, 1)
        };

        public OpponentAI(MultiplayerConfig config)
        {
            _config = config;
        }

        public OpponentMove CalculateMove(BoardModel board, PieceView[] trayPieces)
        {
            var bestMove = new OpponentMove();
            float bestScore = -1f;
            var allMoves = new List<OpponentMove>();

            for (int p = 0; p < trayPieces.Length; p++)
            {
                if (trayPieces[p] == null) continue;

                var piece = trayPieces[p].Model;
                for (int r = 0; r < board.Rows; r++)
                {
                    for (int c = 0; c < board.Columns; c++)
                    {
                        if (!CanFit(board, piece, r, c)) continue;

                        float score = EvaluateMove(board, piece, r, c);
                        var move = new OpponentMove
                        {
                            PieceIndex = p,
                            BoardPosition = new Vector2Int(r, c),
                            Score = score
                        };

                        allMoves.Add(move);

                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestMove = move;
                        }
                    }
                }
            }

            if (allMoves.Count == 0)
                return new OpponentMove { IsValid = false };

            // Skill level: chance to pick best vs random
            bool makeMistake = Random.value < _config.MistakeChance;

            if (makeMistake && allMoves.Count > 1)
            {
                // Pick a random non-best move
                allMoves.Remove(bestMove);
                bestMove = allMoves[Random.Range(0, allMoves.Count)];
            }

            bestMove.IsValid = true;
            return bestMove;
        }

        public OpponentMove CalculateDecoyMove(BoardModel board, PieceView[] trayPieces)
        {
            // Pick a random valid position for hesitation/cancel behavior
            for (int p = 0; p < trayPieces.Length; p++)
            {
                if (trayPieces[p] == null) continue;

                var piece = trayPieces[p].Model;
                var positions = new List<Vector2Int>();

                for (int r = 0; r < board.Rows; r++)
                {
                    for (int c = 0; c < board.Columns; c++)
                    {
                        if (CanFit(board, piece, r, c))
                            positions.Add(new Vector2Int(r, c));
                    }
                }

                if (positions.Count > 0)
                {
                    return new OpponentMove
                    {
                        PieceIndex = p,
                        BoardPosition = positions[Random.Range(0, positions.Count)],
                        IsValid = true
                    };
                }
            }

            return new OpponentMove { IsValid = false };
        }

        private float EvaluateMove(BoardModel board, PieceModel piece, int startRow, int startCol)
        {
            float score = 0;

            // Score for merges
            int mergeCount = 0;
            for (int i = 0; i < piece.CellCount; i++)
            {
                int r = startRow + piece.Positions[i].x;
                int c = startCol + piece.Positions[i].y;
                int value = piece.GetValueAt(i);

                for (int d = 0; d < MergeDirections.Length; d++)
                {
                    int nr = r + MergeDirections[d].x;
                    int nc = c + MergeDirections[d].y;

                    var neighbor = board.GetCell(nr, nc);
                    if (neighbor != null && !neighbor.IsEmpty && neighbor.Value == value)
                        mergeCount++;
                }
            }

            score += mergeCount * 10f;

            // Score for line clear potential
            var occupiedAfterPlace = new HashSet<Vector2Int>();
            for (int r = 0; r < board.Rows; r++)
                for (int c = 0; c < board.Columns; c++)
                    if (!board.IsCellEmpty(r, c))
                        occupiedAfterPlace.Add(new Vector2Int(r, c));

            for (int i = 0; i < piece.CellCount; i++)
            {
                occupiedAfterPlace.Add(new Vector2Int(
                    startRow + piece.Positions[i].x,
                    startCol + piece.Positions[i].y
                ));
            }

            for (int r = 0; r < board.Rows; r++)
            {
                bool full = true;
                for (int c = 0; c < board.Columns; c++)
                {
                    if (!occupiedAfterPlace.Contains(new Vector2Int(r, c)))
                    {
                        full = false;
                        break;
                    }
                }
                if (full) score += 50f;
            }

            for (int c = 0; c < board.Columns; c++)
            {
                bool full = true;
                for (int r = 0; r < board.Rows; r++)
                {
                    if (!occupiedAfterPlace.Contains(new Vector2Int(r, c)))
                    {
                        full = false;
                        break;
                    }
                }
                if (full) score += 50f;
            }

            // Small bonus for center placement
            float centerR = board.Rows * 0.5f;
            float centerC = board.Columns * 0.5f;
            float distToCenter = Mathf.Abs(startRow - centerR) + Mathf.Abs(startCol - centerC);
            score += Mathf.Max(0, 5f - distToCenter);

            return score;
        }

        private bool CanFit(BoardModel board, PieceModel piece, int startRow, int startCol)
        {
            for (int i = 0; i < piece.CellCount; i++)
            {
                int r = startRow + piece.Positions[i].x;
                int c = startCol + piece.Positions[i].y;

                if (!board.IsInBounds(r, c)) return false;
                if (!board.IsCellEmpty(r, c)) return false;
            }
            return true;
        }
    }

    public struct OpponentMove
    {
        public int PieceIndex;
        public Vector2Int BoardPosition;
        public float Score;
        public bool IsValid;
    }
}
