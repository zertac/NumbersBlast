#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public static class MultiplayerSetupTool
{
    private const string SOPath = "Assets/ScriptableObjects";
    private const string PrefabPath = "Assets/Prefabs/UI";

    public static void RunSetup() => SetupMultiplayer();

    [MenuItem("NumbersBlast/Setup Multiplayer")]
    private static void SetupMultiplayer()
    {
        CreateMultiplayerConfig();
        CreateOpponentSearchPrefab();
        UpdateUIConfig();

        AssetDatabase.SaveAssets();
        Debug.Log("[MultiplayerSetup] Multiplayer setup complete.");
    }

    private static void CreateMultiplayerConfig()
    {
        var path = SOPath + "/MultiplayerConfig.asset";
        if (AssetDatabase.LoadAssetAtPath<MultiplayerConfig>(path) != null) return;

        var config = ScriptableObject.CreateInstance<MultiplayerConfig>();
        config.TurnDuration = 20f;
        config.PenaltyPercent = 0.05f;
        config.MinSearchDuration = 2f;
        config.MaxSearchDuration = 5f;
        config.FakeNames = new[]
        {
            "BlockMaster99", "PuzzleKing", "NumberNinja", "MergeWizard",
            "BlastPro", "GridHero", "ClearQueen", "ComboLord",
            "TileTitan", "RowBreaker", "ChainMaster", "ScoreHunter"
        };
        config.SkillLevel = 0.7f;
        config.MistakeChance = 0.2f;
        config.HesitationChance = 0.5f;
        config.CancelChance = 0.3f;
        config.MinThinkTime = 1.5f;
        config.MaxThinkTime = 4f;
        config.MinHoverTime = 0.5f;
        config.MaxHoverTime = 1.5f;
        config.MinHesitationTime = 0.8f;
        config.MaxHesitationTime = 2f;
        config.MoveSpeed = 0.4f;

        AssetDatabase.CreateAsset(config, path);
    }

    private static void CreateOpponentSearchPrefab()
    {
        var path = PrefabPath + "/OpponentSearchPopup.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

        UISetupTool.RunSetup(); // Ensure UIButton prefab exists

        var root = CreatePopupRoot("OpponentSearchPopup");
        var content = CreatePopupContent(root.transform);
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 350);

        var status = CreateText(content.transform, "StatusText", "Searching for opponent...", 28, FontStyles.Normal);
        SetAnchors(status, 0, 0.6f, 1, 0.85f);

        var opponentName = CreateText(content.transform, "OpponentName", "", 42, FontStyles.Bold);
        SetAnchors(opponentName, 0, 0.3f, 1, 0.6f);

        var cancelBtn = CreateButton(content.transform, "CancelButton", "CANCEL", 0.25f, 0.05f, 0.75f, 0.25f);
        cancelBtn.GetComponent<Image>().color = new Color(0.7f, 0.35f, 0.35f);

        var popup = root.AddComponent<OpponentSearchPopup>();
        var so = new SerializedObject(popup);
        so.FindProperty("_dimBackground").objectReferenceValue = root.transform.Find("DimBackground").GetComponent<Image>();
        so.FindProperty("_content").objectReferenceValue = content.GetComponent<RectTransform>();
        so.FindProperty("_statusText").objectReferenceValue = status.GetComponent<TextMeshProUGUI>();
        so.FindProperty("_opponentNameText").objectReferenceValue = opponentName.GetComponent<TextMeshProUGUI>();
        so.FindProperty("_cancelButton").objectReferenceValue = cancelBtn.GetComponent<Button>();
        so.ApplyModifiedPropertiesWithoutUndo();

        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
    }

    private static void UpdateUIConfig()
    {
        var configPath = SOPath + "/UIConfig.asset";
        var config = AssetDatabase.LoadAssetAtPath<UIConfig>(configPath);
        if (config == null) return;

        var searchPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath + "/OpponentSearchPopup.prefab");
        if (searchPrefab == null) return;

        // Check if already added
        for (int i = 0; i < config.Popups.Length; i++)
        {
            if (config.Popups[i].Prefab != null && config.Popups[i].Prefab.GetComponent<OpponentSearchPopup>() != null) return;
        }

        var newPopups = new PopupEntry[config.Popups.Length + 1];
        config.Popups.CopyTo(newPopups, 0);
        newPopups[config.Popups.Length] = new PopupEntry { Prefab = searchPrefab };
        config.Popups = newPopups;
        EditorUtility.SetDirty(config);
    }

    // === Helpers (duplicated from UISetupTool for independence) ===

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
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath + "/UIButton.prefab");
        var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        go.transform.SetParent(parent, false);
        go.name = name;
        go.GetComponentInChildren<TextMeshProUGUI>().text = text;
        SetAnchors(go, minX, minY, maxX, maxY);
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
