using System.Collections.Generic;
using UnityEngine;
using NumbersBlast.Board;
using NumbersBlast.Core;
using NumbersBlast.Piece;

namespace NumbersBlast.Gameplay
{
    /// <summary>
    /// Resolves merges by iterating placed cells, absorbing same-value neighbors, and chaining subsequent merges.
    /// </summary>
    public class MergeResolver : IMergeResolver
    {
        private readonly HashSet<Vector2Int> _placedPositions = new();
        private readonly List<Vector2Int> _neighborMatches = new(4);
        private readonly List<MergeEvent> _mergeEvents = new(8);
        private readonly List<Vector2Int> _cellsToCheck = new(8);
        private readonly List<Vector2Int> _nextCheck = new(8);

        /// <summary>
        /// Finds and executes all merges triggered by placing a piece, including chain merges.
        /// </summary>
        public List<MergeEvent> Resolve(BoardModel model, PieceModel pieceModel, Vector2Int boardPos)
        {
            _mergeEvents.Clear();
            _placedPositions.Clear();
            _cellsToCheck.Clear();
            _nextCheck.Clear();

            for (int i = 0; i < pieceModel.CellCount; i++)
            {
                var pos = new Vector2Int(
                    boardPos.x + pieceModel.Positions[i].x,
                    boardPos.y + pieceModel.Positions[i].y
                );
                _cellsToCheck.Add(pos);
                _placedPositions.Add(pos);
            }

            var cellsToCheck = _cellsToCheck;
            var nextCheck = _nextCheck;

            bool merged = true;
            bool isFirstPass = true;
            int maxIterations = 100;

            while (merged && maxIterations-- > 0)
            {
                merged = false;
                nextCheck.Clear();

                for (int i = 0; i < cellsToCheck.Count; i++)
                {
                    var cell = cellsToCheck[i];
                    var cellData = model.GetCell(cell.x, cell.y);
                    if (cellData == null || cellData.IsEmpty) continue;

                    var mergedNeighbors = FindMatchingNeighbors(model, cell, cellData.Value, isFirstPass);

                    if (mergedNeighbors.Count > 0)
                    {
                        int sum = cellData.Value;
                        var absorbedPositions = new List<Vector2Int>(mergedNeighbors.Count);

                        for (int j = 0; j < mergedNeighbors.Count; j++)
                        {
                            var neighbor = model.GetCell(mergedNeighbors[j].x, mergedNeighbors[j].y);
                            sum += neighbor.Value;
                            neighbor.Clear();
                            absorbedPositions.Add(mergedNeighbors[j]);
                        }

                        cellData.SetValue(sum);
                        merged = true;
                        nextCheck.Add(cell);

                        _mergeEvents.Add(new MergeEvent
                        {
                            TargetPos = cell,
                            AbsorbedPositions = absorbedPositions,
                            IsChain = !isFirstPass
                        });
                    }
                }

                // Swap references so cellsToCheck points to the freshly built list
                (cellsToCheck, nextCheck) = (nextCheck, cellsToCheck);
                isFirstPass = false;
            }

            return new List<MergeEvent>(_mergeEvents);
        }

        private List<Vector2Int> FindMatchingNeighbors(BoardModel model, Vector2Int pos, int value, bool excludePlaced)
        {
            _neighborMatches.Clear();
            var matches = _neighborMatches;

            for (int i = 0; i < GameConstants.MergeDirections.Length; i++)
            {
                int row = pos.x + GameConstants.MergeDirections[i].x;
                int col = pos.y + GameConstants.MergeDirections[i].y;

                if (excludePlaced && _placedPositions.Contains(new Vector2Int(row, col)))
                    continue;

                var neighbor = model.GetCell(row, col);
                if (neighbor != null && !neighbor.IsEmpty && neighbor.Value == value)
                {
                    matches.Add(new Vector2Int(row, col));
                }
            }

            return matches;
        }
    }
}
