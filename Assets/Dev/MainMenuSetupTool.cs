#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

public static class MainMenuSetupTool
{
    [MenuItem("NumbersBlast/Setup Main Menu Scene")]
    private static void SetupMainMenu()
    {
        // Ensure UIButton prefab exists
        UISetupTool.RunSetup();
        // Create new scene
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Camera
        var cameraGo = new GameObject("Main Camera");
        var camera = cameraGo.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.12f, 0.12f, 0.25f);
        cameraGo.AddComponent<AudioListener>();
        cameraGo.tag = "MainCamera";

        // Canvas
        var canvasGo = new GameObject("Canvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGo.AddComponent<GraphicRaycaster>();

        // Event System
        var eventGo = new GameObject("EventSystem");
        eventGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventGo.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        // Background
        var bgGo = new GameObject("Background", typeof(RectTransform), typeof(Image));
        bgGo.transform.SetParent(canvasGo.transform, false);
        var bgRect = bgGo.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgGo.GetComponent<Image>().color = new Color(0.12f, 0.12f, 0.25f);

        // Title
        var titleGo = new GameObject("Title", typeof(RectTransform), typeof(TextMeshProUGUI));
        titleGo.transform.SetParent(canvasGo.transform, false);
        var titleRect = titleGo.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.1f, 0.65f);
        titleRect.anchorMax = new Vector2(0.9f, 0.85f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        var titleText = titleGo.GetComponent<TextMeshProUGUI>();
        titleText.text = "NUMBERS\nBLAST";
        titleText.fontSize = 72;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;

        // Button container
        var buttonsGo = new GameObject("Buttons", typeof(RectTransform), typeof(VerticalLayoutGroup));
        buttonsGo.transform.SetParent(canvasGo.transform, false);
        var buttonsRect = buttonsGo.GetComponent<RectTransform>();
        buttonsRect.anchorMin = new Vector2(0.2f, 0.25f);
        buttonsRect.anchorMax = new Vector2(0.8f, 0.55f);
        buttonsRect.offsetMin = Vector2.zero;
        buttonsRect.offsetMax = Vector2.zero;
        var layout = buttonsGo.GetComponent<VerticalLayoutGroup>();
        layout.spacing = 20;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        layout.childControlWidth = true;
        layout.childControlHeight = false;

        var playBtn = CreateButton(buttonsGo.transform, "PlayButton", "PLAY", new Color(0.3f, 0.75f, 0.4f), 80);
        var settingsBtn = CreateButton(buttonsGo.transform, "SettingsButton", "SETTINGS", new Color(0.4f, 0.5f, 0.8f), 70);
        var exitBtn = CreateButton(buttonsGo.transform, "ExitButton", "EXIT", new Color(0.7f, 0.35f, 0.35f), 70);

        // Popup container
        var popupContainerGo = new GameObject("PopupContainer", typeof(RectTransform));
        popupContainerGo.transform.SetParent(canvasGo.transform, false);
        popupContainerGo.transform.SetAsLastSibling();
        var pcRect = popupContainerGo.GetComponent<RectTransform>();
        pcRect.anchorMin = Vector2.zero;
        pcRect.anchorMax = Vector2.one;
        pcRect.sizeDelta = Vector2.zero;

        // MainMenuUI
        var uiConfig = AssetDatabase.LoadAssetAtPath<UIConfig>("Assets/ScriptableObjects/UIConfig.asset");
        var audioConfig = AssetDatabase.LoadAssetAtPath<AudioConfig>("Assets/ScriptableObjects/AudioConfig.asset");
        var menuUI = canvasGo.AddComponent<MainMenuUI>();
        var so = new SerializedObject(menuUI);
        so.FindProperty("_playButton").objectReferenceValue = playBtn.GetComponent<Button>();
        so.FindProperty("_settingsButton").objectReferenceValue = settingsBtn.GetComponent<Button>();
        so.FindProperty("_exitButton").objectReferenceValue = exitBtn.GetComponent<Button>();
        so.FindProperty("_uiConfig").objectReferenceValue = uiConfig;
        so.FindProperty("_audioConfig").objectReferenceValue = audioConfig;
        so.FindProperty("_popupContainer").objectReferenceValue = popupContainerGo.transform;
        so.ApplyModifiedPropertiesWithoutUndo();

        // Save scene
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenuScene.unity");
        Debug.Log("[MainMenuSetup] Main menu scene created. Add both scenes to Build Settings.");
    }

    private static GameObject CreateButton(Transform parent, string name, string text, Color color, float height)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/UIButton.prefab");
        var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        go.transform.SetParent(parent, false);
        go.name = name;
        go.GetComponent<RectTransform>().sizeDelta = new Vector2(0, height);
        go.GetComponent<Image>().color = color;
        go.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = text;
        return go;
    }
}
#endif
