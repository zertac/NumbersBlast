#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class TutorialSetupTool
{
    private const string TutorialPath = "Assets/ScriptableObjects/Tutorial";

    [MenuItem("NumbersBlast/Setup Tutorial Data")]
    private static void SetupTutorialData()
    {
        EnsureFolder();

        var step1 = CreateStep("Step1_PlaceBlock", "Place the block on the highlighted area.",
            SetupPlacementStep);

        var step2 = CreateStep("Step2_MergeNumbers", "Place the block to merge the same numbers!",
            SetupMergeStep);

        var step3 = CreateStep("Step3_ClearLine", "Complete the row to clear it!",
            SetupLineClearStep);

        var config = CreateOrLoadAsset<TutorialConfig>(TutorialPath + "/TutorialConfig.asset");
        config.Steps = new TutorialStepData[] { step1, step2, step3 };
        EditorUtility.SetDirty(config);

        AssetDatabase.SaveAssets();
        Debug.Log("[TutorialSetup] Tutorial data created. Customize steps in Inspector.");
    }

    private static void EnsureFolder()
    {
        if (!AssetDatabase.IsValidFolder(TutorialPath))
        {
            if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
                AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Tutorial");
        }
    }

    private static TutorialStepData CreateStep(string name, string instruction, System.Action<TutorialStepData> setup)
    {
        var step = CreateOrLoadAsset<TutorialStepData>(TutorialPath + "/" + name + ".asset");
        step.InstructionText = instruction;
        step.BoardRows = 8;
        step.BoardColumns = 8;
        step.InitializeBoardIfNeeded();
        step.PieceRows = 3;
        step.PieceColumns = 3;
        step.InitializePieceIfNeeded();

        setup(step);

        EditorUtility.SetDirty(step);
        return step;
    }

    private static void SetupPlacementStep(TutorialStepData step)
    {
        // Simple: empty board, L-shaped piece, place at center
        step.SetPieceCell(0, 0, true);
        step.SetPieceValue(0, 0, 2);
        step.SetPieceCell(1, 0, true);
        step.SetPieceValue(1, 0, 3);
        step.SetPieceCell(1, 1, true);
        step.SetPieceValue(1, 1, 1);

        step.TargetBoardPosition = new Vector2Int(3, 3);
    }

    private static void SetupMergeStep(TutorialStepData step)
    {
        // Board has a [2] at (3,4), piece has [2] adjacent
        step.SetBoardValue(3, 4, 2);
        step.SetBoardValue(3, 5, 3);

        step.SetPieceCell(0, 0, true);
        step.SetPieceValue(0, 0, 2);

        step.TargetBoardPosition = new Vector2Int(3, 3);
    }

    private static void SetupLineClearStep(TutorialStepData step)
    {
        // Almost full row at row 5, missing col 3
        for (int c = 0; c < 8; c++)
        {
            if (c != 3)
                step.SetBoardValue(5, c, (c % 4) + 1);
        }

        // Piece is single block that fills the gap
        step.SetPieceCell(0, 0, true);
        step.SetPieceValue(0, 0, 1);

        step.TargetBoardPosition = new Vector2Int(5, 3);
    }

    private static T CreateOrLoadAsset<T>(string path) where T : ScriptableObject
    {
        var existing = AssetDatabase.LoadAssetAtPath<T>(path);
        if (existing != null) return existing;

        var asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, path);
        return asset;
    }
}
#endif
