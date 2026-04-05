#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class GameFlowEnforcer
{
    private const string EnabledKey = "GameFlow_Enabled";
    private const string PreviousSceneKey = "GameFlow_PreviousScene";
    private const string MenuPath = "NumbersBlast/Enable Game Flow";
    private const string BootScenePath = "Assets/Scenes/BootScene.unity";

    static GameFlowEnforcer()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    [MenuItem(MenuPath, priority = 100)]
    private static void ToggleGameFlow()
    {
        bool current = IsEnabled();
        SetEnabled(!current);
        Debug.Log($"[GameFlow] {(!current ? "Enabled" : "Disabled")} - {(!current ? "Will always start from BootScene" : "Start from current scene")}");
    }

    [MenuItem(MenuPath, true)]
    private static bool ToggleGameFlowValidate()
    {
        Menu.SetChecked(MenuPath, IsEnabled());
        return true;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (!IsEnabled()) return;

        if (state == PlayModeStateChange.ExitingEditMode)
        {
            var currentScene = EditorSceneManager.GetActiveScene();
            if (currentScene.path == BootScenePath) return;

            EditorPrefs.SetString(PreviousSceneKey, currentScene.path);
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(BootScenePath);
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            var previousScene = EditorPrefs.GetString(PreviousSceneKey, "");
            if (!string.IsNullOrEmpty(previousScene) && previousScene != BootScenePath)
            {
                EditorSceneManager.OpenScene(previousScene);
                EditorPrefs.DeleteKey(PreviousSceneKey);
            }
        }
    }

    private static bool IsEnabled()
    {
        return EditorPrefs.GetBool(EnabledKey, true);
    }

    private static void SetEnabled(bool value)
    {
        EditorPrefs.SetBool(EnabledKey, value);
    }
}
#endif
