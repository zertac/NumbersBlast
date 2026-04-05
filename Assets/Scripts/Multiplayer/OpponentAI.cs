using System.Collections.Generic;
using UnityEngine;
using NumbersBlast.Board;
using NumbersBlast.Core;
using NumbersBlast.Data;
using NumbersBlast.Piece;

namespace NumbersBlast.Multiplayer
{
    public class OpponentAI
    {
        private readonly MultiplayerConfig _config;
        private readonly List<OpponentMove> _allMoves = new(64);
        private readonly HashSet<Vector2Int> _occupiedAfterPlace = new(64);

        public OpponentAI(MultiplayerConfig config)
        {
            _config = config;
        }

        public OpponentMove CalculateMove(BoardModel board, PieceView[] trayPieces)
        {
            var bestMove = new OpponentMove();
            float bestScore = -1f;
            _allMoves.Clear();

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

                        _allMoves.Add(move);

                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestMove = move;
                        }
                    }
                }
            }

            if (_allMoves.Count == 0)
                return new OpponentMove { IsValid = false };

            // Skill level: chance to pick best vs random
            bool makeMistake = Random.value < _config.MistakeChance;

            if (makeMistake && _allMoves.Count > 1)
            {
                // Pick a random non-best move
                _allMoves.Remove(bestMove);
                bestMove = _allMoves[Random.Range(0, _allMoves.Count)];
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

                for (int d = 0; d < GameConstants.MergeDirections.Length; d++)
                {
                    int nr = r + GameConstants.MergeDirections[d].x;
                    int nc = c + GameConstants.MergeDirections[d].y;

                    var neighbor = board.GetCell(nr, nc);
                    if (neighbor != null && !neighbor.IsEmpty && neighbor.Value == value)
                        mergeCount++;
                }
            }

            score += mergeCount * _config.MergeWeight;

            // Score for line clear potential
            _occupiedAfterPlace.Clear();
            for (int r = 0; r < board.Rows; r++)
                for (int c = 0; c < board.Columns; c++)
                    if (!board.IsCellEmpty(r, c))
                        _occupiedAfterPlace.Add(new Vector2Int(r, c));

            for (int i = 0; i < piece.CellCount; i++)
            {
                _occupiedAfterPlace.Add(new Vector2Int(
                    startRow + piece.Positions[i].x,
                    startCol + piece.Positions[i].y
                ));
            }

            for (int r = 0; r < board.Rows; r++)
            {
                bool full = true;
                for (int c = 0; c < board.Columns; c++)
                {
                    if (!_occupiedAfterPlace.Contains(new Vector2Int(r, c)))
                    {
                        full = false;
                        break;
                    }
                }
                if (full) score += _config.LineClearWeight;
            }

            for (int c = 0; c < board.Columns; c++)
            {
                bool full = true;
                for (int r = 0; r < board.Rows; r++)
                {
                    if (!_occupiedAfterPlace.Contains(new Vector2Int(r, c)))
                    {
                        full = false;
                        break;
                    }
                }
                if (full) score += _config.LineClearWeight;
            }

            // Small bonus for center placement
            float centerR = board.Rows * 0.5f;
            float centerC = board.Columns * 0.5f;
            float distToCenter = Mathf.Abs(startRow - centerR) + Mathf.Abs(startCol - centerC);
            score += Mathf.Max(0, _config.CenterBonus - distToCenter);

            return score;
        }

        private bool CanFit(BoardModel board, PieceModel piece, int startRow, int startCol)
        {
            return board.CanFitPiece(piece.Positions, startRow, startCol);
        }
    }
}
