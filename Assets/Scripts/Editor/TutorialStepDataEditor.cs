#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using NumbersBlast.Tutorial;

namespace NumbersBlast.Editor
{
    [CustomEditor(typeof(TutorialStepData))]
    public class TutorialStepDataEditor : UnityEditor.Editor
    {
        private const float CellSize = 35f;
        private const float Spacing = 2f;

        private static readonly Color EmptyColor = new(0.85f, 0.85f, 0.85f);
        private static readonly Color[] ValueColors =
        {
            Color.clear,
            new(0.55f, 0.83f, 0.78f),
            new(0.99f, 0.75f, 0.44f),
            new(0.55f, 0.63f, 0.80f),
            new(0.91f, 0.54f, 0.54f)
        };
        private static readonly Color TargetColor = new(0.3f, 0.9f, 0.3f, 0.5f);
        private static readonly Color PieceActiveColor = new(0.3f, 0.5f, 0.9f);

        private static GUIStyle _cellLabelStyle;

        private int _boardBrushValue = 1;
        private int _pieceBrushValue = 1;

        private static GUIStyle CellLabelStyle
        {
            get
            {
                if (_cellLabelStyle == null)
                {
                    _cellLabelStyle = new GUIStyle(EditorStyles.boldLabel)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        normal = { textColor = Color.white }
                    };
                }
                return _cellLabelStyle;
            }
        }

        public override void OnInspectorGUI()
        {
            var step = (TutorialStepData)target;
            step.InitializeBoardIfNeeded();
            step.InitializePieceIfNeeded();

            DrawInstruction(step);
            EditorGUILayout.Space(10);
            DrawBoardSection(step);
            EditorGUILayout.Space(10);
            DrawPieceSection(step);
            EditorGUILayout.Space(10);
            DrawTargetSection(step);
        }

