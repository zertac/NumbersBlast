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
        var scoreUI = SetupScoreUI(canvas.transform);
        var gameplayHUD = SetupGameplayHUD(canvas.transform);
        var feedbackConfig = CreateOrLoadFeedbackConfig();
        var tutorialOverlay = SetupTutorialOverlay(canvas.transform);
        var multiplayerHUD = SetupMultiplayerHUD(canvas.transform);
        var opponentVisualPlayer = SetupOpponentVisualPlayer();
        UISetupTool.RunSetup();
        MultiplayerSetupTool.RunSetup();
        SetupGameplayScope(config, boardView, pieceTray, scoreUI, gameplayHUD, feedbackConfig, tutorialOverlay, multiplayerHUD, opponentVisualPlayer);

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
        if (config != null)
        {
            if (config.Theme == null)
                config.Theme = CreateOrLoadTheme();
            return config;
        }

        config = ScriptableObject.CreateInstance<BoardConfig>();
        config.Rows = 8;
        config.Columns = 8;
        config.CellSpacing = 4f;
        config.MinBlockValue = 1;
        config.MaxBlockValue = 4;
        config.Theme = CreateOrLoadTheme();

        AssetDatabase.CreateAsset(config, BoardConfigPath);
        return config;
    }

    private static ThemeData CreateOrLoadTheme()
    {
        var themePath = SOPath + "/DefaultTheme.asset";
        var theme = AssetDatabase.LoadAssetAtPath<ThemeData>(themePath);
        if (theme != null) return theme;

        theme = ScriptableObject.CreateInstance<ThemeData>();
        theme.BackgroundColor = new Color(0.15f, 0.15f, 0.3f);
        theme.EmptyCellColor = new Color(0.85f, 0.85f, 0.85f);
        theme.BlockVisuals = new BlockVisual[]
        {
            new() { Value = 1, Color = new Color(0.55f, 0.83f, 0.78f) },
            new() { Value = 2, Color = new Color(0.99f, 0.75f, 0.44f) },
            new() { Value = 3, Color = new Color(0.55f, 0.63f, 0.80f) },
            new() { Value = 4, Color = new Color(0.91f, 0.54f, 0.54f) }
        };

        AssetDatabase.CreateAsset(theme, themePath);
        return theme;
    }

    private static GameObject CreateOrLoadCellPrefab()
    {
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(CellPrefabPath);
        if (existing != null) return existing;

        var cellGo = new GameObject("CellPrefab", typeof(RectTransform), typeof(Image));
        var image = cellGo.GetComponent<Image>();
        image.color = new Color(0.9f, 0.9f, 0.9f, 1f);

        // Highlight overlay
        var highlightGo = new GameObject("Highlight", typeof(RectTransform), typeof(Image));
        highlightGo.transform.SetParent(cellGo.transform, false);
        var hlRect = highlightGo.GetComponent<RectTransform>();
        hlRect.anchorMin = Vector2.zero;
        hlRect.anchorMax = Vector2.one;
        hlRect.sizeDelta = Vector2.zero;
        var hlImage = highlightGo.GetComponent<Image>();
        hlImage.color = Color.clear;
        hlImage.raycastTarget = false;

        var textGo = CreateValueText(cellGo.transform);
        var text = textGo.GetComponent<TextMeshProUGUI>();

        var cellView = cellGo.AddComponent<CellView>();
        var so = new SerializedObject(cellView);
        so.FindProperty("_background").objectReferenceValue = image;
        so.FindProperty("_highlight").objectReferenceValue = hlImage;
        so.FindProperty("_valueText").objectReferenceValue = text;
        so.ApplyModifiedPropertiesWithoutUndo();

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

    private static void EnsureBackground(Transform canvasTransform)
    {
        if (canvasTransform.Find("Background") != null) return;

        var bgGo = new GameObject("Background", typeof(RectTransform), typeof(Image));
        bgGo.transform.SetParent(canvasTransform, false);
        bgGo.transform.SetAsFirstSibling();
        var bgRect = bgGo.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        var bgImage = bgGo.GetComponent<Image>();
        bgImage.color = new Color(0.15f, 0.15f, 0.3f);
        bgImage.raycastTarget = false;
    }

    private static Canvas FindOrCreateCanvas()
    {
        var canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas != null)
        {
            EnsureBackground(canvas.transform);
            return canvas;
        }

        var canvasGo = new GameObject("Canvas");
        canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGo.AddComponent<GraphicRaycaster>();

        // Background image (first child so it renders behind everything)
        if (canvasGo.transform.Find("Background") == null)
        {
            var bgGo = new GameObject("Background", typeof(RectTransform), typeof(Image));
            bgGo.transform.SetParent(canvasGo.transform, false);
            bgGo.transform.SetAsFirstSibling();
            var bgRect = bgGo.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            var bgImage = bgGo.GetComponent<Image>();
            bgImage.color = new Color(0.15f, 0.15f, 0.3f);
            bgImage.raycastTarget = false;
        }

        if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var eventSystemGo = new GameObject("EventSystem");
            eventSystemGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemGo.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
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

        // Board frame (sibling, behind board)
        var existingFrame = canvasTransform.Find("BoardFrame");
        if (existingFrame != null) Object.DestroyImmediate(existingFrame.gameObject);

        var frameGo = new GameObject("BoardFrame", typeof(RectTransform), typeof(Image));
        frameGo.transform.SetParent(canvasTransform, false);
        frameGo.transform.SetSiblingIndex(boardGo.transform.GetSiblingIndex());
        var frameRect = frameGo.GetComponent<RectTransform>();
        frameRect.anchorMin = rect.anchorMin;
        frameRect.anchorMax = rect.anchorMax;
        frameRect.pivot = rect.pivot;
        frameRect.anchoredPosition = rect.anchoredPosition;
        frameRect.sizeDelta = rect.sizeDelta + new Vector2(20, 20);
        var frameImage = frameGo.GetComponent<Image>();
        frameImage.color = Color.white;
        frameImage.raycastTarget = false;
        frameGo.SetActive(false);

        var boardView = boardGo.AddComponent<BoardView>();
        var cellPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(CellPrefabPath);

        var bvSo = new SerializedObject(boardView);
        bvSo.FindProperty("_cellPrefab").objectReferenceValue = cellPrefab;
        bvSo.FindProperty("_gridLayout").objectReferenceValue = boardGo.GetComponent<GridLayoutGroup>();
        bvSo.FindProperty("_boardRect").objectReferenceValue = rect;
        bvSo.FindProperty("_boardFrame").objectReferenceValue = frameImage;
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

    private static ScoreUI SetupScoreUI(Transform canvasTransform)
    {
        var existing = canvasTransform.Find("ScorePanel");
        if (existing != null)
        {
            var existingUI = existing.GetComponent<ScoreUI>();
            if (existingUI != null) return existingUI;
            Object.DestroyImmediate(existing.gameObject);
        }

        var panelGo = new GameObject("ScorePanel", typeof(RectTransform));
        panelGo.transform.SetParent(canvasTransform, false);

        var panelRect = panelGo.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 1f);
        panelRect.anchorMax = new Vector2(0.5f, 1f);
        panelRect.pivot = new Vector2(0.5f, 1f);
        panelRect.anchoredPosition = new Vector2(0, -50);
        panelRect.sizeDelta = new Vector2(400, 100);

        var labelGo = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        labelGo.transform.SetParent(panelGo.transform, false);

        var labelRect = labelGo.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0.5f);
        labelRect.anchorMax = new Vector2(1, 1);
        labelRect.sizeDelta = Vector2.zero;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        var labelText = labelGo.GetComponent<TextMeshProUGUI>();
        labelText.text = "SCORE";
        labelText.alignment = TextAlignmentOptions.Center;
        labelText.fontSize = 28;
        labelText.color = Color.white;

        var scoreGo = new GameObject("ScoreText", typeof(RectTransform), typeof(TextMeshProUGUI));
        scoreGo.transform.SetParent(panelGo.transform, false);

        var scoreRect = scoreGo.GetComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0, 0);
        scoreRect.anchorMax = new Vector2(1, 0.5f);
        scoreRect.sizeDelta = Vector2.zero;
        scoreRect.offsetMin = Vector2.zero;
        scoreRect.offsetMax = Vector2.zero;

        var scoreText = scoreGo.GetComponent<TextMeshProUGUI>();
        scoreText.text = "0";
        scoreText.alignment = TextAlignmentOptions.Center;
        scoreText.fontSize = 48;
        scoreText.color = Color.white;
        scoreText.fontStyle = FontStyles.Bold;

        var scoreUI = panelGo.AddComponent<ScoreUI>();
        var so = new SerializedObject(scoreUI);
        so.FindProperty("_scoreText").objectReferenceValue = scoreText;
        so.ApplyModifiedPropertiesWithoutUndo();

        return scoreUI;
    }

    private static TutorialOverlay SetupTutorialOverlay(Transform canvasTransform)
    {
        var existing = canvasTransform.Find("TutorialOverlay");
        if (existing != null)
        {
            var existingOverlay = existing.GetComponent<TutorialOverlay>();
            if (existingOverlay != null) return existingOverlay;
            Object.DestroyImmediate(existing.gameObject);
        }

        var rootGo = new GameObject("TutorialOverlay", typeof(RectTransform), typeof(CanvasGroup));
        rootGo.transform.SetParent(canvasTransform, false);
        var rootRect = rootGo.GetComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.sizeDelta = Vector2.zero;

        // Overlay image with cutout shader
        var overlayGo = new GameObject("OverlayImage", typeof(RectTransform), typeof(RawImage));
        overlayGo.transform.SetParent(rootGo.transform, false);
        var overlayRect = overlayGo.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.sizeDelta = Vector2.zero;
        var overlayImage = overlayGo.GetComponent<RawImage>();
        overlayImage.color = Color.white;
        overlayImage.raycastTarget = false;

        var shader = Shader.Find("NumbersBlast/TutorialOverlay");
        if (shader != null)
        {
            var mat = new Material(shader);
            mat.SetColor("_Color", new Color(0, 0, 0, 0.7f));
            var matPath = "Assets/Materials/TutorialOverlayMat.mat";
            if (!AssetDatabase.IsValidFolder("Assets/Materials"))
                AssetDatabase.CreateFolder("Assets", "Materials");
            AssetDatabase.CreateAsset(mat, matPath);
            overlayImage.material = mat;
        }

        // Hand icon
        var handGo = new GameObject("HandIcon", typeof(RectTransform), typeof(TextMeshProUGUI));
        handGo.transform.SetParent(rootGo.transform, false);
        var handRect = handGo.GetComponent<RectTransform>();
        handRect.sizeDelta = new Vector2(60, 60);
        var handText = handGo.GetComponent<TextMeshProUGUI>();
        handText.text = "<b>></b>";
        handText.fontSize = 48;
        handText.alignment = TextAlignmentOptions.Center;
        handText.color = Color.yellow;
        handText.raycastTarget = false;

        // Instruction text
        var instrGo = new GameObject("InstructionText", typeof(RectTransform), typeof(TextMeshProUGUI));
        instrGo.transform.SetParent(rootGo.transform, false);
        var instrRect = instrGo.GetComponent<RectTransform>();
        instrRect.anchorMin = new Vector2(0.1f, 0.85f);
        instrRect.anchorMax = new Vector2(0.9f, 0.95f);
        instrRect.offsetMin = Vector2.zero;
        instrRect.offsetMax = Vector2.zero;
        var instrText = instrGo.GetComponent<TextMeshProUGUI>();
        instrText.fontSize = 32;
        instrText.alignment = TextAlignmentOptions.Center;
        instrText.color = Color.white;
        instrText.fontStyle = FontStyles.Bold;
        instrText.raycastTarget = false;

        var overlay = rootGo.AddComponent<TutorialOverlay>();
        var so = new SerializedObject(overlay);
        so.FindProperty("_overlayImage").objectReferenceValue = overlayImage;
        so.FindProperty("_handIcon").objectReferenceValue = handText;
        so.FindProperty("_instructionText").objectReferenceValue = instrText;
        so.FindProperty("_canvasGroup").objectReferenceValue = rootGo.GetComponent<CanvasGroup>();
        so.ApplyModifiedPropertiesWithoutUndo();

        rootGo.SetActive(false);

        return overlay;
    }

    private static FeedbackConfig CreateOrLoadFeedbackConfig()
    {
        var path = SOPath + "/FeedbackConfig.asset";
        var existing = AssetDatabase.LoadAssetAtPath<FeedbackConfig>(path);
        if (existing != null) return existing;

        var config = ScriptableObject.CreateInstance<FeedbackConfig>();
        AssetDatabase.CreateAsset(config, path);
        return config;
    }

    private static GameplayHUD SetupGameplayHUD(Transform canvasTransform)
    {
        var existing = canvasTransform.Find("GameplayHUD");
        if (existing != null)
        {
            var existingHUD = existing.GetComponent<GameplayHUD>();
            if (existingHUD != null) return existingHUD;
            Object.DestroyImmediate(existing.gameObject);
        }

        var hudGo = new GameObject("GameplayHUD", typeof(RectTransform));
        hudGo.transform.SetParent(canvasTransform, false);
        var hudRect = hudGo.GetComponent<RectTransform>();
        hudRect.anchorMin = Vector2.zero;
        hudRect.anchorMax = Vector2.one;
        hudRect.sizeDelta = Vector2.zero;

        // Pause button - top left (from UIButton prefab)
        var btnPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/UIButton.prefab");

        var pauseBtn = (GameObject)PrefabUtility.InstantiatePrefab(btnPrefab);
        pauseBtn.transform.SetParent(hudGo.transform, false);
        pauseBtn.name = "PauseButton";
        var pauseRect = pauseBtn.GetComponent<RectTransform>();
        pauseRect.anchorMin = new Vector2(0, 1);
        pauseRect.anchorMax = new Vector2(0, 1);
        pauseRect.pivot = new Vector2(0, 1);
        pauseRect.anchoredPosition = new Vector2(20, -20);
        pauseRect.sizeDelta = new Vector2(80, 80);
        pauseBtn.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.5f, 0.8f);
        pauseBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "II";
        pauseBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().fontSize = 32;

        // Settings button - top right (from UIButton prefab)
        var settingsBtn = (GameObject)PrefabUtility.InstantiatePrefab(btnPrefab);
        settingsBtn.transform.SetParent(hudGo.transform, false);
        settingsBtn.name = "SettingsButton";
        var settingsRect = settingsBtn.GetComponent<RectTransform>();
        settingsRect.anchorMin = new Vector2(1, 1);
        settingsRect.anchorMax = new Vector2(1, 1);
        settingsRect.pivot = new Vector2(1, 1);
        settingsRect.anchoredPosition = new Vector2(-20, -20);
        settingsRect.sizeDelta = new Vector2(80, 80);
        settingsBtn.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.5f, 0.8f);
        settingsBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "*";
        settingsBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().fontSize = 36;

        var hud = hudGo.AddComponent<GameplayHUD>();
        var so = new SerializedObject(hud);
        so.FindProperty("_pauseButton").objectReferenceValue = pauseBtn.GetComponent<Button>();
        so.FindProperty("_settingsButton").objectReferenceValue = settingsBtn.GetComponent<Button>();
        so.ApplyModifiedPropertiesWithoutUndo();

        return hud;
    }

    private static MultiplayerHUD SetupMultiplayerHUD(Transform canvasTransform)
    {
        var existing = canvasTransform.Find("MultiplayerHUD");
        if (existing != null)
        {
            var e = existing.GetComponent<MultiplayerHUD>();
            if (e != null) return e;
            Object.DestroyImmediate(existing.gameObject);
        }

        var go = new GameObject("MultiplayerHUD", typeof(RectTransform));
        go.transform.SetParent(canvasTransform, false);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        var panel = new GameObject("MultiplayerPanel", typeof(RectTransform));
        panel.transform.SetParent(go.transform, false);
        var panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 1);
        panelRect.anchorMax = new Vector2(1, 1);
        panelRect.pivot = new Vector2(0.5f, 1);
        panelRect.anchoredPosition = new Vector2(0, -110);
        panelRect.sizeDelta = new Vector2(0, 120);

        // Player name + score (left)
        var playerName = CreateTMP(panel.transform, "PlayerName", "YOU", 20, TMPro.TextAlignmentOptions.Left);
        SetAnchor(playerName, 0.05f, 0.55f, 0.45f, 0.95f);
        var playerScore = CreateTMP(panel.transform, "PlayerScore", "0", 36, TMPro.TextAlignmentOptions.Left);
        SetAnchor(playerScore, 0.05f, 0.05f, 0.45f, 0.55f);
        playerScore.GetComponent<TMPro.TextMeshProUGUI>().fontStyle = TMPro.FontStyles.Bold;

        // Opponent name + score (right)
        var oppName = CreateTMP(panel.transform, "OpponentName", "OPP", 20, TMPro.TextAlignmentOptions.Right);
        SetAnchor(oppName, 0.55f, 0.55f, 0.95f, 0.95f);
        var oppScore = CreateTMP(panel.transform, "OpponentScore", "0", 36, TMPro.TextAlignmentOptions.Right);
        SetAnchor(oppScore, 0.55f, 0.05f, 0.95f, 0.55f);
        oppScore.GetComponent<TMPro.TextMeshProUGUI>().fontStyle = TMPro.FontStyles.Bold;

        // Turn indicator (center)
        var turnText = CreateTMP(panel.transform, "TurnIndicator", "YOUR TURN", 22, TMPro.TextAlignmentOptions.Center);
        SetAnchor(turnText, 0.3f, 0.55f, 0.7f, 0.95f);
        turnText.GetComponent<TMPro.TextMeshProUGUI>().fontStyle = TMPro.FontStyles.Bold;

        // Timer bar
        var timerBg = new GameObject("TimerBg", typeof(RectTransform), typeof(UnityEngine.UI.Image));
        timerBg.transform.SetParent(panel.transform, false);
        SetAnchor(timerBg, 0.3f, 0.1f, 0.7f, 0.4f);
        timerBg.GetComponent<UnityEngine.UI.Image>().color = new Color(0.2f, 0.2f, 0.2f);

        var timerFill = new GameObject("TimerFill", typeof(RectTransform), typeof(UnityEngine.UI.Image));
        timerFill.transform.SetParent(timerBg.transform, false);
        var fillRect = timerFill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        var fillImage = timerFill.GetComponent<UnityEngine.UI.Image>();
        fillImage.color = Color.green;

        var hud = go.AddComponent<MultiplayerHUD>();
        var so = new SerializedObject(hud);
        so.FindProperty("_multiplayerPanel").objectReferenceValue = panel;
        so.FindProperty("_playerScoreText").objectReferenceValue = playerScore.GetComponent<TMPro.TextMeshProUGUI>();
        so.FindProperty("_opponentScoreText").objectReferenceValue = oppScore.GetComponent<TMPro.TextMeshProUGUI>();
        so.FindProperty("_playerNameText").objectReferenceValue = playerName.GetComponent<TMPro.TextMeshProUGUI>();
        so.FindProperty("_opponentNameText").objectReferenceValue = oppName.GetComponent<TMPro.TextMeshProUGUI>();
        so.FindProperty("_turnIndicatorText").objectReferenceValue = turnText.GetComponent<TMPro.TextMeshProUGUI>();
        so.FindProperty("_timerBar").objectReferenceValue = fillImage;
        so.ApplyModifiedPropertiesWithoutUndo();

        panel.SetActive(false);
        return hud;
    }

    private static OpponentVisualPlayer SetupOpponentVisualPlayer()
    {
        var existing = Object.FindAnyObjectByType<OpponentVisualPlayer>();
        if (existing != null) return existing;

        var go = new GameObject("OpponentVisualPlayer");
        return go.AddComponent<OpponentVisualPlayer>();
    }

    private static GameObject CreateTMP(Transform parent, string name, string text, int fontSize, TMPro.TextAlignmentOptions alignment)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(TMPro.TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var tmp = go.GetComponent<TMPro.TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = Color.white;
        return go;
    }

    private static void SetAnchor(GameObject go, float minX, float minY, float maxX, float maxY)
    {
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(minX, minY);
        rect.anchorMax = new Vector2(maxX, maxY);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void SetupGameplayScope(BoardConfig config, BoardView boardView, PieceTray pieceTray, ScoreUI scoreUI, GameplayHUD gameplayHUD, FeedbackConfig feedbackConfig, TutorialOverlay tutorialOverlay, MultiplayerHUD multiplayerHUD, OpponentVisualPlayer opponentVisualPlayer)
    {
        var tutorialConfig = AssetDatabase.LoadAssetAtPath<TutorialConfig>("Assets/ScriptableObjects/Tutorial/TutorialConfig.asset");
        var uiConfig = AssetDatabase.LoadAssetAtPath<UIConfig>("Assets/ScriptableObjects/UIConfig.asset");
        var audioConfig = AssetDatabase.LoadAssetAtPath<AudioConfig>("Assets/ScriptableObjects/AudioConfig.asset");
        var canvas = Object.FindAnyObjectByType<Canvas>();
        var popupContainer = canvas != null ? canvas.transform.Find("PopupContainer") : null;
        var existingScope = Object.FindAnyObjectByType<GameplayLifetimeScope>();

        System.Action<SerializedObject> applyFields = (so) =>
        {
            so.FindProperty("_boardConfig").objectReferenceValue = config;
            so.FindProperty("_boardView").objectReferenceValue = boardView;
            so.FindProperty("_pieceTray").objectReferenceValue = pieceTray;
            so.FindProperty("_scoreUI").objectReferenceValue = scoreUI;
            so.FindProperty("_gameplayHUD").objectReferenceValue = gameplayHUD;
            so.FindProperty("_feedbackConfig").objectReferenceValue = feedbackConfig;
            so.FindProperty("_tutorialOverlay").objectReferenceValue = tutorialOverlay;
            if (uiConfig != null)
                so.FindProperty("_uiConfig").objectReferenceValue = uiConfig;
            if (popupContainer != null)
                so.FindProperty("_popupContainer").objectReferenceValue = popupContainer;
            if (tutorialConfig != null)
                so.FindProperty("_tutorialConfig").objectReferenceValue = tutorialConfig;
            if (audioConfig != null)
                so.FindProperty("_audioConfig").objectReferenceValue = audioConfig;

            var mpConfig = AssetDatabase.LoadAssetAtPath<MultiplayerConfig>("Assets/ScriptableObjects/MultiplayerConfig.asset");
            if (mpConfig != null)
                so.FindProperty("_multiplayerConfig").objectReferenceValue = mpConfig;
            if (multiplayerHUD != null)
                so.FindProperty("_multiplayerHUD").objectReferenceValue = multiplayerHUD;
            if (opponentVisualPlayer != null)
                so.FindProperty("_opponentVisualPlayer").objectReferenceValue = opponentVisualPlayer;

            so.ApplyModifiedPropertiesWithoutUndo();
        };

        if (existingScope != null)
        {
            applyFields(new SerializedObject(existingScope));
            return;
        }

        var scopeGo = new GameObject("GameplayScope");
        var scope = scopeGo.AddComponent<GameplayLifetimeScope>();
        applyFields(new SerializedObject(scope));
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
