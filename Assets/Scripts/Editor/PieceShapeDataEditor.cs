#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PieceShapeData))]
public class PieceShapeDataEditor : Editor
{
    private const float CellSize = 40f;
    private const float Spacing = 2f;

    public override void OnInspectorGUI()
    {
        var shape = (PieceShapeData)target;

        EditorGUI.BeginChangeCheck();
        int newRows = EditorGUILayout.IntSlider("Rows", shape.Rows, 1, 8);
        int newCols = EditorGUILayout.IntSlider("Columns", shape.Columns, 1, 8);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(shape, "Resize Piece Grid");
            shape.ResizeGrid(newRows, newCols);
            EditorUtility.SetDirty(shape);
        }

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Click cells to toggle.", EditorStyles.miniLabel);
        EditorGUILayout.Space(5);

        DrawGrid(shape);

        EditorGUILayout.Space(5);
        var normalizedSize = shape.GetNormalizedSize();
        EditorGUILayout.LabelField($"Active Cells: {shape.GetActiveCellCount()}  |  Shape Size: {normalizedSize.x}x{normalizedSize.y}", EditorStyles.boldLabel);

        EditorGUILayout.Space(5);
        if (GUILayout.Button("Clear All"))
        {
            Undo.RecordObject(shape, "Clear Piece Shape");
            for (int i = 0; i < shape.Cells.Length; i++)
                shape.Cells[i] = false;
            EditorUtility.SetDirty(shape);
        }
    }

    private void DrawGrid(PieceShapeData shape)
    {
        float totalWidth = (CellSize + Spacing) * shape.Columns;
        float totalHeight = (CellSize + Spacing) * shape.Rows;

        var gridRect = GUILayoutUtility.GetRect(totalWidth, totalHeight);
        float startX = gridRect.x + (gridRect.width - totalWidth) * 0.5f;
        float startY = gridRect.y;

        for (int r = 0; r < shape.Rows; r++)
        {
            for (int c = 0; c < shape.Columns; c++)
            {
                var cellRect = new Rect(
                    startX + c * (CellSize + Spacing),
                    startY + r * (CellSize + Spacing),
                    CellSize,
                    CellSize
                );

                bool isActive = shape.GetCell(r, c);
                EditorGUI.DrawRect(cellRect, isActive ? new Color(0.3f, 0.5f, 0.9f) : new Color(0.8f, 0.8f, 0.8f));

                if (Event.current.type == EventType.MouseDown && cellRect.Contains(Event.current.mousePosition))
                {
                    Undo.RecordObject(shape, "Toggle Piece Cell");
                    shape.SetCell(r, c, !isActive);
                    EditorUtility.SetDirty(shape);
                    Event.current.Use();
                    Repaint();
                }
            }
        }
    }
}
#endif
