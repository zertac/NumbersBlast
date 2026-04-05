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
            int index = row * BoardColumns + col;
            if (index < 0 || index >= BoardCellValues.Length) return 0;
            return BoardCellValues[index];
        }

        public void SetBoardValue(int row, int col, int value)
        {
            int index = row * BoardColumns + col;
            if (index < 0 || index >= BoardCellValues.Length) return;
            BoardCellValues[index] = value;
        }

        public bool GetPieceCell(int row, int col)
        {
            int index = row * PieceColumns + col;
            if (index < 0 || index >= PieceCells.Length) return false;
            return PieceCells[index];
        }

        public void SetPieceCell(int row, int col, bool active)
        {
            int index = row * PieceColumns + col;
            if (index < 0 || index >= PieceCells.Length) return;
            PieceCells[index] = active;
        }

        public int GetPieceValue(int row, int col)
        {
            int index = row * PieceColumns + col;
            if (index < 0 || index >= PieceValues.Length) return 0;
            return PieceValues[index];
        }

        public void SetPieceValue(int row, int col, int value)
        {
            int index = row * PieceColumns + col;
            if (index < 0 || index >= PieceValues.Length) return;
            PieceValues[index] = value;
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
