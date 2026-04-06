#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Forces play mode to always start from BootScene, restoring the previous scene when exiting play mode.
/// Toggled via the NumbersBlast menu.
/// </summary>
[InitializeOnLoad]
public static class GameFlowEnforcer
{
    private const string EnabledKey = "GameFlow_Enabled";
    private const string PreviousSceneKey = "GameFlow_PreviousScene";
    private const string MenuPath = "NumbersBlast/Enable Game Flow";
    private const string BootScenePath = "Assets/Scenes/BootScene.unity";

    /// <summary>
    /// Static constructor that subscribes to play mode state changes on editor load.
    /// </summary>
    static GameFlowEnforcer()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    /// <summary>
    /// Toggles the Game Flow enforcer on or off via the NumbersBlast menu.
    /// </summary>
    [MenuItem(MenuPath, priority = 100)]
    private static void ToggleGameFlow()
    {
        bool current = IsEnabled();
        SetEnabled(!current);
        Debug.Log($"[GameFlow] {(!current ? "Enabled" : "Disabled")} - {(!current ? "Will always start from BootScene" : "Start from current scene")}");
    }

    /// <summary>
    /// Validates the menu item state, updating the checkmark to reflect the current enabled status.
    /// </summary>
    [MenuItem(MenuPath, true)]
    private static bool ToggleGameFlowValidate()
    {
        Menu.SetChecked(MenuPath, IsEnabled());
        return true;
    }

    /// <summary>
    /// Handles play mode transitions: saves current scene and opens BootScene on enter, restores previous scene on exit.
    /// </summary>
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

    /// <summary>
    /// Returns whether the Game Flow enforcer is currently enabled in EditorPrefs.
    /// </summary>
    private static bool IsEnabled()
    {
        return EditorPrefs.GetBool(EnabledKey, true);
    }

    /// <summary>
    /// Persists the enabled state of the Game Flow enforcer to EditorPrefs.
    /// </summary>
    private static void SetEnabled(bool value)
    {
        EditorPrefs.SetBool(EnabledKey, value);
    }
}
#endif