        private void DrawInstruction(TutorialStepData step)
        {
            EditorGUILayout.LabelField("Instruction", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            string text = EditorGUILayout.TextArea(step.InstructionText, GUILayout.Height(50));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(step, "Change Instruction");
                step.InstructionText = text;
                EditorUtility.SetDirty(step);
            }

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Completion Feedback", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            string title = EditorGUILayout.TextField("Title", step.CompletionTitle);
            string desc = EditorGUILayout.TextField("Description", step.CompletionDescription);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(step, "Change Completion Feedback");
                step.CompletionTitle = title;
                step.CompletionDescription = desc;
                EditorUtility.SetDirty(step);
            }
        }

        private void DrawBoardSection(TutorialStepData step)
        {
            EditorGUILayout.LabelField("Board State", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            int newRows = EditorGUILayout.IntSlider("Rows", step.BoardRows, 1, 10);
            int newCols = EditorGUILayout.IntSlider("Columns", step.BoardColumns, 1, 10);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(step, "Resize Tutorial Board");
                ResizeBoard(step, newRows, newCols);
                EditorUtility.SetDirty(step);
            }

            EditorGUILayout.Space(3);
            _boardBrushValue = EditorGUILayout.IntSlider("Brush Value (0=clear)", _boardBrushValue, 0, 4);
            EditorGUILayout.LabelField("Click cell to paint. Right-click to clear.", EditorStyles.miniLabel);

            DrawBoardGrid(step);
        }

        private void DrawBoardGrid(TutorialStepData step)
        {
            float totalWidth = (CellSize + Spacing) * step.BoardColumns;
            float totalHeight = (CellSize + Spacing) * step.BoardRows;

            var gridRect = GUILayoutUtility.GetRect(totalWidth, totalHeight);
            float startX = gridRect.x + (gridRect.width - totalWidth) * 0.5f;
            float startY = gridRect.y;

            for (int r = 0; r < step.BoardRows; r++)
            {
                for (int c = 0; c < step.BoardColumns; c++)
                {
                    var cellRect = new Rect(
                        startX + c * (CellSize + Spacing),
                        startY + r * (CellSize + Spacing),
                        CellSize, CellSize
                    );

                    int value = step.GetBoardValue(r, c);

                    // Draw target highlight underneath
                    bool isTarget = IsTargetCell(step, r, c);
                    if (isTarget)
                    {
                        EditorGUI.DrawRect(cellRect, TargetColor);
                        var innerRect = new Rect(cellRect.x + 2, cellRect.y + 2, cellRect.width - 4, cellRect.height - 4);
                        EditorGUI.DrawRect(innerRect, value > 0 ? GetValueColor(value) : EmptyColor);
                    }
                    else
                    {
                        EditorGUI.DrawRect(cellRect, value > 0 ? GetValueColor(value) : EmptyColor);
                    }

                    if (value > 0)
                    {
                        EditorGUI.LabelField(cellRect, value.ToString(), CellLabelStyle);
                    }

                    HandleBoardCellClick(step, cellRect, r, c);
                }
            }
        }

        private void HandleBoardCellClick(TutorialStepData step, Rect cellRect, int row, int col)
        {
            if (Event.current.type == EventType.MouseDown && cellRect.Contains(Event.current.mousePosition))
            {
                Undo.RecordObject(step, "Paint Board Cell");

                if (Event.current.button == 1)
                    step.SetBoardValue(row, col, 0);
                else
                    step.SetBoardValue(row, col, _boardBrushValue);

                EditorUtility.SetDirty(step);
                Event.current.Use();
                Repaint();
            }
        }

        private void DrawPieceSection(TutorialStepData step)
        {
            EditorGUILayout.LabelField("Piece", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            int newRows = EditorGUILayout.IntSlider("Piece Rows", step.PieceRows, 1, 5);
            int newCols = EditorGUILayout.IntSlider("Piece Columns", step.PieceColumns, 1, 5);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(step, "Resize Tutorial Piece");
                ResizePiece(step, newRows, newCols);
                EditorUtility.SetDirty(step);
            }

            EditorGUILayout.Space(3);
            _pieceBrushValue = EditorGUILayout.IntSlider("Piece Value", _pieceBrushValue, 1, 4);
            EditorGUILayout.LabelField("Left-click to toggle + set value. Right-click to clear.", EditorStyles.miniLabel);

            DrawPieceGrid(step);
        }

        private void DrawPieceGrid(TutorialStepData step)
        {
            float totalWidth = (CellSize + Spacing) * step.PieceColumns;
            float totalHeight = (CellSize + Spacing) * step.PieceRows;

            var gridRect = GUILayoutUtility.GetRect(totalWidth, totalHeight);
            float startX = gridRect.x + (gridRect.width - totalWidth) * 0.5f;
            float startY = gridRect.y;

            for (int r = 0; r < step.PieceRows; r++)
            {
                for (int c = 0; c < step.PieceColumns; c++)
                {
                    var cellRect = new Rect(
                        startX + c * (CellSize + Spacing),
                        startY + r * (CellSize + Spacing),
                        CellSize, CellSize
                    );

                    bool active = step.GetPieceCell(r, c);
                    int value = step.GetPieceValue(r, c);

                    EditorGUI.DrawRect(cellRect, active ? GetValueColor(value) : EmptyColor);

                    if (active && value > 0)
                    {
                        EditorGUI.LabelField(cellRect, value.ToString(), CellLabelStyle);
                    }

                    if (Event.current.type == EventType.MouseDown && cellRect.Contains(Event.current.mousePosition))
                    {
                        Undo.RecordObject(step, "Toggle Piece Cell");

                        if (Event.current.button == 1)
                        {
                            step.SetPieceCell(r, c, false);
                            step.SetPieceValue(r, c, 0);
                        }
                        else
                        {
                            step.SetPieceCell(r, c, true);
                            step.SetPieceValue(r, c, _pieceBrushValue);
                        }

                        EditorUtility.SetDirty(step);
                        Event.current.Use();
                        Repaint();
                    }
                }
            }
        }

        private void DrawTargetSection(TutorialStepData step)
        {
            EditorGUILayout.LabelField("Target Position", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            var target = EditorGUILayout.Vector2IntField("Board Position (row, col)", step.TargetBoardPosition);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(step, "Change Target Position");
                step.TargetBoardPosition = target;
                EditorUtility.SetDirty(step);
            }
        }

        private bool IsTargetCell(TutorialStepData step, int row, int col)
        {
            var positions = step.GetPieceNormalizedPositions();
            var target = step.TargetBoardPosition;

            for (int i = 0; i < positions.Length; i++)
            {
                if (target.x + positions[i].x == row && target.y + positions[i].y == col)
                    return true;
            }

            return false;
        }

        private void ResizeBoard(TutorialStepData step, int newRows, int newCols)
        {
            var newValues = new int[newRows * newCols];
            int minRows = Mathf.Min(step.BoardRows, newRows);
            int minCols = Mathf.Min(step.BoardColumns, newCols);

            for (int r = 0; r < minRows; r++)
                for (int c = 0; c < minCols; c++)
                    newValues[r * newCols + c] = step.BoardCellValues[r * step.BoardColumns + c];

            step.BoardRows = newRows;
            step.BoardColumns = newCols;
            step.BoardCellValues = newValues;
        }

        private void ResizePiece(TutorialStepData step, int newRows, int newCols)
        {
            int newSize = newRows * newCols;
            var newCells = new bool[newSize];
            var newValues = new int[newSize];
            int minRows = Mathf.Min(step.PieceRows, newRows);
            int minCols = Mathf.Min(step.PieceColumns, newCols);

            for (int r = 0; r < minRows; r++)
            {
                for (int c = 0; c < minCols; c++)
                {
                    newCells[r * newCols + c] = step.PieceCells[r * step.PieceColumns + c];
                    newValues[r * newCols + c] = step.PieceValues[r * step.PieceColumns + c];
                }
            }

            step.PieceRows = newRows;
            step.PieceColumns = newCols;
            step.PieceCells = newCells;
            step.PieceValues = newValues;
        }

        private Color GetValueColor(int value)
        {
            if (value >= 1 && value <= 4) return ValueColors[value];
            return new Color(0.7f, 0.7f, 0.7f);
        }
    }
}
#endif
