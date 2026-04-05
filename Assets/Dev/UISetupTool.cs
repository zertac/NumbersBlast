#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public static class UISetupTool
{
    private const string PrefabPath = "Assets/Prefabs/UI";
    private const string SOPath = "Assets/ScriptableObjects";

    public static void RunSetup() => SetupUISystem();

    [MenuItem("NumbersBlast/Setup UI System")]
    private static void SetupUISystem()
    {
        if (!AssetDatabase.IsValidFolder(PrefabPath))
        {
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            AssetDatabase.CreateFolder("Assets/Prefabs", "UI");
        }

        CreateUIButtonPrefab();
        CreateGameOverPrefab();
        CreateTutorialFeedbackPrefab();
        CreateSettingsPrefab();
        CreatePausePrefab();
        CreateUIConfig();
        SetupPopupContainer();

        AssetDatabase.SaveAssets();
        Debug.Log("[UISetupTool] UI system setup complete.");
    }

    private static void SetupPopupContainer()
    {
        var canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) return;

        var container = canvas.transform.Find("PopupContainer");
        if (container != null) return;

        var containerGo = new GameObject("PopupContainer", typeof(RectTransform));
        containerGo.transform.SetParent(canvas.transform, false);
        containerGo.transform.SetAsLastSibling();
        var containerRect = containerGo.GetComponent<RectTransform>();
        containerRect.anchorMin = Vector2.zero;
        containerRect.anchorMax = Vector2.one;
        containerRect.sizeDelta = Vector2.zero;
    }

    private static void CreateGameOverPrefab()
    {
        var path = PrefabPath + "/GameOverPopup.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

        var root = CreatePopupRoot("GameOverPopup");
        var content = CreatePopupContent(root.transform);

        var title = CreateText(content.transform, "Title", "GAME OVER", 56, FontStyles.Bold);
        SetAnchors(title, 0, 0.65f, 1, 1);

        var score = CreateText(content.transform, "FinalScore", "0", 72, FontStyles.Bold);
        SetAnchors(score, 0, 0.35f, 1, 0.65f);

        var button = CreateButton(content.transform, "RestartButton", "RESTART", 0.2f, 0.05f, 0.8f, 0.3f);

        var gameOverUI = root.AddComponent<GameOverUI>();
        var so = new SerializedObject(gameOverUI);
        so.FindProperty("_dimBackground").objectReferenceValue = root.transform.Find("DimBackground").GetComponent<Image>();
        so.FindProperty("_content").objectReferenceValue = content.GetComponent<RectTransform>();
        so.FindProperty("_finalScoreText").objectReferenceValue = score.GetComponent<TextMeshProUGUI>();
        so.FindProperty("_restartButton").objectReferenceValue = button.GetComponent<Button>();
        so.ApplyModifiedPropertiesWithoutUndo();

        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
    }

    private static void CreateTutorialFeedbackPrefab()
    {
        var path = PrefabPath + "/TutorialFeedbackPopup.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

        var root = CreatePopupRoot("TutorialFeedbackPopup");
        var content = CreatePopupContent(root.transform);

        var title = CreateText(content.transform, "Title", "", 48, FontStyles.Bold);
        SetAnchors(title, 0, 0.6f, 1, 1);

        var desc = CreateText(content.transform, "Description", "", 28, FontStyles.Normal);
        SetAnchors(desc, 0, 0.3f, 1, 0.6f);

        var button = CreateButton(content.transform, "ContinueButton", "CONTINUE", 0.2f, 0.05f, 0.8f, 0.25f);

        var popup = root.AddComponent<TutorialFeedbackPopup>();
        var so = new SerializedObject(popup);
        so.FindProperty("_dimBackground").objectReferenceValue = root.transform.Find("DimBackground").GetComponent<Image>();
        so.FindProperty("_content").objectReferenceValue = content.GetComponent<RectTransform>();
        so.FindProperty("_titleText").objectReferenceValue = title.GetComponent<TextMeshProUGUI>();
        so.FindProperty("_descriptionText").objectReferenceValue = desc.GetComponent<TextMeshProUGUI>();
        so.FindProperty("_continueButton").objectReferenceValue = button.GetComponent<Button>();
        so.ApplyModifiedPropertiesWithoutUndo();

        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
    }

    private static void CreateSettingsPrefab()
    {
        var path = PrefabPath + "/SettingsPopup.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

        var root = CreatePopupRoot("SettingsPopup");
        var content = CreatePopupContent(root.transform);
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 450);

        var title = CreateText(content.transform, "Title", "SETTINGS", 48, FontStyles.Bold);
        SetAnchors(title, 0, 0.82f, 1, 1);

        var musicBtn = CreateToggleButton(content.transform, "MusicToggle", "Music: ON", 0.1f, 0.58f, 0.9f, 0.75f);
        var sfxBtn = CreateToggleButton(content.transform, "SFXToggle", "SFX: ON", 0.1f, 0.38f, 0.9f, 0.55f);
        var hapticBtn = CreateToggleButton(content.transform, "HapticToggle", "Haptic: ON", 0.1f, 0.18f, 0.9f, 0.35f);
        var closeBtn = CreateButton(content.transform, "CloseButton", "CLOSE", 0.2f, 0.02f, 0.8f, 0.15f);

        var settingsPopup = root.AddComponent<SettingsPopup>();
        var so = new SerializedObject(settingsPopup);
        so.FindProperty("_dimBackground").objectReferenceValue = root.transform.Find("DimBackground").GetComponent<Image>();
        so.FindProperty("_content").objectReferenceValue = content.GetComponent<RectTransform>();
        so.FindProperty("_musicToggle").objectReferenceValue = musicBtn.GetComponent<Button>();
        so.FindProperty("_sfxToggle").objectReferenceValue = sfxBtn.GetComponent<Button>();
        so.FindProperty("_hapticToggle").objectReferenceValue = hapticBtn.GetComponent<Button>();
        so.FindProperty("_closeButton").objectReferenceValue = closeBtn.GetComponent<Button>();
        so.FindProperty("_musicLabel").objectReferenceValue = musicBtn.GetComponentInChildren<TextMeshProUGUI>();
        so.FindProperty("_sfxLabel").objectReferenceValue = sfxBtn.GetComponentInChildren<TextMeshProUGUI>();
        so.FindProperty("_hapticLabel").objectReferenceValue = hapticBtn.GetComponentInChildren<TextMeshProUGUI>();
        so.ApplyModifiedPropertiesWithoutUndo();

        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
    }

    private static GameObject CreateToggleButton(Transform parent, string name, string text, float minX, float minY, float maxX, float maxY)
    {
        var go = InstantiateButtonPrefab(parent, name, text);
        SetAnchors(go, minX, minY, maxX, maxY);
        go.GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.4f);
        go.GetComponentInChildren<TextMeshProUGUI>().fontSize = 28;
        return go;
    }

    private static void CreatePausePrefab()
    {
        var path = PrefabPath + "/PausePopup.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

        var root = CreatePopupRoot("PausePopup");
        var content = CreatePopupContent(root.transform);
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 450);

        var title = CreateText(content.transform, "Title", "PAUSED", 56, FontStyles.Bold);
        SetAnchors(title, 0, 0.8f, 1, 1);

        var resumeBtn = CreateButton(content.transform, "ResumeButton", "RESUME", 0.15f, 0.52f, 0.85f, 0.72f);
        resumeBtn.GetComponent<Image>().color = new Color(0.3f, 0.75f, 0.4f);

        var restartBtn = CreateButton(content.transform, "RestartButton", "RESTART", 0.15f, 0.28f, 0.85f, 0.48f);
        restartBtn.GetComponent<Image>().color = new Color(0.4f, 0.5f, 0.8f);

        var menuBtn = CreateButton(content.transform, "MainMenuButton", "MAIN MENU", 0.15f, 0.04f, 0.85f, 0.24f);
        menuBtn.GetComponent<Image>().color = new Color(0.7f, 0.35f, 0.35f);

        var pausePopup = root.AddComponent<PausePopup>();
        var so = new SerializedObject(pausePopup);
        so.FindProperty("_dimBackground").objectReferenceValue = root.transform.Find("DimBackground").GetComponent<Image>();
        so.FindProperty("_content").objectReferenceValue = content.GetComponent<RectTransform>();
        so.FindProperty("_resumeButton").objectReferenceValue = resumeBtn.GetComponent<Button>();
        so.FindProperty("_restartButton").objectReferenceValue = restartBtn.GetComponent<Button>();
        so.FindProperty("_mainMenuButton").objectReferenceValue = menuBtn.GetComponent<Button>();
        so.ApplyModifiedPropertiesWithoutUndo();

        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
    }

    private static void CreateUIConfig()
    {
        var path = SOPath + "/UIConfig.asset";
        if (AssetDatabase.LoadAssetAtPath<UIConfig>(path) != null) return;

        var config = ScriptableObject.CreateInstance<UIConfig>();

        var gameOverPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath + "/GameOverPopup.prefab");
        var tutorialPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath + "/TutorialFeedbackPopup.prefab");
        var settingsPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath + "/SettingsPopup.prefab");
        var pausePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath + "/PausePopup.prefab");

        config.Popups = new PopupEntry[]
        {
            new() { Type = PopupType.GameOver, Prefab = gameOverPrefab },
            new() { Type = PopupType.TutorialFeedback, Prefab = tutorialPrefab },
            new() { Type = PopupType.Settings, Prefab = settingsPrefab },
            new() { Type = PopupType.Pause, Prefab = pausePrefab }
        };

        AssetDatabase.CreateAsset(config, path);
    }

    private static void CreateUIButtonPrefab()
    {
        var path = PrefabPath + "/UIButton.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

        var go = new GameObject("UIButton", typeof(RectTransform), typeof(Image), typeof(Button), typeof(UIButton));
        go.GetComponent<Image>().color = new Color(0.3f, 0.7f, 0.4f);

        var uiBtn = new SerializedObject(go.GetComponent<UIButton>());
        uiBtn.FindProperty("_button").objectReferenceValue = go.GetComponent<Button>();
        uiBtn.ApplyModifiedPropertiesWithoutUndo();

        var textGo = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textGo.transform.SetParent(go.transform, false);
        var textRect = textGo.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        var tmp = textGo.GetComponent<TextMeshProUGUI>();
        tmp.text = "BUTTON";
        tmp.fontSize = 32;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
    }

    // === Helpers ===

    private static GameObject CreatePopupRoot(string name)
    {
        var root = new GameObject(name, typeof(RectTransform), typeof(CanvasGroup));
        var rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.sizeDelta = Vector2.zero;

        var dim = new GameObject("DimBackground", typeof(RectTransform), typeof(Image));
        dim.transform.SetParent(root.transform, false);
        var dimRect = dim.GetComponent<RectTransform>();
        dimRect.anchorMin = Vector2.zero;
        dimRect.anchorMax = Vector2.one;
        dimRect.sizeDelta = Vector2.zero;
        dim.GetComponent<Image>().color = new Color(0, 0, 0, 0.6f);

        return root;
    }

    private static GameObject CreatePopupContent(Transform parent)
    {
        var content = new GameObject("Content", typeof(RectTransform));
        content.transform.SetParent(parent, false);
        var rect = content.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(500, 400);
        return content;
    }

    private static GameObject CreateText(Transform parent, string name, string text, int fontSize, FontStyles style)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        return go;
    }

    private static GameObject CreateButton(Transform parent, string name, string text, float minX, float minY, float maxX, float maxY)
    {
        var go = InstantiateButtonPrefab(parent, name, text);
        SetAnchors(go, minX, minY, maxX, maxY);
        return go;
    }

    private static GameObject InstantiateButtonPrefab(Transform parent, string name, string text)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath + "/UIButton.prefab");
        var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        go.transform.SetParent(parent, false);
        go.name = name;
        go.GetComponentInChildren<TextMeshProUGUI>().text = text;
        return go;
    }

    private static void SetAnchors(GameObject go, float minX, float minY, float maxX, float maxY)
    {
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(minX, minY);
        rect.anchorMax = new Vector2(maxX, maxY);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
#endif
