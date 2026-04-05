#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public static class ApplySpritesTool
{
    private const string SpritesSOPath = "Assets/Dev/DevUISprites.asset";

    [MenuItem("NumbersBlast/Apply UI Sprites")]
    private static void ApplySprites()
    {
        var sprites = AssetDatabase.LoadAssetAtPath<DevUISprites>(SpritesSOPath);
        if (sprites == null)
        {
            Debug.LogWarning("[ApplySprites] DevUISprites SO not found. Create it first.");
            return;
        }

        // Apply to current scene
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        var rootObjects = scene.GetRootGameObjects();

        foreach (var root in rootObjects)
        {
            ApplyToMainMenu(root, sprites);
            ApplyToGameplayHUD(root, sprites);
            ApplyToPopups(root, sprites);
            ApplyToMultiplayerHUD(root, sprites);
            ApplyToBackground(root, sprites);
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);

        // Apply to popup prefabs
        ApplyToPrefab("Assets/Prefabs/UI/GameOverPopup.prefab", sprites);
        ApplyToPrefab("Assets/Prefabs/UI/PausePopup.prefab", sprites);
        ApplyToPrefab("Assets/Prefabs/UI/SettingsPopup.prefab", sprites);
        ApplyToPrefab("Assets/Prefabs/UI/TutorialFeedbackPopup.prefab", sprites);
        ApplyToPrefab("Assets/Prefabs/UI/OpponentSearchPopup.prefab", sprites);

        AssetDatabase.SaveAssets();
        Debug.Log($"[ApplySprites] Sprites applied to scene + prefabs.");
    }

    private static void ApplyToPrefab(string path, DevUISprites sprites)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null) return;

        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        ApplyPopupButtons(instance, sprites);
        ApplyPopupBackground(instance, sprites);

        PrefabUtility.SaveAsPrefabAsset(instance, path);
        Object.DestroyImmediate(instance);
    }

    private static void ApplyPopupButtons(GameObject root, DevUISprites sprites)
    {
        ApplyButton(root, "ResumeButton", sprites.ResumeButtonSprite);
        ApplyButton(root, "RestartButton", sprites.RestartButtonSprite);
        ApplyButton(root, "MainMenuButton", sprites.MainMenuButtonSprite);
        ApplyButton(root, "ContinueButton", sprites.ContinueButtonSprite);
        ApplyButton(root, "CloseButton", sprites.CloseButtonSprite);
        ApplyButton(root, "CancelButton", sprites.CancelButtonSprite);
        ApplyButton(root, "MusicToggle", sprites.ToggleOnSprite);
        ApplyButton(root, "SFXToggle", sprites.ToggleOnSprite);
        ApplyButton(root, "HapticToggle", sprites.ToggleOnSprite);
    }

    private static void ApplyPopupBackground(GameObject root, DevUISprites sprites)
    {
        if (sprites.DimBackgroundSprite != null)
        {
            var dim = FindDeep(root.transform, "DimBackground");
            if (dim != null) ApplySprite(dim.GetComponent<Image>(), sprites.DimBackgroundSprite);
        }

        if (sprites.PopupBackgroundSprite != null)
        {
            var content = FindDeep(root.transform, "Content");
            if (content != null)
            {
                var image = content.GetComponent<Image>();
                if (image == null)
                    image = content.gameObject.AddComponent<Image>();
                ApplySprite(image, sprites.PopupBackgroundSprite);
            }
        }
    }

    private static void ApplyToMainMenu(GameObject root, DevUISprites sprites)
    {
        ApplyButton(root, "PlayButton", sprites.PlayButtonSprite);
        ApplyButton(root, "MultiplayerButton", sprites.MultiplayerButtonSprite);
        ApplyButton(root, "SettingsButton", sprites.SettingsButtonSprite);
        ApplyButton(root, "ExitButton", sprites.ExitButtonSprite);
    }

    private static void ApplyToGameplayHUD(GameObject root, DevUISprites sprites)
    {
        ApplyButton(root, "PauseButton", sprites.PauseButtonSprite);

        // Settings in HUD uses GameSettingsButton sprite
        var settingsBtn = FindDeep(root.transform, "SettingsButton");
        if (settingsBtn != null && sprites.GameSettingsButtonSprite != null)
        {
            var parent = settingsBtn.parent;
            if (parent != null && parent.GetComponent<GameplayHUD>() != null)
                ApplySprite(settingsBtn.GetComponent<Image>(), sprites.GameSettingsButtonSprite);
        }

        ApplyImage(root, "ScorePanel", sprites.ScoreBackgroundSprite);
    }

    private static void ApplyToPopups(GameObject root, DevUISprites sprites)
    {
        // Popup backgrounds
        var popupContainer = FindDeep(root.transform, "PopupContainer");
        if (popupContainer != null)
        {
            foreach (Transform popup in popupContainer)
            {
                var dim = FindChild(popup, "DimBackground");
                if (dim != null && sprites.DimBackgroundSprite != null)
                    ApplySprite(dim.GetComponent<Image>(), sprites.DimBackgroundSprite);

                var content = FindChild(popup, "Content");
                if (content != null && sprites.PopupBackgroundSprite != null)
                {
                    var contentImage = content.GetComponent<Image>();
                    if (contentImage == null)
                        contentImage = content.gameObject.AddComponent<Image>();
                    ApplySprite(contentImage, sprites.PopupBackgroundSprite);
                }
            }
        }

        // Popup buttons
        ApplyButton(root, "ResumeButton", sprites.ResumeButtonSprite);
        ApplyButton(root, "RestartButton", sprites.RestartButtonSprite);
        ApplyButton(root, "MainMenuButton", sprites.MainMenuButtonSprite);
        ApplyButton(root, "ContinueButton", sprites.ContinueButtonSprite);
        ApplyButton(root, "CloseButton", sprites.CloseButtonSprite);
        ApplyButton(root, "CancelButton", sprites.CancelButtonSprite);

        // Toggle buttons
        ApplyButton(root, "MusicToggle", sprites.ToggleOnSprite);
        ApplyButton(root, "SFXToggle", sprites.ToggleOnSprite);
        ApplyButton(root, "HapticToggle", sprites.ToggleOnSprite);
    }

    private static void ApplyToMultiplayerHUD(GameObject root, DevUISprites sprites)
    {
        ApplyImage(root, "TimerBg", sprites.TimerBarBackgroundSprite);
        ApplyImage(root, "TimerFill", sprites.TimerBarSprite);
    }

    private static void ApplyToBackground(GameObject root, DevUISprites sprites)
    {
        var bg = FindChild(root.transform, "Background");
        if (bg != null && sprites.BackgroundSprite != null)
            ApplySprite(bg.GetComponent<Image>(), sprites.BackgroundSprite);
    }

    private const float ButtonHeight = 120f;

    private static void ApplyButton(GameObject root, string name, Sprite sprite)
    {
        if (sprite == null) return;
        var transform = FindDeep(root.transform, name);
        if (transform == null) return;
        var image = transform.GetComponent<Image>();
        if (image != null)
        {
            ApplySprite(image, sprite);
            var rect = transform.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, ButtonHeight);
            EditorUtility.SetDirty(rect);
        }
    }

    private static void ApplyImage(GameObject root, string name, Sprite sprite)
    {
        if (sprite == null) return;
        var transform = FindDeep(root.transform, name);
        if (transform == null) return;
        var image = transform.GetComponent<Image>();
        if (image != null)
            ApplySprite(image, sprite);
    }

    private static void ApplySprite(Image image, Sprite sprite)
    {
        if (image == null || sprite == null) return;
        image.sprite = sprite;
        image.color = Color.white;
        image.type = Image.Type.Sliced;
        image.pixelsPerUnitMultiplier = 1f;
        EditorUtility.SetDirty(image);
    }

    private static Transform FindDeep(Transform parent, string name)
    {
        if (parent.name == name) return parent;
        foreach (Transform child in parent)
        {
            var result = FindDeep(child, name);
            if (result != null) return result;
        }
        return null;
    }

    private static Transform FindChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
        }
        return null;
    }
}
#endif
