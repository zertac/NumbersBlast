#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoardConfig))]
public class BoardConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var config = (BoardConfig)target;

        if (config.PieceSpawnConfig == null)
        {
            EditorGUILayout.HelpBox("PieceSpawnConfig is not assigned!", MessageType.Error);
            return;
        }

        if (config.Theme == null)
        {
            EditorGUILayout.HelpBox("Theme is not assigned!", MessageType.Error);
        }

        if (config.PieceSpawnConfig.Shapes == null || config.PieceSpawnConfig.Shapes.Length == 0)
        {
            EditorGUILayout.HelpBox("PieceSpawnConfig has no shapes assigned!", MessageType.Warning);
            return;
        }

        bool hasInvalid = false;
        for (int i = 0; i < config.PieceSpawnConfig.Shapes.Length; i++)
        {
            var shape = config.PieceSpawnConfig.Shapes[i];
            if (shape == null) continue;

            if (!config.ValidatePieceShape(shape))
            {
                EditorGUILayout.HelpBox(
                    $"Piece shape '{shape.name}' is too large for {config.Rows}x{config.Columns} board!",
                    MessageType.Error);
                hasInvalid = true;
            }
        }

        if (!hasInvalid)
        {
            EditorGUILayout.HelpBox(
                $"All {config.PieceSpawnConfig.Shapes.Length} piece shapes valid for {config.Rows}x{config.Columns} board.",
                MessageType.Info);
        }

        EditorGUILayout.Space(10);
        if (GUILayout.Button("Run Full Validation"))
        {
            RunFullValidation(config);
        }
    }

    private void RunFullValidation(BoardConfig config)
    {
        int issues = 0;

        if (config.PieceSpawnConfig == null)
        {
            Debug.LogError("[Validation] PieceSpawnConfig is not assigned!");
            issues++;
        }

        if (config.Theme == null)
        {
            Debug.LogError("[Validation] Theme is not assigned!");
            issues++;
        }
        else
        {
            if (config.Theme.EmptyCellSprite == null)
                Debug.LogWarning("[Validation] Theme: EmptyCellSprite is not assigned.");
            if (config.Theme.BlockSprite == null)
                Debug.LogWarning("[Validation] Theme: BlockSprite is not assigned.");
            if (config.Theme.BlockVisuals == null || config.Theme.BlockVisuals.Length == 0)
                Debug.LogWarning("[Validation] Theme: No BlockVisuals defined.");
        }

        if (config.PieceSpawnConfig != null && config.PieceSpawnConfig.Shapes != null)
        {
            for (int i = 0; i < config.PieceSpawnConfig.Shapes.Length; i++)
            {
                var shape = config.PieceSpawnConfig.Shapes[i];
                if (shape == null)
                {
                    Debug.LogError($"[Validation] PieceSpawnConfig.Shapes[{i}] is null!");
                    issues++;
                    continue;
                }

                if (shape.GetActiveCellCount() == 0)
                {
                    Debug.LogWarning($"[Validation] Shape '{shape.name}' has no active cells.");
                    issues++;
                }

                if (!config.ValidatePieceShape(shape))
                {
                    Debug.LogError($"[Validation] Shape '{shape.name}' too large for {config.Rows}x{config.Columns} board!");
                    issues++;
                }
            }
        }

        if (config.MinBlockValue > config.MaxBlockValue)
        {
            Debug.LogError($"[Validation] MinBlockValue ({config.MinBlockValue}) > MaxBlockValue ({config.MaxBlockValue})!");
            issues++;
        }

        if (issues == 0)
            Debug.Log("[Validation] All checks passed!");
        else
            Debug.LogWarning($"[Validation] Found {issues} issue(s).");
    }
}
#endif
