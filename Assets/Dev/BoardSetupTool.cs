#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public static class BoardSetupTool
{
    private const string PrefabPath = "Assets/Prefabs";
    private const string SOPath = "Assets/ScriptableObjects";
    private const string CellPrefabPath = PrefabPath + "/CellPrefab.prefab";
    private const string PieceCellPrefabPath = PrefabPath + "/PieceCellPrefab.prefab";
    private const string PiecePrefabPath = PrefabPath + "/PiecePrefab.prefab";
    private const string BoardConfigPath = SOPath + "/BoardConfig.asset";
    private const string PieceSpawnConfigPath = SOPath + "/PieceSpawnConfig.asset";

    [MenuItem("NumbersBlast/Setup Everything")]
    private static void SetupEverything()
    {
        EnsureFolders();
        var config = CreateOrLoadBoardConfig();
        CreateOrLoadCellPrefab();
        CreateOrLoadPieceCellPrefab();
        CreateOrLoadPiecePrefab();
        CreateOrLoadPieceSpawnConfig();
        var canvas = FindOrCreateCanvas();
        var boardView = SetupBoardView(canvas.transform);
        var pieceTray = SetupPieceTray(canvas.transform);
        SetupGameplayScope(config, boardView, pieceTray);

        AssetDatabase.SaveAssets();
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log("[BoardSetupTool] Setup complete. Press Play.");
    }

    private static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder(PrefabPath))
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        if (!AssetDatabase.IsValidFolder(SOPath))
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
    }

    private static BoardConfig CreateOrLoadBoardConfig()
    {
        var config = AssetDatabase.LoadAssetAtPath<BoardConfig>(BoardConfigPath);
        if (config != null) return config;

        config = ScriptableObject.CreateInstance<BoardConfig>();
        config.Rows = 8;
        config.Columns = 8;
        config.CellSpacing = 4f;
        config.MinBlockValue = 1;
        config.MaxBlockValue = 4;
        config.EmptyCellColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        config.BlockColors = new BlockColorEntry[]
        {
            new() { Value = 1, Color = new Color(0.55f, 0.83f, 0.78f) },
            new() { Value = 2, Color = new Color(0.99f, 0.75f, 0.44f) },
            new() { Value = 3, Color = new Color(0.55f, 0.63f, 0.80f) },
            new() { Value = 4, Color = new Color(0.91f, 0.54f, 0.54f) }
        };

        AssetDatabase.CreateAsset(config, BoardConfigPath);
        return config;
    }

    private static GameObject CreateOrLoadCellPrefab()
    {
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(CellPrefabPath);
        if (existing != null) return existing;

        var cellGo = new GameObject("CellPrefab", typeof(RectTransform), typeof(Image));
        var image = cellGo.GetComponent<Image>();
        image.color = new Color(0.9f, 0.9f, 0.9f, 1f);

        var textGo = CreateValueText(cellGo.transform);
        var text = textGo.GetComponent<TextMeshProUGUI>();

        var cellView = cellGo.AddComponent<CellView>();
        BindSerializedFields(cellView, "_background", image, "_valueText", text);

        var prefab = PrefabUtility.SaveAsPrefabAsset(cellGo, CellPrefabPath);
        Object.DestroyImmediate(cellGo);
        return prefab;
    }

    private static GameObject CreateOrLoadPieceCellPrefab()
    {
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(PieceCellPrefabPath);
        if (existing != null) return existing;

        var cellGo = new GameObject("PieceCellPrefab", typeof(RectTransform), typeof(Image));
        var image = cellGo.GetComponent<Image>();
        image.color = Color.white;

        var textGo = CreateValueText(cellGo.transform);
        var text = textGo.GetComponent<TextMeshProUGUI>();
        text.fontSize = 24;

        var cellView = cellGo.AddComponent<PieceCellView>();
        BindSerializedFields(cellView, "_background", image, "_valueText", text);

        var prefab = PrefabUtility.SaveAsPrefabAsset(cellGo, PieceCellPrefabPath);
        Object.DestroyImmediate(cellGo);
        return prefab;
    }

    private static GameObject CreateOrLoadPiecePrefab()
    {
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(PiecePrefabPath);
        if (existing != null) return existing;

        var pieceGo = new GameObject("PiecePrefab", typeof(RectTransform));
        var pieceView = pieceGo.AddComponent<PieceView>();

        var pieceCellPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PieceCellPrefabPath);
        var so = new SerializedObject(pieceView);
        so.FindProperty("_cellPrefab").objectReferenceValue = pieceCellPrefab;
        so.ApplyModifiedPropertiesWithoutUndo();

        var prefab = PrefabUtility.SaveAsPrefabAsset(pieceGo, PiecePrefabPath);
        Object.DestroyImmediate(pieceGo);
        return prefab;
    }

    private static PieceSpawnConfig CreateOrLoadPieceSpawnConfig()
    {
        var existing = AssetDatabase.LoadAssetAtPath<PieceSpawnConfig>(PieceSpawnConfigPath);
        if (existing != null) return existing;

        var config = ScriptableObject.CreateInstance<PieceSpawnConfig>();
        config.PiecesPerTray = 3;
        config.Shapes = new PieceShapeData[0];

        AssetDatabase.CreateAsset(config, PieceSpawnConfigPath);
        return config;
    }

    private static Canvas FindOrCreateCanvas()
    {
        var canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas != null) return canvas;

        var canvasGo = new GameObject("Canvas");
        canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGo.AddComponent<GraphicRaycaster>();

        if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var eventSystemGo = new GameObject("EventSystem");
            eventSystemGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemGo.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        return canvas;
    }

    private static BoardView SetupBoardView(Transform canvasTransform)
    {
        var existing = canvasTransform.Find("Board");
        if (existing != null)
        {
            var existingView = existing.GetComponent<BoardView>();
            if (existingView != null) return existingView;
            Object.DestroyImmediate(existing.gameObject);
        }

        var boardGo = new GameObject("Board", typeof(RectTransform), typeof(GridLayoutGroup));
        boardGo.transform.SetParent(canvasTransform, false);

        var rect = boardGo.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0, 100);
        rect.sizeDelta = new Vector2(832, 832);

        var boardView = boardGo.AddComponent<BoardView>();
        var cellPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(CellPrefabPath);

        var bvSo = new SerializedObject(boardView);
        bvSo.FindProperty("_cellPrefab").objectReferenceValue = cellPrefab;
        bvSo.FindProperty("_gridLayout").objectReferenceValue = boardGo.GetComponent<GridLayoutGroup>();
        bvSo.FindProperty("_boardRect").objectReferenceValue = rect;
        bvSo.ApplyModifiedPropertiesWithoutUndo();

        return boardView;
    }

    private static PieceTray SetupPieceTray(Transform canvasTransform)
    {
        var existing = canvasTransform.Find("PieceTray");
        if (existing != null)
        {
            var existingTray = existing.GetComponent<PieceTray>();
            if (existingTray != null) return existingTray;
            Object.DestroyImmediate(existing.gameObject);
        }

        var trayGo = new GameObject("PieceTray", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        trayGo.transform.SetParent(canvasTransform, false);

        var rect = trayGo.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0, 50);
        rect.sizeDelta = new Vector2(900, 200);

        var layout = trayGo.GetComponent<HorizontalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.spacing = 20;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
        layout.childControlWidth = false;
        layout.childControlHeight = false;

        var slots = new Transform[3];
        for (int i = 0; i < 3; i++)
        {
            var slotGo = new GameObject($"Slot_{i}", typeof(RectTransform));
            slotGo.transform.SetParent(trayGo.transform, false);
            var slotRect = slotGo.GetComponent<RectTransform>();
            slotRect.sizeDelta = new Vector2(250, 200);
            slots[i] = slotGo.transform;
        }

        var pieceTray = trayGo.AddComponent<PieceTray>();
        var piecePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PiecePrefabPath);

        var so = new SerializedObject(pieceTray);
        so.FindProperty("_piecePrefab").objectReferenceValue = piecePrefab;

        var slotsProperty = so.FindProperty("_pieceSlots");
        slotsProperty.arraySize = 3;
        for (int i = 0; i < 3; i++)
        {
            slotsProperty.GetArrayElementAtIndex(i).objectReferenceValue = slots[i];
        }
        so.ApplyModifiedPropertiesWithoutUndo();

        return pieceTray;
    }

    private static void SetupGameplayScope(BoardConfig config, BoardView boardView, PieceTray pieceTray)
    {
        var existingScope = Object.FindAnyObjectByType<GameplayLifetimeScope>();
        if (existingScope != null)
        {
            var so = new SerializedObject(existingScope);
            so.FindProperty("_boardConfig").objectReferenceValue = config;
            so.FindProperty("_boardView").objectReferenceValue = boardView;
            so.FindProperty("_pieceTray").objectReferenceValue = pieceTray;
            so.ApplyModifiedPropertiesWithoutUndo();
            return;
        }

        var scopeGo = new GameObject("GameplayScope");
        var scope = scopeGo.AddComponent<GameplayLifetimeScope>();

        var scopeSo = new SerializedObject(scope);
        scopeSo.FindProperty("_boardConfig").objectReferenceValue = config;
        scopeSo.FindProperty("_boardView").objectReferenceValue = boardView;
        scopeSo.FindProperty("_pieceTray").objectReferenceValue = pieceTray;
        scopeSo.ApplyModifiedPropertiesWithoutUndo();
    }

    private static GameObject CreateValueText(Transform parent)
    {
        var textGo = new GameObject("Value", typeof(RectTransform), typeof(TextMeshProUGUI));
        textGo.transform.SetParent(parent, false);

        var textRect = textGo.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        var text = textGo.GetComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = 36;
        text.color = Color.white;
        text.fontStyle = FontStyles.Bold;

        return textGo;
    }

    private static void BindSerializedFields(Component target, string field1, Object value1, string field2, Object value2)
    {
        var so = new SerializedObject(target);
        so.FindProperty(field1).objectReferenceValue = value1;
        so.FindProperty(field2).objectReferenceValue = value2;
        so.ApplyModifiedPropertiesWithoutUndo();
    }
}
#endif
