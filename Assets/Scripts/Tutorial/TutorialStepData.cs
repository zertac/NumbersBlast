using System;
using UnityEngine;

namespace NumbersBlast.Tutorial
{
    /// <summary>
    /// ScriptableObject defining a single tutorial step, including board state, piece shape, instruction text, and target placement.
    /// </summary>
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

        /// <summary>
        /// Allocates the board cell values array if it is null or its size does not match the current board dimensions.
        /// </summary>
        public void InitializeBoardIfNeeded()
        {
            int size = BoardRows * BoardColumns;
            if (BoardCellValues == null || BoardCellValues.Length != size)
                BoardCellValues = new int[size];
        }

        /// <summary>
        /// Allocates the piece cell and value arrays if they are null or their size does not match the current piece dimensions.
        /// </summary>
        public void InitializePieceIfNeeded()
        {
            int size = PieceRows * PieceColumns;
            if (PieceCells == null || PieceCells.Length != size)
            {
                PieceCells = new bool[size];
                PieceValues = new int[size];
            }
        }

        /// <summary>
        /// Returns the board cell value at the given row and column, or 0 if out of bounds.
        /// </summary>
        public int GetBoardValue(int row, int col)
        {
            int index = row * BoardColumns + col;
            if (index < 0 || index >= BoardCellValues.Length) return 0;
            return BoardCellValues[index];
        }

        /// <summary>
        /// Sets the board cell value at the given row and column. No-op if out of bounds.
        /// </summary>
        public void SetBoardValue(int row, int col, int value)
        {
            int index = row * BoardColumns + col;
            if (index < 0 || index >= BoardCellValues.Length) return;
            BoardCellValues[index] = value;
        }

        /// <summary>
        /// Returns whether the piece cell at the given row and column is active, or false if out of bounds.
        /// </summary>
        public bool GetPieceCell(int row, int col)
        {
            int index = row * PieceColumns + col;
            if (index < 0 || index >= PieceCells.Length) return false;
            return PieceCells[index];
        }

        /// <summary>
        /// Sets the active state of the piece cell at the given row and column. No-op if out of bounds.
        /// </summary>
        public void SetPieceCell(int row, int col, bool active)
        {
            int index = row * PieceColumns + col;
            if (index < 0 || index >= PieceCells.Length) return;
            PieceCells[index] = active;
        }

        /// <summary>
        /// Returns the piece cell value at the given row and column, or 0 if out of bounds.
        /// </summary>
        public int GetPieceValue(int row, int col)
        {
            int index = row * PieceColumns + col;
            if (index < 0 || index >= PieceValues.Length) return 0;
            return PieceValues[index];
        }

        /// <summary>
        /// Sets the piece cell value at the given row and column. No-op if out of bounds.
        /// </summary>
        public void SetPieceValue(int row, int col, int value)
        {
            int index = row * PieceColumns + col;
            if (index < 0 || index >= PieceValues.Length) return;
            PieceValues[index] = value;
        }

        /// <summary>
        /// Returns the active piece cell positions normalized so the top-left active cell is at (0, 0).
        /// </summary>
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

        /// <summary>
        /// Returns the values of all active piece cells in row-major order.
        /// </summary>
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
