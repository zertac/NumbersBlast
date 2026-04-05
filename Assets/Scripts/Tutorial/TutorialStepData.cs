using System;
using UnityEngine;

namespace NumbersBlast.Tutorial
{
    [CreateAssetMenu(fileName = "TutorialStep", menuName = "NumbersBlast/Tutorial Step")]
    public class TutorialStepData : ScriptableObject
    {
        [Header("Message")]
        public string InstructionText;
        public string CompletionTitle;
        public string CompletionDescription;

        [Header("Board State")]
        public int BoardRows = 8;
        public int BoardColumns = 8;
        [HideInInspector]
        public int[] BoardCellValues;

        [Header("Piece")]
        public int PieceRows = 3;
        public int PieceColumns = 3;
        [HideInInspector]
        public bool[] PieceCells;
        [HideInInspector]
        public int[] PieceValues;

        [Header("Target Placement")]
        public Vector2Int TargetBoardPosition;

        public void InitializeBoardIfNeeded()
        {
            int size = BoardRows * BoardColumns;
            if (BoardCellValues == null || BoardCellValues.Length != size)
                BoardCellValues = new int[size];
        }

        public void InitializePieceIfNeeded()
        {
            int size = PieceRows * PieceColumns;
            if (PieceCells == null || PieceCells.Length != size)
            {
                PieceCells = new bool[size];
                PieceValues = new int[size];
            }
        }

        public int GetBoardValue(int row, int col)
        {
            return BoardCellValues[row * BoardColumns + col];
        }

        public void SetBoardValue(int row, int col, int value)
        {
            BoardCellValues[row * BoardColumns + col] = value;
        }

        public bool GetPieceCell(int row, int col)
        {
            return PieceCells[row * PieceColumns + col];
        }

        public void SetPieceCell(int row, int col, bool active)
        {
            PieceCells[row * PieceColumns + col] = active;
        }

        public int GetPieceValue(int row, int col)
        {
            return PieceValues[row * PieceColumns + col];
        }

        public void SetPieceValue(int row, int col, int value)
        {
            PieceValues[row * PieceColumns + col] = value;
        }

        public Vector2Int[] GetPieceNormalizedPositions()
        {
            int count = 0;
            int minRow = int.MaxValue, minCol = int.MaxValue;

            for (int r = 0; r < PieceRows; r++)
            {
                for (int c = 0; c < PieceColumns; c++)
                {
                    if (GetPieceCell(r, c))
                    {
                        count++;
                        if (r < minRow) minRow = r;
                        if (c < minCol) minCol = c;
                    }
                }
            }

            var positions = new Vector2Int[count];
            int index = 0;
            for (int r = 0; r < PieceRows; r++)
            {
                for (int c = 0; c < PieceColumns; c++)
                {
                    if (GetPieceCell(r, c))
                    {
                        positions[index++] = new Vector2Int(r - minRow, c - minCol);
                    }
                }
            }

            return positions;
        }

        public int[] GetPieceFixedValues()
        {
            int count = 0;
            for (int r = 0; r < PieceRows; r++)
                for (int c = 0; c < PieceColumns; c++)
                    if (GetPieceCell(r, c)) count++;

            var values = new int[count];
            int index = 0;
            for (int r = 0; r < PieceRows; r++)
            {
                for (int c = 0; c < PieceColumns; c++)
                {
                    if (GetPieceCell(r, c))
                    {
                        values[index++] = GetPieceValue(r, c);
                    }
                }
            }

            return values;
        }
    }
}
