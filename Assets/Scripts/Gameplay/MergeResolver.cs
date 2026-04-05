using System.Collections.Generic;
using UnityEngine;

public class MergeResolver
{
    private static readonly Vector2Int[] Directions =
    {
        new(-1, 0), new(1, 0), new(0, -1), new(0, 1)
    };

    private readonly HashSet<Vector2Int> _placedPositions = new();

    public List<MergeEvent> Resolve(BoardModel model, PieceModel pieceModel, Vector2Int boardPos, BoardView boardView)
    {
        var mergeEvents = new List<MergeEvent>();
        _placedPositions.Clear();

        var cellsToCheck = new List<Vector2Int>(pieceModel.CellCount);

        for (int i = 0; i < pieceModel.CellCount; i++)
        {
            var pos = new Vector2Int(
                boardPos.x + pieceModel.Positions[i].x,
                boardPos.y + pieceModel.Positions[i].y
            );
            cellsToCheck.Add(pos);
            _placedPositions.Add(pos);
        }

        bool merged = true;
        bool isFirstPass = true;

        while (merged)
        {
            merged = false;
            var nextCheck = new List<Vector2Int>();

            for (int i = 0; i < cellsToCheck.Count; i++)
            {
                var cell = cellsToCheck[i];
                var cellData = model.GetCell(cell.x, cell.y);
                if (cellData == null || cellData.IsEmpty) continue;

                var mergedNeighbors = FindMatchingNeighbors(model, cell, cellData.Value, isFirstPass);

                if (mergedNeighbors.Count > 0)
                {
                    int sum = cellData.Value;
                    var absorbedPositions = new List<Vector2Int>();

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

                    mergeEvents.Add(new MergeEvent
                    {
                        TargetPos = cell,
                        AbsorbedPositions = absorbedPositions,
                        IsChain = !isFirstPass
                    });
                }
            }

            cellsToCheck = nextCheck;
            isFirstPass = false;
        }

        return mergeEvents;
    }

    private List<Vector2Int> FindMatchingNeighbors(BoardModel model, Vector2Int pos, int value, bool excludePlaced)
    {
        var matches = new List<Vector2Int>(4);

        for (int i = 0; i < Directions.Length; i++)
        {
            int row = pos.x + Directions[i].x;
            int col = pos.y + Directions[i].y;

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
